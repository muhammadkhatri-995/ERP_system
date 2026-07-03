//using ERP_sys.Data; // Ensure this matches the namespace inside your SqlConnectionFactory.cs

using ERP_sys.Data;
using ERP_sys.Repositories;


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






// Swagger/OpenAPI Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

app.MapControllers();

app.Run();