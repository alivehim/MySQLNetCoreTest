using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySQLDemo.Core.DB;
using MySQLDemo.Core.Service;
using MySQLDemo.Framework.Config;
using MySQLDemo.Framework.Infrastructure;
using MySQLDemo.Resource.Extension;
using MySQLDemo.Resource.Model;
using MySQLDemo.Resource.Service;
using SaasKit.Multitenancy;
using SaasKit.Multitenancy.Extensions;
using Swashbuckle.AspNetCore.Swagger;

namespace MySQLDemo.Resource
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddMultitenancy<AppTenant, CachingAppTenantResolver>();

            //services.AddDbContextPool<IORMDbContext, TenantDbContext>(opts=> { });

            //services.AddEntityFrameworkSqlServer().AddDbContext<IORMDbContext,TenantDbContext > ();
            services.AddDbContext<IORMDbContext, TenantDbContext>();


            services.Configure<ConsulConfig>(Configuration.GetSection("consulConfig"));
            services.Configure<RedisConfig>(Configuration.GetSection("consulConfig"));


            services.AddHealthChecks();
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var address = Configuration["consulConfig:address"];
                consulConfig.Address = new Uri(address);
            }));

            services.Configure<MultitenancyOptions>(Configuration.GetSection("Multitenancy"));

            services.AddSingleton<IRedisRepository, RedisRepository>();
            //services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, ConsulHostedService>();

            //services.AddAuthentication("Bearer")
            //.AddJwtBearer("Bearer", options =>
            //{
            //    options.Authority = "http://localhost:8020";
            //    options.RequireHttpsMetadata = false;

            //    options.Audience = "api1";
            //});

            //services.AddAuthentication("Bearer")
            //   .AddIdentityServerAuthentication(options =>
            //   {
            //       options.Authority = "http://localhost:8020";
            //       options.RequireHttpsMetadata = false;
            //       options.ApiName = "api1";
            //   });

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            //});
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(Configuration["SwaggerConfigure:DocName"], new Info
                {
                    Title = Configuration["SwaggerConfigure:Title"],
                    Version = Configuration["SwaggerConfigure:Version"],
                    Description = Configuration["SwaggerConfigure:Description"],
                    Contact = new Contact
                    {
                        Name = Configuration["SwaggerConfigure:Contact:Name"],
                        Email = Configuration["SwaggerConfigure:Contact:Email"]
                    }
                });
            });

            var builder = new ContainerBuilder();
            builder.Populate(services);
            //builder.RegisterAssemblyTypes(typeof(Startup).Assembly).AsImplementedInterfaces();

            //builder.Register(context => new XTestDbContext(context.Resolve<DbContextOptions<XTestDbContext>>()))
            //         .As<IORMDbContext>().InstancePerLifetimeScope();

            //repositories
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();

            var Container = builder.Build();
            return new AutofacServiceProvider(Container);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env ,IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.RegisterToConsul(Configuration, lifetime);

            app.UseHealthChecks("/Health", new HealthCheckOptions()
            {
                ResultStatusCodes =
                {
                    [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
                    [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                }
            });


            //app.UseSwagger();
            ////启用中间件服务对swagger-ui，指定Swagger JSON终结点
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            //});

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "doc/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint($"/doc/{Configuration["SwaggerConfigure:DocName"]}/swagger.json",
                    $"{Configuration["SwaggerConfigure:Name"]} {Configuration["SwaggerConfigure:Version"]}");
            });


            app.UseAuthentication();
            app.UseMultitenancy<AppTenant>();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" }
                );
            });
        }
    }
}
