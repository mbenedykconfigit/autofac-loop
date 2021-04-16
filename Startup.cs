using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Diagnostics;
using NLog;
using ILogger = NLog.ILogger;

namespace AutofacInfiniteLoop
{
    public class Startup
    {
        private ILogger _nLogger;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var resolveOperationTracer = new DefaultDiagnosticTracer();
            resolveOperationTracer.OperationCompleted += (sender, args) =>
            {
                // HERE
                // this will try to resolve IHttpContextAccessor
                LogManager.GetLogger(nameof(DefaultDiagnosticTracer)).Info(args.TraceContent); 
            };

            builder.RegisterBuildCallback(c =>
            {
                var container = c as IContainer;
                container.SubscribeToDiagnostics(resolveOperationTracer);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
