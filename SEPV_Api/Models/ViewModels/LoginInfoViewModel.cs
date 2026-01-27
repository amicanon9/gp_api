using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gp_Api.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string Old_password { get; set; }
        public string New_password { get; set; }
    }
    public class AddRoleViewModel
    {
        public int Role_id { get; set; }
    }
    public class LoginInfoViewModel
    {
        public int? Id { get; set; }
        [Required]
        public short Book_id { get; set; }
        [Required]
        [MaxLength(20)]
        public string Username { get; set; }
        [MaxLength(30)]
        public string Password { get; set; }
        [Required]
        public string Description { get; set; }
        public bool Disabled { get; set; }

        // --- 新增欄位 ---
        public DateTime? Joined_date { get; set; } // 到職日
        public int? Dept_id { get; set; }           // 部門 ID
                                                    // ----------------

        public string Company_name { get; set; }
        public List<LoginRolesViewModel> Roles { get; set; }
    }
}
