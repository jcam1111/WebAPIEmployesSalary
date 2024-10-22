using WebAPIEmployesSalary.Controllers;
using WebAPIEmployesSalary.Model.Exception;
using WebAPIEmployesSalary.Services;
using WebAPIEmployesSalary.Services.Interfaces;
//using Microsoft.EntityFrameworkCore;
//using Swashbuckle.Swagger;

//var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpClient();

builder.Services.AddScoped<EmployeeApiService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAngularOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
                            .AllowAnyMethod();
            //.WithMethods("PUT", "DELETE", "GET","POST");
        });
});

//*********************** Add services to the container.***********************
//builder.Services.AddSingleton<IEmployController, EmployService>();
builder.Services.AddScoped<IEmployController, EmployService>();
//*********************** Add services to the container end.***********************


builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}
//app.UseExceptionHandler();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseRouting();

// Configure the HTTP request pipeline.
app.UseCors("AllowAngularOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllerRoute(name: "default",
               pattern: "api/{controller}/{id?}");

app.MapControllers();

app.Run();

