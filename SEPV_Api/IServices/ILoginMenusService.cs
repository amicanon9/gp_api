using SEPV_Api.Models.GreenPower;
using Gp_Api.Models.ViewModels;
using System.Collections.Generic;

namespace Gp_Api.IServices
{
    public interface ILoginMenusService
    {
        public short BookId { get; set; }
        public short UserId { get; set; }
        public short RoleId { get; set; }
        public LoginMenusFormViewModel GetMenu(int user_id, int role_id);
        public List<LoginMenus> GetRoleMenus(int user_id, int role_id);
        public List<LoginMenus> GetLoginMenus(); 
    }
}
