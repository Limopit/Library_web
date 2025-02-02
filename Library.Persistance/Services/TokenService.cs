﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Library.Application.Interfaces;
using Library.Application.Interfaces.Services;
using Library.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Library.Persistance.Services;

public class TokenService: ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILibraryDBContext _libraryDbContext;
    
    public TokenService(IConfiguration configuration, ILibraryDBContext libraryDbContext)
    {
        _configuration = configuration;
        _libraryDbContext = libraryDbContext;
    }

    public async Task<(string accessToken, string refreshToken)> GenerateTokens(User user,
        UserManager<User> userManager, CancellationToken token)
    {
        var accessToken = await GenerateAccessToken(user, userManager);
        var refreshToken = await GenerateRefreshToken(user, token);

        return (accessToken, refreshToken);
    }

    public async Task<string> GenerateNewToken(User user, UserManager<User> userManager)
    {
        return await GenerateAccessToken(user, userManager);
    }

    private async Task<string> GenerateAccessToken(User user, UserManager<User> userManager)
    {
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        
        if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        {
            throw new InvalidOperationException("JWT settings are not configured properly.");
        }
        
        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> GenerateRefreshToken(User user, CancellationToken cancellationToken)
    {
        var randomBytes = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            Expires = DateTime.UtcNow.AddDays(1),
            Created = DateTime.UtcNow,
            UserId = user.Id
        };

        await _libraryDbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _libraryDbContext.SaveChangesAsync(cancellationToken);

        return refreshToken.Token;
    }
}