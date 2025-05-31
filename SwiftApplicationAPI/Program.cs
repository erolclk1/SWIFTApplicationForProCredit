using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SQLitePCL;
using SwiftApplicationAPI.Data;
using System;
using SwiftApplicationAPI;
using SwiftApplicationAPI.Services;
using Microsoft.OpenApi.Models;
using SwiftApplicationAPI.SignalR;

var builder = WebApplication.CreateBuilder(args);

//Initializing SQLite
Batteries.Init();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJI...\"",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddWebServices(builder.Configuration);
Log.Information("The application is starting");


var app = builder.Build();
app.UseRouting();
//Push Notifications for the MT799 STATUS MESSAGE
app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotificationHub>("/notificationHub");
});

//Creating Database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SWIFTMessagesDataContext>();
    await context.Init();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
