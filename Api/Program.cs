using Api.Services;
using Application.Common.Interfaces.General;
using Application.Validators.Tenant;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

using InfrastructureUnitOfWork = Infrastructure.Repositories.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<NotificationBackgroundService>();
// Add services to the container.
builder.Services.AddControllers();

// Configure CORS: permissive in Development, restricted in Production
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });
}
else
{
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });
}

// Swagger (development only)
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Sakani API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the token here"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddHttpContextAccessor();


builder.Services.Scan(scan => scan
    .FromAssemblies(
        typeof(Application.Common.Interfaces.User.ICurrentUserService).Assembly,
        typeof(Infrastructure.ApplicationDbContext).Assembly,
        System.Reflection.Assembly.GetExecutingAssembly()
    )
    .AddClasses(classes => classes
        .Where(type => type.Name.EndsWith("Service")
               && !typeof(Microsoft.Extensions.Hosting.IHostedService).IsAssignableFrom(type)))
        .AsImplementedInterfaces()
        .WithScopedLifetime()

    .AddClasses(classes => classes
        .Where(type => type.Name.EndsWith("Service")
               && !typeof(Microsoft.Extensions.Hosting.IHostedService).IsAssignableFrom(type)))
        .AsSelf()
        .WithScopedLifetime()

    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention());



var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Missing configuration: Jwt:Key");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true
        };
    });

builder.Services.AddSingleton<IAuthorizationHandler, TenantOwnershipHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TenantAccess", policy =>
        policy.Requirements.Add(new TenantOwnershipRequirement()));
});
builder.Services.AddValidatorsFromAssemblyContaining<CreateTenantDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IUnitOfWork, InfrastructureUnitOfWork>();


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    try
    {
        await context.Database.MigrateAsync();

        // حقن البيانات الأساسية يعمل في جميع البيئات
        await DbInitializer.SeedSystemEssentialsAsync(context);

        // حقن البيانات الوهمية مقتصر على بيئة التطوير
        if (app.Environment.IsDevelopment())
        {
            await DbInitializer.SeedDummyDataAsync(context);
            Console.WriteLine("Seed dummy data injected (Development only).");
        }

        Console.WriteLine("Database is ready and tables are up to date.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Migration or Seeding failed");
    }
}
// If behind a reverse proxy or load balancer, forward X-Forwarded-* headers first
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Enforce HSTS in production
    app.UseHsts();
}


// Ensure HTTPS redirect runs after forwarded headers and before auth/CORS
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
// Apply CORS (uses the default policy configured above)
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
