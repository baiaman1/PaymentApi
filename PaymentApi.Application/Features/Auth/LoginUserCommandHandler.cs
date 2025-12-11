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
        private readonly IJwtTokenService _jwt;

        private const int MaxAttempts = 3;
        private static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(5);

        public LoginCommandHandler(
            IUserRepository users,
            ISessionRepository sessions,
            IJwtTokenService jwt)
        {
            _users = users;
            _sessions = sessions;
            _jwt = jwt;
        }

        public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _users.GetByLoginAsync(request.Login);

            HttpException.ThrowIf(user == null, HttpStatusCode.Unauthorized, "Invalid login or password");

            if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.UtcNow)
                HttpException.Throw(HttpStatusCode.Unauthorized, $"Account locked until {user.LockedUntil:u}");

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

            var token = _jwt.GenerateToken(user.Id);

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
