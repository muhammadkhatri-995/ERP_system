////using ERP_sys.Data; // Ensure this matches the namespace inside your SqlConnectionFactory.cs

//using ERP_sys.Data;
//using ERP_sys.Repositories;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using ERP_sys.Services;
//using ERP_sys.Repositories;


//var builder = WebApplication.CreateBuilder(args);

//// 1. Register Services to the Container
//builder.Services.AddControllers();

//// Register your raw ADO.NET Connection Factory here
//builder.Services.AddSingleton<SqlConnectionFactory>();


//// REGISTER YOUR REPOSITORY HERE 
//builder.Services.AddScoped<ERP_sys.Repositories.UserRepository>();
//builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
//builder.Services.AddScoped<IProductRepository, ProductRepository>();
//builder.Services.AddScoped<ISalesLedgerRepository, SalesLedgerRepository>();
//builder.Services.AddScoped<ICustomerLedgerRepository, CustomerLedgerRepository>();







//// Swagger/OpenAPI Configuration
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//// JWT Authentication Configuration
//var jwtSettings = builder.Configuration.GetSection("Jwt");
//var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = jwtSettings["Issuer"],
//        ValidAudience = jwtSettings["Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(key)
//    };
//});

//builder.Services.AddAuthorization();
//builder.Services.AddScoped<IJwtService, JwtService>();
//builder.Services.AddScoped<IAuthRepository, AuthRepository>();

//var app = builder.Build();

//// 2. Configure the HTTP Request Pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//// Enables serving static HTML files from your wwwroot folder
//app.UseStaticFiles();
//app.UseDefaultFiles(new DefaultFilesOptions
//{
//    DefaultFileNames = new List<string> { "users/login.html" }
//});

//app.UseStaticFiles();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();

using ERP_sys.Data;
using ERP_sys.Repositories;
using ERP_sys.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ERP_sys.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Register Services to the Container
builder.Services.AddControllers();

// Register your raw ADO.NET Connection Factory here
builder.Services.AddSingleton<SqlConnectionFactory>();

// REGISTER YOUR REPOSITORY HERE 
builder.Services.AddScoped<ERP_sys.Repositories.UserRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISalesLedgerRepository, SalesLedgerRepository>();
builder.Services.AddScoped<ICustomerLedgerRepository, CustomerLedgerRepository>();

// Swagger/OpenAPI Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Authentication Configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

var app = builder.Build();

// 2. Configure the HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enables serving static HTML files from your wwwroot folder
app.UseStaticFiles();
app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "users/login.html" }
});

app.UseStaticFiles();

app.UseAuthentication();   // ← was missing entirely — must come before UseAuthorization
app.UseAuthorization();

app.UseMiddleware<AuditLogMiddleware>();   // ← must come after UseAuthorization, before MapControllers

app.MapControllers();

app.Run();