
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gp_Api.Models.ViewModels
{
    public class CodeLookupSourceViewModel
    {
        [Required]
        [MaxLength(10)]
        public string Source { get; set; }
        [MaxLength(50)]
        public string Description { get; set; }
    }
}
