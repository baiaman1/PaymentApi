using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentApi.Application.Common;
using PaymentApi.Application.Common.Exceptions;
using PaymentApi.Application.Interfaces;
using PaymentApi.Domain.Entities;
using System.Net;

namespace PaymentApi.Application.Features.Users
{
    public class RegisterUserCommandHandler
        : IRequestHandler<RegisterUserCommand, string>
    {
        private readonly IAppDbContext _db;

        public RegisterUserCommandHandler(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var exists = await _db.Users
                .AnyAsync(x => x.Login == request.UserName, cancellationToken);

            HttpException.ThrowIf(exists, HttpStatusCode.BadRequest, "User with this login already exists.");
            
            string passwordHash = PasswordHasher.Hash(request.Password);

            var user = new User
            {
                Login = request.UserName,
                PasswordHash = passwordHash,
                Balance = 8m
            };

            await _db.Users.AddAsync(user, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            return "User successfully registered.";
        }
    }
}
