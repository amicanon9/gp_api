using Newtonsoft.Json;
using Gp_Api.Tools.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gp_Api.Models.ViewModels
{
    public class ProformaInvoicesViewModel
    {
        public int? Id { get; set; }
        [Required]
        public short Organization_id { get; set; }
        public int? Pi_number { get; set; }
        [Required]
        public string Cust_id { get; set; }
        public string Cust_name { get; set;}
        [Required]
        public string Status_id { get; set; }
        [Required]
        public short Currency_id { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public string Description { get; set; }
        [Required]
        //[JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime Pi_date { get;set; }
        public int? Created_by_id { get; set; }
        public string Created_by { get; set; }
        //[JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime? Created_time { get; set; }
        public int? Updated_by_id { get; set; }
        public string Updated_by { get; set; }
        //[JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime? Updated_time { get; set; }

        // read only
           // Organizations 
        public string Organization_name { get; set; }
           // Customers
        public string Customer_name { get; set; }
            // Currency_config
        public string Currency { get; set; }
            // Status
        public string Status { get; set; }
            // invoices amount
        public decimal? Invoices_amount { get; set; }
            // invoices
        public List<InvoicesViewModel> Invoices { get; set; }
    }
    public class ProformaInvoicesSelectorViewModel
    {
        public int Id { get; set; }
        public int Pi_number { get; set; }
        public string Cust_name { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public List<InvoicesViewModel> Invoices { get; set; }
        public DateTime? Created_time { get; set; }
        public DateTime? Updated_time { get; set; }
    }
}
