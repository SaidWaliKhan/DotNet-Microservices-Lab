using OrderService.Services.DTOs;

namespace OrderService.Services.HttpServices;

public class ProductServiceClient
{
    private readonly HttpClient _httpClient;

    public ProductServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    // we send a req to product for this id
    public async Task<ProductDto?> GetProductAsync(int productId)
    {
        var response = await _httpClient.GetAsync($"/api/products/{productId}");

        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<ProductDto>();
    }


    // the we send req to reduce the stock

    public async Task<bool> ReduceStockAsync(int productId, int quantity)
    {
        var response = await _httpClient.PutAsync(
            $"/api/products/{productId}/reduce-stock/{quantity}", null);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RestoreStockAsync(int productId, int quantity)
    {
        var response = await _httpClient.PutAsync(
            $"/api/products/{productId}/restore-stock/{quantity}", null);
        return response.IsSuccessStatusCode;
    }



}