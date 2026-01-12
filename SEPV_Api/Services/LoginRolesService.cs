using SEPV_Api.Models.GreenPower;
using Gp_Api.IServices;
using Gp_Api.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gp_Api.Services
{
    public class LoginRolesService: ILoginRolesService
    {
        private readonly PMSContext _PMSContext;
        private readonly ILoginMenusService _loginMenusService;
        public LoginRolesService(PMSContext PMSContext, ILoginMenusService loginMenusService)
        {
            _PMSContext = PMSContext;
            _loginMenusService = loginMenusService;
        }

        public short BookId { get; set; }
        public short UserId { get; set; }
        public short RoleId { get; set; }

        public List<LoginRolesViewModel> GetAllData()
        {
            var check = _PMSContext.LoginInfoRoles.Where(a => a.InfoId == UserId && a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var query = _PMSContext.LoginRoles.Select(t => new LoginRolesViewModel
            {
                Id = t.Id,
                Book_id = t.BookId,
                Role_name = t.RoleName,
                Description = t.Description,
                Disabled = t.Disabled,
                Is_admin = t.IsAdmin,
                Company_name = t.Book.Name,
                Permission_level = t.PermissionLevel,
                Menus = new List<LoginMenus>()
            });
            // 如果不是 admin 才加上條件
            if (!check.IsAdmin)
            {
                query = query.Where(a => a.Book_id == check.BookId);
            }
            var allData = query.ToList();
            foreach (var data in allData)
            {
                var menus = new List<LoginMenus>();
                if (!data.Is_admin)
                    menus = _PMSContext.LoginRolesMenus.Where(a => a.RoleId == data.Id).Select(b => b.Menu).Where(c => c.IsNode == true).ToList();
                else
                    menus = _PMSContext.LoginMenus.Where(a => a.IsNode == true).ToList();
                data.Menus = menus;
            }
            return allData;
        }

        public LoginRolesViewModel GetData(int id)
        {
            var data = _PMSContext.LoginRoles.Select(t => new LoginRolesViewModel
            {
                Id = t.Id,
                Book_id = t.BookId,
                Role_name = t.RoleName,
                Description = t.Description,
                Disabled = t.Disabled,
                Is_admin = t.IsAdmin,
                Company_name = t.Book.Name,
                Permission_level = t.PermissionLevel,
                Menus = _PMSContext.LoginRolesMenus.Where(a => a.RoleId == t.Id).Select(b => b.Menu).ToList()
            }).Where(a => a.Id == id && a.Book_id == BookId).FirstOrDefault();

            var menus = new List<LoginMenus>();
            if (!data.Is_admin)
                menus = _PMSContext.LoginRolesMenus.Where(a => a.RoleId == data.Id).Select(b => b.Menu).Where(c => c.IsNode == true).ToList();
            else
                menus = _PMSContext.LoginMenus.Where(a => a.IsNode == true).ToList();
            data.Menus = menus;
            return data;
        }

        public void InsertData(LoginRolesViewModel viewModel)
        {
            var check = _PMSContext.LoginInfoRoles.Where(a => a.InfoId == UserId && a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            
            var data = new LoginRoles
            {
                BookId = viewModel.Book_id,
                RoleName = viewModel.Role_name,
                Description = viewModel.Description,
                Disabled = viewModel.Disabled,
                IsAdmin = viewModel.Is_admin,
                PermissionLevel = viewModel.Permission_level,
            };
            if (!check.IsAdmin)
            {
                data.IsAdmin = false;
            }
            _PMSContext.LoginRoles.Add(data);
            _PMSContext.SaveChanges();
            if (viewModel.Menus != null)
            {
                insertMenus(data.Id,viewModel);
            }
        }

        private void insertMenus(int roleid,LoginRolesViewModel viewModel)
        {
            var myId = _PMSContext.LoginRoles
                .Where(a => a.BookId == BookId && a.RoleName == viewModel.Role_name).Select(b => b.Id).FirstOrDefault();
            foreach(var menu in viewModel.Menus)
            {
                var menu_data = new LoginRolesMenus
                {
                    RoleId = roleid,
                    MenuId = menu.Id
                };
                _PMSContext.LoginRolesMenus.Add(menu_data);
            }
            try
            {
                _PMSContext.SaveChanges();

            }
            catch (Exception ex)
            {
                var a = ex;
            }
        }
        public string DeleteData(int id)
        {
            var data = _PMSContext.LoginRoles.Where(a => a.Id == id).FirstOrDefault();
            var menus = _PMSContext.LoginRolesMenus.Where(a => a.RoleId == id).ToList();
            foreach(var menu in menus)
            {
                _PMSContext.LoginRolesMenus.Remove(menu);
            }
            
            _PMSContext.LoginRoles.Remove(data);
            try
            {
                _PMSContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? $"{ex}\nInnerException:{ex.InnerException}": ex.ToString();

            }

            return "OK";
        }

        public string EditData(int id, LoginRolesViewModel viewModel)
        {
            var check = _PMSContext.LoginInfoRoles.Where(a => a.InfoId == UserId && a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var data = _PMSContext.LoginRoles.Where(a => a.Id == id).FirstOrDefault();
            
            data.RoleName = viewModel.Role_name ?? data.RoleName;
            data.Description = viewModel.Description ?? data.Description;
            data.Disabled = viewModel.Disabled;
            data.IsAdmin = viewModel.Is_admin;
            data.PermissionLevel = viewModel.Permission_level;
            if (!check.IsAdmin)
            {
                data.IsAdmin = false;
            }
            var data_menus = _PMSContext.LoginRolesMenus.Where(t => t.RoleId == data.Id).ToList();
            foreach(var data_menu in data_menus)
            {
                _PMSContext.Remove(data_menu);
            }
            if (viewModel.Menus != null)
            {
                foreach (var menu in viewModel.Menus)
                {
                    var menu_data = new LoginRolesMenus
                    {
                        RoleId = data.Id,
                        MenuId = menu.Id
                    };
                    _PMSContext.LoginRolesMenus.Add(menu_data);
                }
            }
            try
            {
                _PMSContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? $"{ex}\nInnerException:{ex.InnerException}": ex.ToString();

            }

            return "OK";
        }

        public List<LoginRolesViewModel> GetUserRoles(int user_id)
        {
            return _PMSContext.LoginInfoRoles.Where(t => t.InfoId == user_id).Select(a => a.Role)
                .Where(c => c.Disabled == false).Select(b => new LoginRolesViewModel
                {
                Id = b.Id,
                Book_id = b.BookId,
                Role_name = b.RoleName,
                Description = b.Description,
                Disabled = b.Disabled,
                Is_admin = b.IsAdmin,
                Company_name = b.Book.Name,
                Permission_level = b.PermissionLevel
                }).OrderByDescending(t => t.Is_admin).ToList();
        }

        public List<LoginRolesSelectorViewModel> GetSelector()
        {
            var check = _PMSContext.LoginInfoRoles.Where(a => a.InfoId == UserId && a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var query = _PMSContext.LoginRoles.AsQueryable();

            // 如果不是 admin，就加上 BookId 條件
            if (!check.IsAdmin)
            {
                query = query.Where(a => a.BookId == check.BookId);
            }


            // 始終保留 Disabled 條件
            query = query.Where(a => a.Disabled == false);

            // 專案轉換
            return query.Select(b => new LoginRolesSelectorViewModel
            {
                Id = b.Id,
                Role_name = b.RoleName,
                Disabled = b.Disabled,
                Permission_level = b.PermissionLevel,
            }).ToList();
        }
    }
}
