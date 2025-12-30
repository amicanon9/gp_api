using Newtonsoft.Json;
using Gp_Api.Tools.Converters;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gp_Api.Models
{
    public class ContractsViewModel
    {
        public int? Id { get; set; }
        [Required]
        public string Contract_number { get; set; }
        [Required]
        public int Pi_id { get; set; }
        public int? Pi_number { get; set; }
        [Required]
        public string Status_id { get; set; }
        public string Status { get; set; }
        //[JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime? Start_date { get; set; }
        //[JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime? End_date { get; set; }
        public string Description { get; set; }
        public int? Created_by_id { get; set; }
        public string Created_by { get; set; }
        //[JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime? Created_time { get; set; }
        public int? Updated_by_id { get; set; }
        public string Updated_by { get; set; }
        //[JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime? Updated_time { get; set; }

        // read only
        public string Customer_id { get;set;}
        public string Customer_name { get;set;}
    }
}
