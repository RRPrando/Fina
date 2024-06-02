using Fina.Core.Handlers;
using Fina.Core.Models;
using Fina.Core.Requests.Categories;
using Fina.Core.Responses;
using System.Net.Http.Json;

namespace Fina.Web.Handlers;

public class CategoryHandler : ICategoryHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;

    public CategoryHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient(WebConfiguration.HttpClientName);
    }

    public async Task<Response<Category?>> CreateAsync(CreateCategoryRequest request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/categories", request);
        return await result.Content.ReadFromJsonAsync<Response<Category?>>()
            ?? new Response<Category?>(null, 400, "Falha ao criar categoria");
    }

    public async Task<Response<Category?>> UpdateAsync(UpdateCategoryRequest request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/categories/{request.Id}", request);
        return await result.Content.ReadFromJsonAsync<Response<Category?>>()
            ?? new Response<Category?>(null, 400, "Falha ao atualizar categoria");
    }

    public async Task<Response<Category?>> DeleteAsync(DeleteCategoryRequest request)
    {
        var result = await _httpClient.DeleteAsync($"v1/categories/{request.Id}");
        return await result.Content.ReadFromJsonAsync<Response<Category?>>()
            ?? new Response<Category?>(null, 400, "Falha ao excluir categoria");
    }

    public async Task<PagedResponse<List<Category>?>> GetAllAsync(GetAllCategoriesRequest request)
    {
        return await _httpClient.GetFromJsonAsync<PagedResponse<List<Category>?>>($"v1/categories/")
            ?? new PagedResponse<List<Category>?>(null, 400, "Não foi possível obter categorias");
    }

    public async Task<Response<Category?>> GetByIdAsync(GetCategoryByIdRequest request)
    {
        return await _httpClient.GetFromJsonAsync<Response<Category?>>($"v1/categories/{request.Id}")
            ?? new Response<Category?>(null, 400, "Não foi possível obter categoria");
    }
}
