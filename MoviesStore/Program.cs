using Microsoft.EntityFrameworkCore;
using MoviesStore.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using System.Globalization;
using MoviesStore.Services;
using MoviesStore.data;



var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;


services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


services.AddControllers();
services.AddEndpointsApiExplorer();

services.AddScoped<IJwtService, JwtService>();

services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "MoviesStore", Version = "v1" });

    // 🔒 Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your JWT token.\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI..."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

services.AddCors(options =>
{
    options.AddPolicy("allowedDomains", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000").
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
        config.Window= TimeSpan.FromSeconds(10);
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
        options.RequireHttpsMetadata= false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
            {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer= jwtSection["Issuer"],
            ValidAudience= jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey((key))
        };
    });

services.AddAuthorization();


var app = builder.Build();




//----------------------------middleware pipeline configuration----------------------------//


if (app.Environment.IsDevelopment() ) 
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        DbSeeder.Seed(db);
    }
}

else
{
       app.UseExceptionHandler(errApp =>
       {
           errApp.Run(async context =>
           {
               context.Response.StatusCode = 500;
               context.Response.ContentType = "application/json";
               var errorFeature =context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
               var exception = errorFeature?.Error;
               var result = System.Text.Json.JsonSerializer.Serialize( new { message ="An internal server error occured" });
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
    .AddSupportedCultures(supportedCultures.Select(c => c.Name).ToArray())
    .AddSupportedUICultures(supportedCultures.Select(c => c.Name).ToArray());

app.UseRequestLocalization(localizationOptions);

app.UseResponseCaching();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
