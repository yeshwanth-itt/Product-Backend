using Microsoft.EntityFrameworkCore;
using Product.Backend.Application.Contracts.Persistance;
using Product.Backend.Application.Contracts.Services;
using Product.Backend.Application.MappingProfiles;
using Product.Backend.Application.Services;
using Product.Backend.Infrastructure;
using Product.Backend.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(ProductMapping));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
