using Demo.Security.Domain.Users;
using Demo.Security.Infrastructure.Abstractions;
using Demo.Security.Infrastructure.Persistence;
using Demo.Security.Infrastructure.Persistence.Repositories;
using Demo.Security.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Demo.Security.Domain.Abstractions;
using Demo.Security.Infrastructure;
using Demo.Security.Infrastructure.Email;

namespace Demo.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(cfg.GetConnectionString("DefaultConnection")));

            services.AddScoped<IRepository<User, Guid>, UserRepository>();
            services.AddScoped<UserRepository>(); // para métodos específicos
            services.AddScoped<IRepository<Role, Guid>, RepositoryRole>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddScoped<PasswordResetRepository>();

            // Bind de opciones SMTP desde appsettings
            services.Configure<SmtpOptions>(cfg.GetSection("Smtp"));

            // Email sender productivo
            services.AddSingleton<IEmailSender, NoopEmailSender>(); // cambia por SMTP en prod
            //services.AddSingleton<IEmailSender, SmtpEmailSender>();

            return services;
        }
    }
}
