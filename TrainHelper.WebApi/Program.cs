using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TrainHelper.DAL.Providers;
using TrainHelper.WebApi;
using TrainHelper.WebApi.Config;
using TrainHelper.WebApi.Middleware;
using TrainHelper.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

//Configuration
var authSection = builder.Configuration.GetSection(AuthSettings.SectionName);
var authConfig = authSection.Get<AuthSettings>();
var appSection = builder.Configuration.GetSection(AppSettings.SectionName);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = "Fill token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IReportGeneratorService, ReportGeneratorService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITrainDataProvider, TrainDataProvider>();
builder.Services.AddScoped<IUserDataProvider, UserDataProvider>();
builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Services.AddDbContext<TrainHelper.DAL.DataContext>(options => options
        .UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql")));

builder.Services.Configure<AppSettings>(appSection);
builder.Services.Configure<AuthSettings>(authSection);

//Authentication setup
builder.Services.AddAuthentication(o => o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authConfig.Issuer,
            ValidateAudience = true,
            ValidAudience = authConfig.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = authConfig.GetSymmetricSecurityKey(),
            ClockSkew = TimeSpan.Zero
        };
    });

//Authorization setup
builder.Services.AddAuthorization(o => o.AddPolicy("ValidAccessToken", p =>
{
    p.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
    p.RequireAuthenticatedUser();
}));

var app = builder.Build();

//Migration
using var serviceScope = ((IApplicationBuilder)app)
    .ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
var context = serviceScope?.ServiceProvider.GetRequiredService<TrainHelper.DAL.DataContext>();
context?.Database.Migrate();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.UseTokenValidator();
app.MapControllers();

app.Run();