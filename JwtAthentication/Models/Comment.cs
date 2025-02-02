using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SecondApp.Models
{
    public class Comment
    {
    // ForiegnKey  class 
        public int Id { get; set; }
        public int? StockID { get; set; }
        public string  Content { get; set; } = string.Empty;
        public string? Title { get; set; }
        public DateTime CreatedOn { get; set;} = DateTime.Now;
    }
}