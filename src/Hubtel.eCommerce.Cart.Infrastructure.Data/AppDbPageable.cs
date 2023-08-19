using Hubtel.eCommerce.Cart.Core.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Hubtel.eCommerce.Cart.Infrastructure.Data
{
    public class AppDbPageable<T> : IPageable<T>
    {
        public AppDbPageable(int pageNumber, int pageSize, long totalItems, IEnumerable<T> items)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalItems = totalItems;
            Items = items;
        }

        public int PageNumber { get; }
        public int PageSize { get; }
        public long TotalItems { get; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public IEnumerable<T> Items { get; }
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPrevPage => PageNumber > 1;
    }

    public static class PageableExtensions
    {
        public static AppDbPageable<T> Paginate<T>(
            this IEnumerable<T> source,
            int pageNumber,
            int pageSize)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Page number must be greater than or equal to 1.");

            if (pageSize < 1)
                throw new ArgumentException("Page size must be greater than or equal to 1.");

            long totalItems = source.LongCount();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new AppDbPageable<T>(pageNumber, pageSize, totalItems, items);
        }

        public static async Task<AppDbPageable<T>> PaginateAsync<T>(
            this IQueryable<T> source,
            int pageNumber,
            int pageSize)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Page number must be greater than or equal to 1.");

            if (pageSize < 1)
                throw new ArgumentException("Page size must be greater than or equal to 1.");

            long totalItems = await source.LongCountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new AppDbPageable<T>(pageNumber, pageSize, totalItems, items);
        }

        public static AppDbPageable<TResult> Paginate<TSource, TResult>(
            this IEnumerable<TSource> source,
            int pageNumber,
            int pageSize,
            Func<TSource, TResult> selector)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Page number must be greater than or equal to 1.");

            if (pageSize < 1)
                throw new ArgumentException("Page size must be greater than or equal to 1.");

            long totalItems = source.LongCount();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(selector).ToList();

            return new AppDbPageable<TResult>(pageNumber, pageSize, totalItems, items);
        }

        public static async Task<AppDbPageable<TResult>> PaginateAsync<TSource, TResult>(
            this IQueryable<TSource> source,
            int pageNumber,
            int pageSize,
            Func<TSource, TResult> selector)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Page number must be greater than or equal to 1.");

            if (pageSize < 1)
                throw new ArgumentException("Page size must be greater than or equal to 1.");

            long totalItems = await source.LongCountAsync();
            var items = (await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync()).Select(selector).ToList();

            return new AppDbPageable<TResult>(pageNumber, pageSize, totalItems, items);
        }
    }
}
