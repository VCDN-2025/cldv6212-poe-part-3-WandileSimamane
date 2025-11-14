using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderSystem.Services;

namespace OrderSystem
{
    public interface IQueueService
    {
        void SendMessage(string queueName, string message);
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---------------------------
            // Add services to DI container
            // ---------------------------
            builder.Services.AddControllersWithViews();

            // Session setup
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            builder.Services.AddSingleton<TableService>(x => new TableService(builder.Configuration));
            builder.Services.AddScoped<CartService>(); 
            builder.Services.AddSingleton<BlobService>(x => new BlobService(builder.Configuration));
            builder.Services.AddSingleton<QueueService>(x => new QueueService(builder.Configuration));
            builder.Services.AddSingleton<FileService>(x => new FileService(builder.Configuration));

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
        }
    }
}
