// Program.cs (MVC Web app)
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

// --- Register Azure SDK clients using the same connection string ---
// Use the storage connection string you already have in appsettings.json:
// "AzureWebJobsStorage" or dedicated keys like "AzureBlobStorage"
string storageConnection = builder.Configuration.GetConnectionString("AzureTableStorage")
// fallback to AzureWebJobsStorage if you prefer
?? builder.Configuration["AzureWebJobsStorage"]
?? throw new InvalidOperationException("Azure storage connection string not found.");

// Blob and File clients
builder.Services.AddSingleton(new BlobServiceClient(storageConnection));
builder.Services.AddSingleton(new ShareServiceClient(storageConnection));

// Register your services which accept SDK clients or IConfiguration
builder.Services.AddSingleton<TableService>();           // TableService accepts IConfiguration in this rewrite
builder.Services.AddScoped<CartService>();              // scoped OK for web requests
builder.Services.AddSingleton<BlobService>();           // BlobService accepts BlobServiceClient
builder.Services.AddSingleton<FileService>();           // FileService accepts ShareServiceClient
builder.Services.AddSingleton<QueueService>();          // QueueService accepts IConfiguration

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
