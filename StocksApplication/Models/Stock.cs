using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StocksApplication.Models
{
    [Table("Stocks")]
    public class Stock
    {
        public int Id { get; set; }
        [Required]
        [StringLength(10,MinimumLength = 3)]
        public string Symbol { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string CompanyName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Purchase { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LastDiv { get; set; }

        public string Industry { get; set; } = string.Empty;

        public long MarketCap   { get; set; }

        public List<Comment> Comments { get; set; }  = new List<Comment> { };
        public List<Portfolio> Portfolios   { get; set; } = new List<Portfolio>();

}
}
