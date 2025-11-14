using Azure;
using Azure.Data.Tables;
using System;

namespace OrderSystem.Models
{
    public class CartItem : ITableEntity
    {
   
        public string PartitionKey { get; set; }  
        public string RowKey { get; set; }        

        public string CartId { get; set; }
        public string ProductId { get; set; }

        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }

        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
