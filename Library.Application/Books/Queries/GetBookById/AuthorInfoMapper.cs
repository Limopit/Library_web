﻿using AutoMapper;
using Library.Application.Common.Mappings;
using Library.Domain;

namespace Library.Application.Books.Queries.GetBookById;

public class AuthorInfoMapper: IMapWith<Author>
{
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Author, AuthorInfoDto>()
            .ForMember(dto => dto.AuthorFirstname, opt => opt.MapFrom(auth => auth.AuthorFirstname))
            .ForMember(dto => dto.AuthorLastname, opt => opt.MapFrom(auth => auth.AuthorLastname));
    }
}