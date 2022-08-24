using DesafioDtiBack;
using DesafioDtiBack.Models;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
var app = builder.Build();

app.UseCors(policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin();
        policyBuilder.AllowAnyHeader();
        policyBuilder.AllowAnyMethod();
    }
);

app.MapPost("/api/loan", async (context) =>
{
    // read the request body as json
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    var loan = JsonConvert.DeserializeObject<LoanRequest>(body);

    if (loan == null)
    {
        context.Response.StatusCode = 400;
        return;
    }
    
    var success = Rules.CheckRequest(loan, out var message);
    if (!success) context.Response.StatusCode = 400;
    await context.Response.WriteAsync(message);
});
app.Run();