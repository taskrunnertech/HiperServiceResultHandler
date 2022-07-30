using HiperServiceResultHandler;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();
app.ConfigurePublicServiceExceptionHandler(new PublicExceptionMiddleware.Options { DevelopmentMode = false });

app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

app.MapGet("/getOk", async (HttpContext context) =>
{
    await DebugRequest(context.Request);

    var http = new HttpClient();
    var s = await http.GetAsync<string>("http://localhost:5046/getOk");

    return s;
});

app.MapGet("/getFailure", async (HttpContext context) =>
{
    await DebugRequest(context.Request);

    var http = new HttpClient();
    var s = await http.GetAsync<string>("http://localhost:5046/getFailure");

    // This will fail and generate an ApiError response

    return s;
});

app.Run();

async Task DebugRequest(HttpRequest request)
{
    Console.WriteLine($"HTTP {request.Method} {request.Path}");
    foreach (var h in request.Headers)
    {
        Console.WriteLine($"{h.Key}: {h.Value}");
    }
    if (request.ContentLength.GetValueOrDefault(0) > 0)
    {
        request.Body.Position = 0;
        using var sr = new StreamReader(request.Body);
        var body = await sr.ReadToEndAsync();
        Console.WriteLine(body);
    }
    Console.WriteLine();
}
