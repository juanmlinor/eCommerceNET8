﻿using eCommerceNET8.SharedLibrary.Responses;
using System.Linq.Expressions;

namespace eCommerceNET8.SharedLibrary.Interface
{
    public interface IGenericInterface<T> where T:class
    {
        Task<Response> CreateAsync(T entity);
        Task<Response> UpdateAsync(T entity);
        Task<Response> DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> FindByIdAsync(int id);
        Task<T> GetByAsync(Expression<Func<T, bool>> predicate);

    }
}
