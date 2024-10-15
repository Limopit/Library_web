﻿namespace Library.Application.Interfaces.Repositories;

public interface IBaseRepository<T> where T: class
{
    Task<T?> GetEntityByIdAsync(Guid id, CancellationToken token);
    Task AddEntityAsync(T entity, CancellationToken token);
    Task RemoveEntity(T entity);
}