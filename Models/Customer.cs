using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace OrderSystem.Models
{
    public class Customer : ITableEntity
    {
        public string PartitionKey { get; set; } = "Customer";
        public string RowKey { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string CustomerName { get; set; }

        [Required, EmailAddress]
        public string CustomerEmail { get; set; }

        [IgnoreDataMember] // Azure manages this automatically 
        public DateTimeOffset? Timestamp { get; set; }

        [IgnoreDataMember] // handled by Azure
        public ETag ETag { get; set; }
    }
}
