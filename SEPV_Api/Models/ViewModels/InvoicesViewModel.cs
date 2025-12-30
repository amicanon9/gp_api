using Gp_Api.Tools.Converters;
using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Gp_Api.Models.ViewModels
{
    public class InvoicesViewModel
    {
        public int? Id { get; set; }
        [MaxLength(10)]
        [Required]
        public string Invoice_number { get; set; }
        [Required]
        //[JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime Invoice_date { get; set; }
        public string Description { get; set; }
        public short? Currency_id { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string Status_id { get; set; }
        [Required]
        public int Pi_id { get; set; }
        public int? Created_by_id { get; set; }
        public string Created_by { get; set; }
        //[JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime? Created_time { get; set; }
        public int? Updated_by_id { get; set; }
        public string Updated_by { get; set; }
        //[JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime? Updated_time { get; set; }
        public bool Notify { get; set; }
        //[JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime? Notify_date {get;set;}

        // read only
            // Currency_config
        public string Currency { get; set; }
            // Status
        public string Status { get; set; }
            // proformaInvoices
        public int? Pi_number { get; set; }
           // proformaInvoices customer_id
        public string Customer_id { get;set;}
        public string Customer_name { get; set; }
    }
}
