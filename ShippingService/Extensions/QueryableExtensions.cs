using ShippingService.Models;

namespace ShippingService.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TSource> Paginate<TSource>(this IQueryable<TSource> source, PagingOptions pagingOptions)
        => source.Paginate(pagingOptions.Page, pagingOptions.ItemsCountPerPage);

    public static IQueryable<TSource> Paginate<TSource>(this IQueryable<TSource> source, int page, int itemsCountPerPage)
    {
        return source.Skip(page * itemsCountPerPage)
                     .Take(itemsCountPerPage);
    }
}