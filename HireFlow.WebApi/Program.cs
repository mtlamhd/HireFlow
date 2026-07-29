using System.Text;
using HireFlow.Business;
using HireFlow.Business.Authentications;
using HireFlow.Domain.Entities;
using HireFlow.Infrustructure;
using HireFlow.Infrustructure.Data;
using HireFlow.Infrustructure.EmailServices;
using HireFlow.WebApi.Extentions;
using HireFlow.WebApi.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services
    .AddIdentity<User, Role>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;

        options.User.RequireUniqueEmail = false;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("JwtConfigurations").Get<JwtSettings>()!;

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtConfigurations"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ClockSkew = TimeSpan.Zero,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ValidAudience = jwtSettings.Audience,
        ValidateAudience = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateIssuer = true
    };
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "HireFlow API", 
        Version = "v1" 
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below."
    });

    options.AddSecurityRequirement(document =>
    {
        var securitySchemeRef = new OpenApiSecuritySchemeReference("Bearer", document);
        return new OpenApiSecurityRequirement
        {
            [securitySchemeRef] = new List<string>()
        };
    });
});


var app = builder.Build();

await app.SeedDataBaseAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();