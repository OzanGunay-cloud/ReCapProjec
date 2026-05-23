using Autofac;
using Autofac.Extensions.DependencyInjection;
using Business.DependencyResolvers.Autofac;
using Core.DependencyResolvers;
using Core.Extensions.DependencyInjection;
using Core.Utilities.IoC;
using Core.Utilities.Security.Encryption; // SecurityKeyHelper için gerekli
using Core.Utilities.Security.JWT; // TokenOptions için gerekli
using Microsoft.AspNetCore.Authentication.JwtBearer; // Bu namespace şart
using Microsoft.IdentityModel.Tokens; // TokenValidationParameters için şart
using Microsoft.OpenApi.Models;
using SessionSentinel.WebApi;
using WebAPI.Options;
using WebAPI.Services;
using WebAPI.Swagger;

var builder = WebApplication.CreateBuilder(args);

// --- 1. AUTOFAC ENTEGRASYONU ---
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new AutofacBusinessModule());
});

builder.Services.AddControllers();

// --- 2. CORE KATMANI MODÜLLERİ ---
builder.Services.AddDependencyResolvers(new ICoreModule[] {
    new CoreModule()
});

// --- 3. JWT DOĞRULAMA AYARLARI (EKSİK OLAN KISIM BURASIYDI) ---
// appsettings.json dosyasındaki ayarları okuyoruz.
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>()
    ?? throw new InvalidOperationException("TokenOptions configuration is missing.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = tokenOptions.Issuer,
            ValidAudience = tokenOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
        };
    });
// ----------------------------------------------------------------
builder.Services.AddAuthorization();
builder.Services.AddSessionSentinel(options =>
{
    // Session-Sentinel ayarlarını uygulama config'inden alıyoruz.
    builder.Configuration.GetSection("SessionSentinel").Bind(options);
});
builder.Services.Configure<LoginChallengeOptions>(
    builder.Configuration.GetSection(LoginChallengeOptions.SectionName));
builder.Services.AddSingleton<ILoginChallengeStore, MemoryLoginChallengeStore>();
builder.Services.AddSingleton<ILoginChallengeEmailSender, LoginChallengeEmailSender>();
builder.Services.AddScoped<ILoginChallengeService, LoginChallengeService>();

builder.Services.AddEndpointsApiExplorer();

// --- 4. SWAGGER AYARLARI ---
builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\""
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
    opt.OperationFilter<FingerprintHeaderOperationFilter>();
});

var app = builder.Build();

// app.ConfigureCustomExceptionMiddleware(); 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// --- 5. KİMLİK DOĞRULAMA & YETKİLENDİRME ---
app.UseAuthentication();
app.UseSessionSentinel();
app.UseAuthorization();

app.MapControllers();
app.MapSessionSentinelHub();

app.Run();
