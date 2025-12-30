using SEPV_Api.Models.GreenPower;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gp_Api.Models.ViewModels
{
    public class LoginRolesViewModel
    {
        public int? Id { get; set; }
        [Required]
        public short Book_id { get; set; }
        [Required]
        public string Role_name { get; set; }
        public string Description { get; set; }
        [Required]
        public bool Disabled { get; set; }
        [Required]
        public bool Is_admin { get; set; }
        public string Company_name { get; set; }
        public byte Permission_level { get; set; }
        public List<LoginMenus> Menus { get; set; }
    }

    public class LoginRolesSelectorViewModel
    {
        public int Id { get; set; }
        public string Role_name { get; set; }
        public bool Disabled { get; set; }
        public byte Permission_level { get; set; }
    }
}
