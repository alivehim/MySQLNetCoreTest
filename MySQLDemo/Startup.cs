using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySQLDemo.Config;
using MySQLDemo.Core.DB;
using MySQLDemo.Core.Service;
using MySQLDemo.Core;
using MySQLDemo.Extension;

namespace MySQLDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["ConntectString"];
            //services.AddDbContext<TestDbContext>(Options=> Options.UseMySql(connectionString));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContextPool<XTestDbContext>( 
                options => options.UseMySql(connectionString));


            //add identity server configure

            var identityBuilder = services.AddIdentityServer()
               .AddDeveloperSigningCredential()
               .AddConfigurationStore(options =>
               {
                   options.ConfigureDbContext = b =>
                       b.UseMySql(connectionString);
               })
               .AddOperationalStore(options =>
               {
                   options.ConfigureDbContext = b =>
                       b.UseMySql(connectionString);
                   options.EnableTokenCleanup = true;
               })
               .AddCustomTokenRequestValidator<CustomTokenRequestValidator>()
               .AddResourceOwnerValidator<MyResourceOwnerPasswordValidator>()
               .AddMyIdentityServer();



            var builder = new ContainerBuilder();
            builder.Populate(services);
            //builder.RegisterAssemblyTypes(typeof(Startup).Assembly).AsImplementedInterfaces();

            builder.Register(context => new XTestDbContext(context.Resolve<DbContextOptions<XTestDbContext>>()))
                     .As<IORMDbContext>().InstancePerLifetimeScope();

            //repositories
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();

            var Container = builder.Build();
            return new AutofacServiceProvider(Container);
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                //serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                //context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in AssertConfig.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in AssertConfig.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in AssertConfig.GetApis())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            InitializeDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();

            app.UseMvcWithDefaultRoute();
        }
    }
}
