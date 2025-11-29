using Backend.Attributes;
using Backend.Configuration;
using Backend.data;
using Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Stripe;
using System.Globalization;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

var emailConfig = configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();


if (emailConfig is not null)
{
    services.AddSingleton(emailConfig);
}

services.AddScoped<IEmailSender, EmailSender>();

services.AddHttpClient<ChapaService>();

builder.Services.Configure<TwilioSettings>(
    builder.Configuration.GetSection("Twilio"));

// 2. Register the SMS service
services.AddScoped<ISmsService, SmsService>();

services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
services.AddEndpointsApiExplorer();

services.AddScoped<IJwtService, JwtService>();

services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MoviesStore", Version = "v1" });

    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });



});

services.AddCors(options =>
{
    options.AddPolicy("allowedDomains", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://192.168.100.167:3000").
                AllowAnyHeader().
                AllowAnyMethod().
                AllowCredentials();
    });
});

services.AddResponseCompression();
services.AddResponseCaching();
services.AddLocalization();

services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", config =>
    {
        config.PermitLimit = 20;
        config.Window = TimeSpan.FromSeconds(10);
        config.QueueLimit = 10;
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
    options.RejectionStatusCode = 429;
});

var jwtSection = configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection.GetValue<string>("Key")!);

services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey((key))
        };
    });


services.AddAuthorizationBuilder()
        .AddPolicy("SubscribedOnly", policy =>
        {
            policy.RequireAssertion(context =>
            {
                var isSubscribed = context.User.HasClaim(c => c.Type == "IsSubscribed" && c.Value == "true");
                return isSubscribed;
            });
        });

builder.Services.AddScoped<RequireSubscriptionAttribute>();


var stripeSection = configuration.GetSection("Stripe");
StripeConfiguration.ApiKey = stripeSection.GetValue<string>("SecretKey");




var app = builder.Build();


//----------------------------middleware pipeline configuration----------------------------//


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger(options =>
    {
        options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
    });
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("v1/swagger.json", "MoviesStore V1");
    });

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(db);
}

else
{
    app.UseExceptionHandler(errApp =>
    {
        errApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var errorFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
            var exception = errorFeature?.Error;
            var result = System.Text.Json.JsonSerializer.Serialize(new { message = "An internal server error occured" });
            await context.Response.WriteAsync(result);
        });
    });
}
app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseRouting();

app.UseCors("allowedDomains");



var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("fr-FR"), new CultureInfo("es-ES"), new CultureInfo("de-DE"), new CultureInfo("it-IT") };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en-US")
    .AddSupportedCultures([.. supportedCultures.Select(c => c.Name)])
    .AddSupportedUICultures([.. supportedCultures.Select(c => c.Name)]);

app.UseRequestLocalization(localizationOptions);

app.UseResponseCaching();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
