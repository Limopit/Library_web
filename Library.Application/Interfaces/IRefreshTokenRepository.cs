﻿using Library.Domain;

namespace Library.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task SaveRefreshToken(RefreshToken token, CancellationToken cancellationToken);
    string ValidateRefreshToken(string token);
    void RevokeToken(string refreshToken);
}