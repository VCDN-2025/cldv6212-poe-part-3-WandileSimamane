using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Files.Shares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderSystem.Services;
using OrderSystem.Services.Services;
using OrderSystem.Services.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
    o.IdleTimeout = TimeSpan.FromMinutes(60);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});

string conn = builder.Configuration.GetConnectionString("AzureStorage")
              ?? throw new InvalidOperationException("AzureStorage connection string missing");

builder.Services.AddSingleton(new BlobServiceClient(conn));
builder.Services.AddSingleton(new ShareServiceClient(conn));

// Services
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

using (var scope = app.Services.CreateScope())
{
    var tableService = scope.ServiceProvider.GetRequiredService<TableService>();
    await tableService.CreateAllTablesAsync();

    var blobService = scope.ServiceProvider.GetRequiredService<BlobService>();
    var container = blobService._client.GetBlobContainerClient("product-images");
    await container.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

 // 
    var logins = await tableService.GetAllLoginsAsync(); 
    foreach (var login in logins.Where(l => l.Role == "Customer"))
    {
        var cust = await tableService.GetCustomerAsync(login.RowKey);
        if (cust == null)
        {
            await tableService.AddCustomerAsync(new Customer
            {
                PartitionKey = "Customer",
                RowKey = login.RowKey,
                CustomerName = login.Username,
                CustomerEmail = $"{login.Username}@example.com"
            });
        }
    }
    var fileService = scope.ServiceProvider.GetRequiredService<FileService>();
    var share = fileService._client.GetShareClient("contracts");
    await share.CreateIfNotExistsAsync();
}

app.Run();