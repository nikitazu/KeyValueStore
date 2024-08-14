using Microsoft.AspNetCore.Mvc;
using KeyValueStore.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/keys", Handlers.GetKeys).WithOpenApi();
app.MapGet("/api/value/{key}", Handlers.GetValue).WithOpenApi();
app.MapPut("/api/value/{key}", Handlers.SetValue).WithOpenApi();
app.MapDelete("/api/value/{key}", Handlers.RemoveValue).WithOpenApi();

app.Run();

internal static class Handlers
{
    public static IResult GetKeys() => Results.Ok(Store.GetKeys());

    public static IResult GetValue(string key) =>
        Store.GetValue(key, out string? value)
            ? Results.Ok(value!)
            : Results.NotFound();

    public static IResult SetValue(string key, [FromBody] string value)
    {
        Store.SetValue(key, value);
        return Results.NoContent();
    }

    public static IResult RemoveValue(string key)
    {
        Store.RemoveValue(key);
        return Results.NoContent();
    }
}
