using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderSystem.Models;

namespace OrderSystem.Data
{
    public class OrderSystemContext : DbContext
    {
        public OrderSystemContext (DbContextOptions<OrderSystemContext> options)
            : base(options)
        {
        }

        public DbSet<OrderSystem.Models.Customer> Customer { get; set; } = default!;
        public DbSet<OrderSystem.Models.Order> Order { get; set; } = default!;
        public DbSet<OrderSystem.Models.Product> Product { get; set; } = default!;
    }
}
