using SEPV_Api.Models.GreenPower;
using Gp_Api.IServices;
using System.Collections.Generic;
using System.Linq;
using Gp_Api.Models.ViewModels;
namespace Gp_Api.Services
{
    public class LoginMenusService: ILoginMenusService
    {
        private readonly PMSContext _PMSContext;

        public LoginMenusService(PMSContext PMSContext)
        {
            _PMSContext = PMSContext;
        }

        public short BookId { get; set; }
        public short UserId { get; set; }
        public short RoleId { get; set; }

        public LoginMenusFormViewModel GetMenu(int user_id, int role_id)
        {
            var role = _PMSContext.LoginInfoRoles.Where(t => t.RoleId == role_id);
            var role_result = new LoginRoles();
            if (user_id  == -1) role_result =  role.Select(a => a.Role).Where(c => c.Disabled == false).FirstOrDefault();
            else role_result =  role.Where(t => t.InfoId == user_id).Select(a => a.Role).Where(c => c.Disabled == false).FirstOrDefault();
         
            // get role's node menus, and select node menus from books_menus.
            var menus = new List<LoginMenus>();
            if (role_result.IsAdmin)
            {
                menus = _PMSContext.LoginMenus.Where(t => t.IsNode == true)
                    .OrderBy(b => b.SeqNo).ToList();
            }
            else
            {
                menus = _PMSContext.LoginRolesMenus
                    .Where(t => t.RoleId == role_result.Id).Select(a => a.Menu)
                    .Where(c => c.IsNode == true).OrderBy(b => b.SeqNo).ToList();
            }
            if (menus == null)
            {
                return new LoginMenusFormViewModel
                {
                    Id = role_result.Id,
                    Name = role_result.RoleName,
                    Menus = new List<LoginMenusViewModel>()
                };
            }
            else
            {
              
                var temp_menus = new List<LoginMenusViewModel>();
                var memory_parent = new List<int>();
                foreach (var menu in menus)
                {
                    if (menu.Parent == null)
                    {
                        var temp_data =
                        new LoginMenusViewModel
                        {
                            Id = menu.Id,
                            Name = menu.MenuName,
                            Description = menu.Description,
                            Url = menu.Url,
                            Children = new List<object>(),
                            Icon = menu.Icon,
                            Seq_no = menu.SeqNo,
                        };
                        temp_menus.Add(temp_data);
                    }
                    else if (menu.Parent != null)
                    {
                        var parent = menus.Where(t => t.Id == menu.Parent).FirstOrDefault();
                        if (parent != null || memory_parent.Contains((int)menu.Parent)) continue;
                        memory_parent.Add((int)menu.Parent);
                        temp_menus.Add(GetChild((int)menu.Parent, menus));
                        
                    }
                }

                temp_menus = temp_menus.OrderBy(t => t.Seq_no).ToList();
                return new LoginMenusFormViewModel
                {
                    Id = role_result.Id,
                    Name = role_result.RoleName,
                    Menus = temp_menus
                };
            }
        }
        private LoginMenusViewModel GetChild(int parent_id, List<LoginMenus> menus, string prev_url = "")
        {
            var parent = _PMSContext.LoginMenus.Where(t => t.Id == parent_id).FirstOrDefault();

            var menu = menus.Where(t => t.Parent == parent_id).ToList()
                .OrderBy(b => b.SeqNo);
            var temp = new List<object>();
            foreach (var data in menu)
            {
                var data_prev = prev_url + data.Url;
                if (!(bool)data.IsNode) temp.Add(GetChild(data.Id, menus, data_prev));
                else
                {
                    var temp_data =
                        new LoginMenusViewModel
                        {
                            Id = data.Id,
                            Name = data.MenuName,
                            Description = data.Description,
                            Url = data.Url,
                            Icon = data.Icon,
                            Seq_no = data.SeqNo
                        };
                    temp_data.Url = prev_url + parent.Url + data.Url;
                    temp.Add(temp_data);
                }
            }
            return new LoginMenusViewModel
            {
                Id = parent.Id,
                Name = parent.MenuName,
                Description = parent.Description,
                Url = "",
                Children = temp,
                Icon = parent.Icon,
                Seq_no = parent.SeqNo
            };

        }
        public List<LoginMenus> GetRoleMenus(int user_id, int role_id)
        {
            var menus = _PMSContext.LoginMenus.ToList();
            var check = _PMSContext.LoginInfoRoles.Where(a => a.InfoId == user_id && a.RoleId == role_id).Select(b => b.Role).FirstOrDefault();
            var temp = new List<LoginMenus>();
            if (check != null)
            {
                if (check.IsAdmin) return menus;
                var role_menus = _PMSContext.LoginRolesMenus.Where(a => a.RoleId == role_id).Select(b => b.Menu).ToList();
                foreach (var menu in role_menus)
                {
                    if (menus.Select(b => b.Id).Contains(menu.Id)) temp.Add(menu);
                }
            }
            return temp;
        }
        public List<LoginMenus> GetLoginMenus()
        {
            var check = _PMSContext.LoginInfoRoles.Where(a => a.InfoId == UserId && a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var data = new List<LoginMenus>();
            // 如果不是 admin，就加上 BookId 條件
            if (!check.IsAdmin)
            {
                data = _PMSContext.LoginRolesMenus.Where(a => a.RoleId == RoleId).Select(e => e.Menu).ToList();
            }
            else
            {
                data = _PMSContext.LoginMenus.ToList();
            }

            // 專案轉換
            return data;
        }
    }
}
