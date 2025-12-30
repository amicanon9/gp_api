using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gp_Api.Models.ViewModels
{
    public class SetOfBooksViewModel
    {
        [Required]
        public short Book_id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
