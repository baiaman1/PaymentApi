using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PaymentApi.Application.Interfaces;
using System;
using System.Reflection;

namespace PaymentApi.Application.Extensions
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
