using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace yuyu.Infrastructure
{
    public class AutofacExt
    {
        private static IContainer _container;
        public static IContainer InitAutofac(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            
            services.AddScoped(typeof(IUnitWork), typeof(UnitWork));

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly());
            builder.RegisterAssemblyTypes();
            
            if (services.All(u => u.ServiceType != typeof(IHttpContextAccessor)))
            {
                services.AddScoped(typeof(IHttpContextAccessor), typeof(HttpContextAccessor));
            }
            if (services.All(u => u.ServiceType != typeof(ICacheContext)))
            {
                services.AddScoped(typeof(ICacheContext), typeof(CacheContext));
            }

            builder.Populate(services);

            _container = builder.Build();
            return _container;
        }
    }
}