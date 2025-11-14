using Azure;
using Azure.Data.Tables;
using System;

namespace OrderSystem.Models
{
    public class CartEntity : ITableEntity
    {
        public string PartitionKey { get; set; } = "Cart";
        public string RowKey { get; set; } = Guid.NewGuid().ToString();

        public string UserId { get; set; } 

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
