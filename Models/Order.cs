using System;
using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace OrderSystem.Models
{
    public class Order : ITableEntity
    {
        public string PartitionKey { get; set; } = "Order";
        public string RowKey { get; set; } = Guid.NewGuid().ToString();

        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int Quantity { get; set; }

        [Required]
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
