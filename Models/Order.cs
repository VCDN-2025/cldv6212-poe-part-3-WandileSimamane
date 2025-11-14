using Azure;
using Azure.Data.Tables;
using System;

namespace OrderSystem.Models
{
    public class Order : ITableEntity
    {
        public string PartitionKey { get; set; } = "Order";
        public string RowKey { get; set; } = Guid.NewGuid().ToString(); 

        public string CustomerId { get; set; } 
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;

        public string Status { get; set; } = "Pending";
        public double? Total { get; set; }

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string ProductId { get; internal set; }
        public int Quantity { get; internal set; }
    }
}
