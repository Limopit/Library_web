﻿using FluentValidation;

namespace Library.Application.Books.Commands.UpdateBook;

public class UpdateBookCommandValidator: AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(command => command.book_id).NotEmpty().WithMessage("Id is required");
        RuleFor(command => command.ISBN)
            .NotEmpty().WithMessage("ISBN is required")
            .Length(13,17).WithMessage("ISBN must be 13-17 char");
        RuleFor(command => command.book_name).NotEmpty()
            .WithMessage("Name is required").MaximumLength(64);
        RuleFor(command => command.book_genre).MaximumLength(32);
        RuleFor(command => command.book_description).MaximumLength(256);
        RuleFor(command => command.author_id).NotEmpty().WithMessage("Author is required");
    }
}