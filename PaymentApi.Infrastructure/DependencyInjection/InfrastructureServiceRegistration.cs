using Microsoft.Extensions.DependencyInjection;
using PaymentApi.Application.Interfaces;
using PaymentApi.Application.Interfaces.Repositories;
using PaymentApi.Application.Interfaces.Services;
using PaymentApi.Infrastructure.Persistence;
using PaymentApi.Infrastructure.Repositories;
using PaymentApi.Infrastructure.Services;

namespace PaymentApi.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            // Services
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IAppDbContext>(sp =>
                sp.GetRequiredService<AppDbContext>());

            return services;
        }
    }
}
