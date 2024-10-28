﻿namespace Library.Application.Common.Mappings;

public interface IMapperService
{
    Task<TDestination> Map<TSource, TDestination>(TSource source);
}