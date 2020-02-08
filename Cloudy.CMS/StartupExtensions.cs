﻿using Cloudy.CMS.ComponentSupport;
using Cloudy.CMS.DocumentSupport.MongoSupport;
using Microsoft.Extensions.DependencyInjection;
using Poetry;
using Poetry.AspNetCore.DependencyInjectionSupport;
using Poetry.ComponentSupport;
using Poetry.DependencyInjectionSupport;
using Poetry.InitializerSupport;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Cloudy.CMS
{
    public static class StartupExtensions
    {
        public static void AddCloudy(this IServiceCollection services)
        {
            AddCloudy(services, cloudy => { });
        }
        public static void AddCloudy(this IServiceCollection services, Action<CloudyConfigurator> configure)
        {
            var options = new CloudyOptions();

            options.Components.Add(typeof(CloudyComponent));
            options.ComponentAssemblies.Add(Assembly.GetCallingAssembly());

            var configurator = new CloudyConfigurator(services, options);

            configure(configurator);

            if (!options.HasDocumentProvider)
            {
                configurator.WithInMemoryDatabase();
            }

            var container = new Container(services);

            container.RegisterSingleton<IComponentAssemblyProvider>(new ComponentAssemblyProvider(options.ComponentAssemblies));
            container.RegisterSingleton<IComponentTypeProvider>(new ComponentTypeProvider(options.Components));

            new PoetryDependencyInjector().InjectDependencies(container);

            foreach (var injector in container.CreateResolver().Resolve<IDependencyInjectorProvider>().GetAll())
            {
                injector.InjectDependencies(container);
            }

            if (options.DatabaseConnectionString != null)
            {
                container.RegisterSingleton<IDatabaseConnectionStringNameProvider>(new DatabaseConnectionStringNameProvider(options.DatabaseConnectionString));
            }

            foreach (var initializer in services.BuildServiceProvider().GetRequiredService<IInitializerProvider>().GetAll())
            {
                initializer.Initialize();
            }
        }
    }
}
