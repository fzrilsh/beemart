using BeeMart.WebClient.Models;
using BeeMart.WebClient.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpClient<ApiClient>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapPost("/api/proxy/invoices/lock", async (HttpContext context, ApiClient api, CancellationToken cancellationToken) =>
{
    var token = context.Request.Cookies[SessionCookie.Name];
    if (string.IsNullOrEmpty(token))
    {
        return Results.Unauthorized();
    }
    try
    {
        var result = await api.LockInvoiceAsync(token, cancellationToken);
        return Results.Ok(result);
    }
    catch (ApiException ex)
    {
        return ApiFailure(ex);
    }
});

app.MapPost("/api/proxy/transactions/calculate", async (HttpContext context, ApiClient api, CancellationToken cancellationToken) =>
{
    var token = context.Request.Cookies[SessionCookie.Name];
    if (string.IsNullOrEmpty(token))
    {
        return Results.Unauthorized();
    }
    try
    {
        var payload = await context.Request.ReadFromJsonAsync<CalculateProxyRequest>(cancellationToken: cancellationToken);
        var result = await api.CalculateAsync(payload!.Items, token, cancellationToken);
        return Results.Ok(result);
    }
    catch (ApiException ex)
    {
        return ApiFailure(ex);
    }
});

app.MapPost("/api/proxy/transactions", async (HttpContext context, ApiClient api, CancellationToken cancellationToken) =>
{
    var token = context.Request.Cookies[SessionCookie.Name];
    if (string.IsNullOrEmpty(token))
    {
        return Results.Unauthorized();
    }
    try
    {
        var payload = await context.Request.ReadFromJsonAsync<SubmitProxyRequest>(cancellationToken: cancellationToken);
        var result = await api.SubmitTransactionAsync(
            new SubmitTransactionRequest(payload!.InvoiceNo, payload.IdPaymentMethod, payload.Items),
            token,
            cancellationToken);
        return Results.Ok(result);
    }
    catch (ApiException ex)
    {
        return ApiFailure(ex);
    }
});

app.MapGet("/api/proxy/products/search", async (HttpContext context, ApiClient api, CancellationToken cancellationToken) =>
{
    var token = context.Request.Cookies[SessionCookie.Name];
    if (string.IsNullOrEmpty(token))
    {
        return Results.Unauthorized();
    }
    try
    {
        var keyword = context.Request.Query["keyword"].ToString();
        var result = await api.SearchProductsAsync(keyword, token, cancellationToken);
        return Results.Ok(result);
    }
    catch (ApiException ex)
    {
        return ApiFailure(ex);
    }
});

app.MapGet("/api/proxy/payment-methods", async (HttpContext context, ApiClient api, CancellationToken cancellationToken) =>
{
    var token = context.Request.Cookies[SessionCookie.Name];
    if (string.IsNullOrEmpty(token))
    {
        return Results.Unauthorized();
    }
    try
    {
        var result = await api.GetPaymentMethodsAsync(token, cancellationToken);
        return Results.Ok(result);
    }
    catch (ApiException ex)
    {
        return ApiFailure(ex);
    }
});

app.MapGet("/api/proxy/history", async (HttpContext context, ApiClient api, CancellationToken cancellationToken) =>
{
    var token = context.Request.Cookies[SessionCookie.Name];
    if (string.IsNullOrEmpty(token))
    {
        return Results.Unauthorized();
    }
    try
    {
        var date = context.Request.Query["date"].ToString();
        var page = int.TryParse(context.Request.Query["page"], out var parsed) ? parsed : 1;
        if (!DateTime.TryParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedDate))
        {
            return Results.BadRequest(new { code = "VALIDATION_ERROR", message = "Format tanggal tidak valid." });
        }
        var result = await api.GetHistoryAsync(parsedDate, page, token, cancellationToken);
        return Results.Ok(result);
    }
    catch (ApiException ex)
    {
        return ApiFailure(ex);
    }
});

app.MapRazorPages();

app.Run();

static IResult ApiFailure(ApiException ex)
{
    return Results.Json(new { code = ex.Code, message = ex.Message }, statusCode: ex.StatusCode);
}
