using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Gp_Api.Models.ViewModels
{
    public class IvProfileHeadersViewModel
    {
        public string company_name { get; set; }
        public int header_id { get; set; }
    }
}
