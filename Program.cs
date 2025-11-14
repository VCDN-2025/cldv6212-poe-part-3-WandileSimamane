using Azure.Storage.Blobs;
using Azure.Storage.Files.Shares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderSystem.Services;
using OrderSystem.Services.Services;

var builder = WebApplication.CreateBuilder(args);




builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Azure Storage Connection String
string storageConnection = builder.Configuration.GetConnectionString("AzureTableStorage")
    ?? builder.Configuration["AzureWebJobsStorage"]
    ?? throw new InvalidOperationException("Azure storage connection string not found.");

// Register Azure Clients
builder.Services.AddSingleton(new BlobServiceClient(storageConnection));
builder.Services.AddSingleton(new ShareServiceClient(storageConnection));

// Register Application Services
builder.Services.AddSingleton<TableService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddSingleton<BlobService>();
builder.Services.AddSingleton<FileService>();
builder.Services.AddSingleton<QueueService>();

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

app.Run();