using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BeeMart.WebClient.Models;

namespace BeeMart.WebClient.Services;

public class ApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _http;

    public ApiClient(HttpClient http, IConfiguration configuration)
    {
        _http = http;
        var baseUrl = configuration["Api:BaseUrl"] ?? "http://127.0.0.1:5099";
        _http.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
    }

    public Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken) =>
        PostAsync<LoginRequest, LoginResponse>("api/v1/auth/login", request, null, cancellationToken);

    public Task<JsonElement> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken) =>
        PostAsync<RegisterRequest, JsonElement>("api/v1/auth/register", request, null, cancellationToken);

    public Task<List<ProductDto>> SearchProductsAsync(string keyword, string token, CancellationToken cancellationToken) =>
        GetAsync<List<ProductDto>>($"api/v1/products/search?keyword={Uri.EscapeDataString(keyword)}&limit=10", token, cancellationToken);

    public Task<List<PaymentMethodDto>> GetPaymentMethodsAsync(string token, CancellationToken cancellationToken) =>
        GetAsync<List<PaymentMethodDto>>("api/v1/payment-methods", token, cancellationToken);

    public Task<InvoiceLockResponse> LockInvoiceAsync(string token, CancellationToken cancellationToken) =>
        PostAsync<object, InvoiceLockResponse>("api/v1/invoices/lock", new { }, token, cancellationToken);

    public Task<CalculateResponse> CalculateAsync(List<CartItemRequest> items, string token, CancellationToken cancellationToken) =>
        PostAsync<CalculateRequest, CalculateResponse>("api/v1/transactions/calculate", new CalculateRequest(items), token, cancellationToken);

    public Task<SubmitTransactionResponse> SubmitTransactionAsync(SubmitTransactionRequest request, string token, CancellationToken cancellationToken) =>
        PostAsync<SubmitTransactionRequest, SubmitTransactionResponse>("api/v1/transactions", request, token, cancellationToken);

    public Task<HistoryResponse> GetHistoryAsync(DateTime date, int page, string token, CancellationToken cancellationToken) =>
        GetAsync<HistoryResponse>($"api/v1/history?date={date:yyyy-MM-dd}&page={page}", token, cancellationToken);

    private async Task<TResponse> GetAsync<TResponse>(string url, string token, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        AddToken(request, token);
        using var response = await _http.SendAsync(request, cancellationToken);
        return await ReadAsync<TResponse>(response);
    }

    private async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest body, string? token, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(body, options: JsonOptions)
        };
        AddToken(request, token);
        using var response = await _http.SendAsync(request, cancellationToken);
        return await ReadAsync<TResponse>(response);
    }

    private static void AddToken(HttpRequestMessage request, string? token)
    {
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private static async Task<TResponse> ReadAsync<TResponse>(HttpResponseMessage response)
    {
        var payload = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            var success = JsonSerializer.Deserialize<TResponse>(payload, JsonOptions);
            return success!;
        }

        var code = "INTERNAL_ERROR";
        var message = "Terjadi kesalahan pada server.";
        try
        {
            using var doc = JsonDocument.Parse(payload);
            if (doc.RootElement.TryGetProperty("code", out var codeElement))
            {
                code = codeElement.GetString() ?? code;
            }
            if (doc.RootElement.TryGetProperty("message", out var messageElement))
            {
                message = messageElement.GetString() ?? message;
            }
        }
        catch (JsonException)
        {
        }

        throw new ApiException(code, message, (int)response.StatusCode);
    }
}
