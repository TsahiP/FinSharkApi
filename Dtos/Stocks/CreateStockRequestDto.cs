using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Stocks
{
    public class CreateStockRequestDto
    {

        [Required]
        [MinLength(1, ErrorMessage = "Symbol must be at least 1 Chareavters")]
        [MaxLength(10, ErrorMessage = "Symbol cannot be over 10 Chareacters.")]
        public string Symbol { get; set; } = string.Empty;
        [Required]
        [MaxLength(20, ErrorMessage = "Company Name cannot be over 10 Chareacters.")]
        public string CompanyName { get; set; } = string.Empty;
        [Required]
        [Range(1, 1000000000)]
        public decimal Purchase { get; set; }
        [Required]
        [Range(0.001, 100)]
        public decimal Dividend { get; set; }
        [Required]
        [Range(1, 1000000000)]
        public decimal LastDiv { get; set; }
        [Required]
        [MaxLength(10, ErrorMessage = "Industry cannot be over 10 Chareacters.")]
        public string Industry { get; set; } = string.Empty;
        [Range(1,5000000000)]
        public long MarketCap { get; set; }
    }
}