using Autofac;
using Autofac.Extensions.DependencyInjection;
using Business.DependencyResolvers.Autofac;
using Core.DependencyResolvers;
using Core.Extensions.DependencyInjection;
using Core.Utilities.IoC;
using Core.Utilities.Security.Encryption; // SecurityKeyHelper için gerekli
using Core.Utilities.Security.JWT; // TokenOptions için gerekli
using Microsoft.AspNetCore.Authentication.JwtBearer; // Bu namespace þart
using Microsoft.IdentityModel.Tokens; // TokenValidationParameters için þart
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- 1. AUTOFAC ENTEGRASYONU ---
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new AutofacBusinessModule());
});

builder.Services.AddControllers();

// --- 2. CORE KATMANI MODÜLLERÝ ---
builder.Services.AddDependencyResolvers(new ICoreModule[] {
    new CoreModule()
});

// --- 3. JWT DOÐRULAMA AYARLARI (EKSÝK OLAN KISIM BURASIYDI) ---
// appsettings.json dosyasýndaki ayarlarý okuyoruz
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();

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

// --- 5. KÝMLÝK DOÐRULAMA & YETKÝLENDÝRME ---
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();