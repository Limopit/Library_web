﻿using Library.Application.Common.Exceptions;
using Library.Application.Interfaces;
using Library.Application.Interfaces.Services;
using Library.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Library.Application.Users.Commands.LoginUser;

public class LoginUserCommandHandler: IRequestHandler<LoginUserCommand, (string,string)>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;

    public LoginUserCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService, UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _userManager = userManager;
    }

    public async Task<(string, string)> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.FindUserByEmail(request.Email);
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.Email);
        }

        if (!isPasswordValid)
        {
            throw new Exception("Password is invalid");
        }

        return await _tokenService.GenerateTokens(user, _userManager, cancellationToken);
    }
}