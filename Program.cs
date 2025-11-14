
using Azure.Storage.Blobs;
using Azure.Storage.Files.Shares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
string storageConnection = builder.Configuration.GetConnectionString("AzureTableStorage")

?? builder.Configuration["AzureWebJobsStorage"]
?? throw new InvalidOperationException("Azure storage connection string not found.");

// Blob and File clients
builder.Services.AddSingleton(new BlobServiceClient(storageConnection));
builder.Services.AddSingleton(new ShareServiceClient(storageConnection));

builder.Services.AddSingleton<TableService>();          
builder.Services.AddScoped<CartService>();          
builder.Services.AddSingleton<BlobService>();         
builder.Services.AddSingleton<FileService>();       
builder.Services.AddSingleton<QueueService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseSession();

app.Run();
