using BCrypt.Net;
using MediatR;
using PaymentApi.Application.Common;
using PaymentApi.Application.Common.Exceptions;
using PaymentApi.Application.Interfaces.Repositories;
using PaymentApi.Application.Interfaces.Services;
using PaymentApi.Domain.Entities;
using System.Net;

namespace PaymentApi.Application.Features.Auth
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        private readonly IUserRepository _users;
        private readonly ISessionRepository _sessions;
        private readonly ITokenService _tokenService;

        private const int MaxAttempts = 3;
        private static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(5);

        public LoginCommandHandler(IUserRepository users, ISessionRepository sessions, ITokenService tokenService)
        {
            _users = users;
            _sessions = sessions;
            _tokenService = tokenService;
        }

        public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _users.GetByLoginAsync(request.Login);

            HttpException.ThrowIf(user == null, HttpStatusCode.Unauthorized, "Invalid login or password", "LoginError");

            HttpException.ThrowIf(user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.UtcNow, 
                                  HttpStatusCode.Unauthorized, $"Account locked until {user.LockedUntil.Value:u}", "LoginError");

            var passwordOk = PasswordHasher.Hash(request.Password) == user.PasswordHash;
            if (!passwordOk)
            {
                user.FailedAttempts++;
                if (user.FailedAttempts >= MaxAttempts)
                    user.LockedUntil = DateTime.UtcNow.Add(LockDuration);

                await _users.UpdateAsync(user);
                HttpException.Throw(HttpStatusCode.Unauthorized, "Invalid login or password");
            }

            user.FailedAttempts = 0;
            user.LockedUntil = null;
            await _users.UpdateAsync(user);

            var token = _tokenService.GenerateToken();

            await _sessions.AddAsync(new Session
            {
                UserId = user.Id,
                Token = token,
                CreatedAt = DateTime.UtcNow
            });

            return new LoginResult { Token = token };
        }

    }
}
