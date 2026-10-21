using CommerceApp.Contracts.Carts;
using CommerceApp.Contracts.Products;
using CommerceApp.Data;
using CommerceApp.Services.Products;
using Microsoft.EntityFrameworkCore;
using CommerceApp.Services.Carts;
using CommerceApp.Contracts.Orders;
using CommerceApp.Services.Orders;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//DbContext configuration
builder.Services.AddDbContext<ApplicationDbContext, ApplicationDbContext>(options => options.UseSqlite(builder
    .Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrdersService, OrderService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .WithHeaders("Content-Type")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
