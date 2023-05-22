using AntonLjunggrenCms.Data.Entites;
using AntonLjunggrenCms.Data.Persistance;
using AntonLjunggrenCms.Data.Services;
using AntonLjunggrenCms.WebEditor.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

//Ef Context
var cosmoDbConn = builder.Configuration.GetConnectionString("cosmo-connection") ?? "";
var cosmoDbName = builder.Configuration["cosmo-db-name"] ?? "";

builder.Services.AddPooledDbContextFactory<EfContext>(opt =>
    opt.UseCosmos(cosmoDbConn, cosmoDbName));

builder.Services.AddScoped<IRepository<PhotographEntity, string>, PhotographRepository>();

var blobConn = builder.Configuration.GetConnectionString("blob-storage-connection") ?? "";
var blobContainer = builder.Configuration["blob-storage-container"] ?? "";

builder.Services.AddTransient<IFileService>(sp =>
{
    return new AzureBlobFileService(blobConn, blobContainer);
});

builder.Services.AddScoped<PhotographService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
