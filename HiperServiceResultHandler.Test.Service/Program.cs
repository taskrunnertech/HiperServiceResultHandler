using HiperServiceResultHandler;
using HiperServiceResultHandler.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();
app.ConfigureServiceExceptionHandler();

app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

app.MapGet("/getOk", async (HttpContext context) =>
{
    await DebugRequest(context.Request);

    return new ApiResultMessage<string>
    {
        Data = "Everything is fine",
        IsSuccessful = true,
    };
});

app.MapGet("/getFailure", async (HttpContext context) =>
{
    await DebugRequest(context.Request);

    throw new ServiceException("User message error", 1234, "USER_MESSAGE_CODE", "Internal message");
});

app.MapPost("/postFailure", async (HttpContext context, [FromBody] PostParameters parms) =>
{
    await DebugRequest(context.Request);

    if (string.IsNullOrEmpty(parms.ParamA))
    {
        throw new ServiceException("Parameter empty", 2345, "PARAM_INVALID", "There was an issue with parameter A");
    }
    if (!parms.ParamB)
    {
        throw new ServiceException("Parameter false", 2345, "PARAM_INVALID", "There was an issue with parameter B");
    }

    // No rest for the wicked
    throw new ServiceException("User message error", 1234, "USER_MESSAGE_CODE", "Internal message");
});

app.Run();

async Task DebugRequest(HttpRequest request)
{
    Console.WriteLine($"HTTP {request.Method} {request.Path}");
    foreach(var h in request.Headers)
    {
        Console.WriteLine($"{h.Key}: {h.Value}");
    }
    if(request.ContentLength.GetValueOrDefault(0) > 0)
    {
        request.Body.Position = 0;
        using var sr = new StreamReader(request.Body);
        var body = await sr.ReadToEndAsync();
        Console.WriteLine(body);
    }
    Console.WriteLine();
}

class PostParameters
{
    public string ParamA { get; set; }
    public bool ParamB { get; set; }
}