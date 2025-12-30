using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Gp_Api.Models.ViewModels
{
    public class CustomersViewModel
    {

        public string Cust_id { get; set; }
        [Required]
        public string Customer_name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public string ContactCellPhone { get; set; }
        public string GUI_number { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? Header_id { get; set; }
        [Required]
        [MaxLength(5)]
        public string Category_id { get; set; }

        public string Category { get; set; }

        public object? ComparedBy { get; set; }
    }

    public class CustomersSelectorViewModel
    {
        public string Cust_id { get; set; }
        public string Customer_name { get; set; }
    }

    public class CustomersUpdateViewModel
    {
        public List<CustomersViewModel> Update { get; set; }
        public List<CustomersViewModel> New { get; set; }
    }
}
