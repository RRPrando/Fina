using Fina.Api.Data;
using Fina.Core.Handlers;
using Fina.Core.Models;
using Fina.Core.Requests.Categories;
using Fina.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Fina.Api.Handlers;

public class CategoryHandler : ICategoryHandler
{
    private readonly AppDbContext _context;

    public CategoryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Response<Category?>> CreateAsync(CreateCategoryRequest request)
    {
        
        var category = new Category
        {
            UserId = request.UserId,
            Title = request.Title,
            Description = request.Description
        };

        try
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return new Response<Category?>(category, 201, "Categoria criada com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Category?>(null, 500, "Não foi possível criar a Categoria");
        }
    }

    public async Task<Response<Category?>> UpdateAsync(UpdateCategoryRequest request)
    {
        try
        {
            var category = await _context
                .Categories
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (category == null)
            {
                return new Response<Category?>(null, 404, "Categoria não encontrada");
            }

            category.Title = request.Title;
            category.Description = request.Description;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return new Response<Category?>(category, 200, "Categoria atualizada com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Category?>(null, 500, "Não foi possível atualizar a Categoria");
        }
    }

    public async Task<Response<Category?>> DeleteAsync(DeleteCategoryRequest request)
    {
        try
        {
            var category = await _context
                .Categories
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (category == null)
            {
                return new Response<Category?>(null, 404, "Categoria não encontrada");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return new Response<Category?>(category, 200, "Categoria excluída com sucesso");
        }
        catch (Exception ex)
        {
            return new Response<Category?>(null, 500, "Não foi possível excluir a Categoria");
        }
    }

    public async Task<Response<Category?>> GetByIdAsync(GetCategoryByIdRequest request)
    {
        try
        {
            var category = await _context
                .Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            return category is null
                ? new Response<Category?>(null, 404, "Categoria não encontrada")
                : new Response<Category?>(category);
        }
        catch (Exception ex)
        {
            return new Response<Category?>(null, 500, "Não foi possível recuperar a Categoria");
        }
    }

    public async Task<PagedResponse<List<Category>?>> GetAllAsync(GetAllCategoriesRequest request)
    {
        try
        {
            var query = _context
                .Categories
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId)
                .OrderBy(x => x.Title);

            var categories = await query
                .Skip(request.PageSize * (request.PageNumber - 1))
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            return new PagedResponse<List<Category>?>(categories, count, request.PageNumber, request.PageSize);
        }
        catch (Exception ex)
        {
            return new PagedResponse<List<Category>?>(null, 500, "Não foi possível recuperar as Categorias");
        }
    }
}
