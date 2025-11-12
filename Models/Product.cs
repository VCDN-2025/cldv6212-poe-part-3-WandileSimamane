using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace OrderSystem.Models
{
    public class Product : ITableEntity
    {
        public string PartitionKey { get; set; } = "Product";
        public string RowKey { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string ProductName { get; set; }

        public string ProductDetails { get; set; }

        public string ProductImage { get; set; }

        [IgnoreDataMember]
        public DateTimeOffset? Timestamp { get; set; }

        [IgnoreDataMember]
        public ETag ETag { get; set; }
    }
}
