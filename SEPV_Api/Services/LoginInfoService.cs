using SEPV_Api.Models.GreenPower;
using Gp_Api.IServices;
using Gp_Api.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Gp_Api.Services
{
    public class LoginInfoService : ILoginInfoService
    {
        private readonly GreenPowerContext _GreenPowerContext;
        private readonly string _sha256Key;

        public LoginInfoService(GreenPowerContext GreenPowerContext, IConfiguration configuration)
        {
            _GreenPowerContext = GreenPowerContext;
            _sha256Key = configuration["HashSettings:Sha256Key"]; // 從 appsettings.json 讀取 key
        }

        public short BookId { get; set; }
        public short UserId { get; set; }
        public short RoleId { get; set; }

        // 加密密碼方法
        private string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return null;

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_sha256Key));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashBytes);
        }

        public List<LoginInfoViewModel> GetAllData()
        {
            var check = _GreenPowerContext.LoginInfoRoles
                .Where(a => a.InfoId == UserId && a.RoleId == RoleId)
                .Select(b => b.Role)
                .FirstOrDefault();

            var query = _GreenPowerContext.LoginInfo
                .Select(t => new LoginInfoViewModel
                {
                    Id = t.Id,
                    Book_id = t.BookId,
                    Username = t.Username,
                    Description = t.Description,
                    Disabled = t.Disabled,
                    Company_name = t.Book.Name,
                    Roles = t.LoginInfoRoles.Select(b => new LoginRolesViewModel
                    {
                        Id = b.Role.Id,
                        Book_id = b.Role.BookId,
                        Role_name = b.Role.RoleName,
                        Description = b.Role.Description,
                        Disabled = b.Role.Disabled,
                        Is_admin = b.Role.IsAdmin,
                        Company_name = b.Role.Book.Name
                    }).ToList()
                });

            if (!check.IsAdmin)
            {
                BookId = check.BookId;
                query = query.Where(a => a.Book_id == BookId);
            }

            return query.ToList();
        }

        public LoginInfoViewModel GetData(int id)
        {
            return _GreenPowerContext.LoginInfo
                .Select(t => new LoginInfoViewModel
                {
                    Id = t.Id,
                    Book_id = t.BookId,
                    Username = t.Username,
                    Description = t.Description,
                    Disabled = t.Disabled,
                    Company_name = t.Book.Name,
                    Roles = t.LoginInfoRoles.Select(b => new LoginRolesViewModel
                    {
                        Id = b.Role.Id,
                        Book_id = b.Role.BookId,
                        Role_name = b.Role.RoleName,
                        Description = b.Role.Description,
                        Disabled = b.Role.Disabled,
                        Is_admin = b.Role.IsAdmin,
                        Company_name = b.Role.Book.Name
                    }).ToList()
                })
                .Where(a => a.Id == id && a.Book_id == BookId)
                .FirstOrDefault();
        }

        public void InsertData(LoginInfoViewModel viewModel)
        {
            var data = new LoginInfo
            {
                BookId = viewModel.Book_id,
                Username = viewModel.Username,
                Password = HashPassword(viewModel.Password), // 加密
                Description = viewModel.Description,
                Disabled = viewModel.Disabled
            };

            _GreenPowerContext.LoginInfo.Add(data);
            _GreenPowerContext.SaveChanges();

            if (viewModel.Roles != null)
            {
                InsertRoles(data.Id, viewModel);
            }
        }

        private void InsertRoles(int infoid, LoginInfoViewModel viewModel)
        {
            foreach (var Role in viewModel.Roles)
            {
                var role_data = new LoginInfoRoles
                {
                    InfoId = infoid,
                    RoleId = (int)Role.Id
                };
                _GreenPowerContext.LoginInfoRoles.Add(role_data);
            }
            _GreenPowerContext.SaveChanges();
        }

        public string DeleteData(int id)
        {
            var data = _GreenPowerContext.LoginInfo.FirstOrDefault(a => a.Id == id);
            var roles = _GreenPowerContext.LoginInfoRoles.Where(a => a.InfoId == id).ToList();

            _GreenPowerContext.LoginInfoRoles.RemoveRange(roles);
            _GreenPowerContext.LoginInfo.Remove(data);

            try
            {
                _GreenPowerContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? $"{ex}\nInnerException:{ex.InnerException}" : ex.ToString();
            }

            return "OK";
        }

        public string EditData(int id, LoginInfoViewModel viewModel)
        {
            var data = _GreenPowerContext.LoginInfo.FirstOrDefault(a => a.Id == id);

            data.Username = viewModel.Username;
            data.Password = string.IsNullOrEmpty(viewModel.Password) ? data.Password : HashPassword(viewModel.Password);
            data.Description = viewModel.Description;
            data.Disabled = viewModel.Disabled;

            var data_roles = _GreenPowerContext.LoginInfoRoles.Where(t => t.InfoId == data.Id).ToList();
            _GreenPowerContext.LoginInfoRoles.RemoveRange(data_roles);

            if (viewModel.Roles != null)
            {
                foreach (var Role in viewModel.Roles)
                {
                    if (!Role.Disabled)
                    {
                        var role_data = new LoginInfoRoles
                        {
                            InfoId = data.Id,
                            RoleId = (int)Role.Id
                        };
                        _GreenPowerContext.LoginInfoRoles.Add(role_data);
                    }
                }
            }

            try
            {
                _GreenPowerContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? $"{ex}\nInnerException:{ex.InnerException}" : ex.ToString();
            }

            return "OK";
        }

        public bool ChangePassword(int id, ChangePasswordViewModel viewModel, bool isAdmin = false)
        {
            var data = _GreenPowerContext.LoginInfo.FirstOrDefault(t => t.Id == id);

            // 驗證舊密碼
            if (data.Password != HashPassword(viewModel.Old_password) && !isAdmin) return false;

            // 設定新密碼
            data.Password = HashPassword(viewModel.New_password);
            _GreenPowerContext.SaveChanges();

            return true;
        }
    }
}
