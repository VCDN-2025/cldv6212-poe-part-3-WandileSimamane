using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderSystem.Services;
using OrderSystem.Models;
using Azure.Data.Tables;


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

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Registered The Services by passing the connection string from configuration.
            builder.Services.AddSingleton(x => new TableService(builder.Configuration));
            builder.Services.AddSingleton(x => new BlobService(builder.Configuration));
            builder.Services.AddSingleton(x => new QueueService(builder.Configuration));
            builder.Services.AddSingleton(x => new FileService(builder.Configuration));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
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