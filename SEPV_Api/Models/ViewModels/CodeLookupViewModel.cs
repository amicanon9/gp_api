using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gp_Api.Models.ViewModels
{
    public class CodeLookupViewModel
    {
        [Required]
        [MaxLength(10)]
        public string Source { get; set; }
        [Required]
        [MaxLength(5)]
        public string Code { get; set; }
        [Required]
        [MaxLength(50)]
        public string Description { get; set; }
    }

    public class CodeLookupSelectorViewModel
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
