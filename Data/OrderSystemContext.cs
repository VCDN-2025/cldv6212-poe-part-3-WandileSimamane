using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderSystem.Services.Shared.Models;

namespace OrderSystem.Data
{
    public class OrderSystemContext : DbContext
    {
        public OrderSystemContext (DbContextOptions<OrderSystemContext> options)
            : base(options)
        {
        }

        public DbSet<OrderSystem.Services.Shared.Models.Customer> Customer { get; set; } = default!;
        public DbSet<Order> Order { get; set; } = default!;
        public DbSet<OrderSystem.Services.Shared.Models.Product> Product { get; set; } = default!;
    }
}
