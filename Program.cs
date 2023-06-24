
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using TodoApi;
using Newtonsoft.Json;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IToDoDbContext, ToDoDbContext>();
builder.Services.AddScoped<ItemService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"),
    new MySqlServerVersion(new Version(8, 0, 26)),
    b => b.MigrationsAssembly(typeof(ToDoDbContext).Assembly.FullName)
));
//לבדוק האם ההוספה של הקורס שעשעיתי מספיקה..
builder.Services.AddCors(x => x.AddPolicy("policy", options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseSwagger();
// app.UseHttpsRedirection();
app.UseCors("policy");

app.MapGet("/", (Func<string>)(() => "Hello World!"));

app.MapGet("/items/getAll", async (ItemService itemService) =>
{
    return await itemService.GetAllItemsAsync();
});


//בהנחה שכשמוסיפים משימה היא לא הושלמה!
app.MapPost("/items/add/{name}", async (string name, ItemService itemService) =>
{
    Item item = new Item
    {
        Name = name,
        IsComplete = "no"
    };
    await itemService.AddItemAsync(item);
    return Results.Created($"/items/{item.Id}", item);
});

//לשנות את הערך של השלמת המשימה להיפך של הערך הקודםrrrr

app.MapPut("/items/update", async (HttpContext context, ItemService itemService) =>
{
    string requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
    dynamic x = JsonConvert.DeserializeObject(requestBody);

    Item item = new Item
    {
        Id = x.id,
        IsComplete = x.isComplete,
        Name = x.name
    };

    await itemService.UpdateItemAsync(item);
    return Results.NoContent();
});

app.MapDelete("/items/delete/{id}", async (int id, ItemService itemService) =>
{
    await itemService.DeleteItemAsync(id);
    return Results.NoContent();
});
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo Api v1");
    c.RoutePrefix = string.Empty;
});
app.Run();