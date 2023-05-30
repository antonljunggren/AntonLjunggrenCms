using AntonLjunggrenCms.Core.Services;
using AntonLjunggrenCms.Data.Entites;
using AntonLjunggrenCms.Data.Persistance;
using AntonLjunggrenCms.Data.Services;
using Microsoft.EntityFrameworkCore;

namespace AntonLjunggrenCms.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Insights
            var appiOptions = new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions();
            appiOptions.EnableAdaptiveSampling = false;
            appiOptions.EnableQuickPulseMetricStream = false;

            //if(!builder.Environment.IsDevelopment())
                builder.Services.AddApplicationInsightsTelemetry(appiOptions);

            // Add services to the container.

            builder.Services.AddControllers();

            //Ef Context
            var cosmoDbConn = builder.Configuration.GetConnectionString("cosmo_connection") ?? "";
            var cosmoDbName = builder.Configuration["cosmo_db_name"] ?? "";

            builder.Services.AddPooledDbContextFactory<EfContext>(opt =>
                opt.UseCosmos(cosmoDbConn, cosmoDbName));

            builder.Services.AddScoped<IRepository<PhotographEntity, string>, PhotographRepository>();

            var blobConn = builder.Configuration.GetConnectionString("blob_storage_connection") ?? "";
            var blobContainer = builder.Configuration["blob_storage_container"] ?? "";

            builder.Services.AddTransient<IFileService>(sp =>
            {
                return new AzureBlobFileService(blobConn, blobContainer);
            });

            builder.Services.AddScoped<PhotographService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {

            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}