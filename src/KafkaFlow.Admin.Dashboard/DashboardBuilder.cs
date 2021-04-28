namespace KafkaFlow.Admin.Dashboard
{
    using System.IO;
    using System.Reflection;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.SpaServices.AngularCli;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class DashboardBuilder
    {
        public static void AddKafkaFlowDashboard(this IServiceCollection services)
        {
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "AngularFiles/dist";
            });
        }

        public static void UseKafkaFlowDashboard(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = @$"{ Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\AngularFiles";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
