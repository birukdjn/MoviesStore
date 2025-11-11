using Backend.data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.IO;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class PaymentsController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _db;

        public PaymentsController(IConfiguration config, AppDbContext db)
        {
            _config= config;
            _db= db;
        }

        // Create a Checkout Session (client will redirect to session.Url)
        [HttpPost("create-checkout-session")]
        [Authorize]
        public async Task<IActionResult> CreateCheckoutSession()
        {
            // Get logged-in user id (however you store it — example using ClaimTypes.NameIdentifier)
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var stripeKey = _config["Stripe:SecretKey"];
            var domain = _config["App:FrontendBaseUrl"];


            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = 999,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "MoviesStore Monthly Subscription",
                            Description = "Unlimited access for 1 month"
                        },
                        // Optionally: Recurring price for subscriptions (see below)
                    },
                    Quantity = 1
                }
            },
                Mode = "payment", // you'd use "subscription" for recurring with Stripe Price objects
                SuccessUrl = $"{domain}/success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{domain}/cancel",
                ClientReferenceId = userId.ToString(), // handy to connect session -> user
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Ok(new { url = session.Url, id = session.Id });
        }

        // Webhook to receive events from Stripe
        [HttpPost("webhook")]
        [AllowAnonymous] // Stripe calls this, no auth header
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var webhookSecret = _config["Stripe:WebhookSecret"];

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    webhookSecret
                );

                
                    if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
                    {
                    var session = stripeEvent.Data.Object as Session;
                    // Get user id from ClientReferenceId
                    if (!string.IsNullOrEmpty(session.ClientReferenceId) && int.TryParse(session.ClientReferenceId, out var userId))
                    {
                        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                        if (user != null)
                        {
                            // Mark subscription: for simple payments we give 30 days; adjust as needed
                            user.IsSubscribed = true;
                            user.SubscriptionExpiresAt = DateTime.UtcNow.AddDays(30);
                            await _db.SaveChangesAsync();
                        }
                    }
                }

                // You can handle other events (invoice.paid, payment_intent.succeeded...) for recurring billing
                return Ok();
            }
            catch (StripeException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}