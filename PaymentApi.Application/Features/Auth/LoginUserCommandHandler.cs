using BCrypt.Net;
using MediatR;
using PaymentApi.Application.Interfaces.Repositories;
using PaymentApi.Application.Interfaces.Services;
using PaymentApi.Domain.Entities;

namespace PaymentApi.Application.Features.Auth
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        private readonly IUserRepository _users;
        private readonly ISessionRepository _sessions;
        private readonly ITokenService _tokenService;

        private const int MaxAttempts = 5;
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
            if (user == null)
                throw new UnauthorizedAccessException("Invalid login or password");

            if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.UtcNow)
                throw new UnauthorizedAccessException($"Account locked until {user.LockedUntil.Value:u}");

            // Проверяем пароль (BCrypt) — реализация хеша в Infrastructure, здесь предполагаем уже хеш сравнивается
            var passwordOk = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!passwordOk)
            {
                user.FailedAttempts++;
                if (user.FailedAttempts >= MaxAttempts)
                    user.LockedUntil = DateTime.UtcNow.Add(LockDuration);

                await _users.UpdateAsync(user);
                throw new UnauthorizedAccessException("Invalid login or password");
            }

            // Успешный логин
            user.FailedAttempts = 0;
            user.LockedUntil = null;
            await _users.UpdateAsync(user);

            var token = _tokenService.GenerateToken();

            var session = new Session
            {
                UserId = user.Id,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            await _sessions.AddAsync(session);

            return new LoginResult { Token = token };
        }
    }
}
