using AntonLjunggrenCms.Core.Services;
using AntonLjunggrenCms.Data.Entites;
using AntonLjunggrenCms.Data.Persistance;
using AntonLjunggrenCms.Data.Services;
using Microsoft.EntityFrameworkCore;

namespace AntonLjunggrenCms.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

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

            builder.Services.AddTransient<ImageProcessingService>();

            builder.Services.AddScoped<PhotographService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}