using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gp_Api.Models.ViewModels
{
    public class CurrencyConfigViewModel
    {
        public short? Id { get; set; }
        [MaxLength(3)]
        [Required]
        public string Currency { get; set; }
        [MaxLength(20)]
        public string Description { get; set; }
        [Required]
        public bool Disabled { get; set; }
        [Required]
        public byte DecimalPrecision { get; set; }
    }

    public class CurrencyConfigSelectorViewModel
    {
        public short Id { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
    }
}
