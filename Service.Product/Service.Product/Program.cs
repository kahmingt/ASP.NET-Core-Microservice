using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Service.Product.Work.Utility;
using Service.Product.Shared.Database;
using Service.Product.Shared.Database.Entity;
using Service.Product.Shared.Repository;


var builder = WebApplication.CreateBuilder(args);


#region --- SQL Connection ---
var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")
);
sqlConnectionStringBuilder.UserID = builder.Configuration["Database:User"];
sqlConnectionStringBuilder.Password = builder.Configuration["Database:Password"];

builder.Services
    .AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(sqlConnectionStringBuilder.ConnectionString,
            opt => opt.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: System.TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null)
        );
    });
#endregion

#region --- Services ---
// Repository Wrapper
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

//AutoMapper.Extensions.Microsoft.DependencyInjection
builder.Services.AddAutoMapper(typeof(Program));

// Operation helper
builder.Services.AddTransient<OperationHelper<dbProduct>>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

builder.Services.AddControllers();

#region --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("WebApiCorsPolicy",
        policy =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.WithOrigins("*");
        });
});
#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("WebApiCorsPolicy");

app.MapControllers();

app.Run();