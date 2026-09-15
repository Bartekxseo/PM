using Microsoft.Extensions.DependencyInjection;

namespace PM.Services
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddPMServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
            return services;
        }
    }
}
