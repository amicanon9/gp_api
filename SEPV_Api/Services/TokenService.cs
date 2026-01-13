using Gp_Api.IServices;
using Gp_Api.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SEPV_Api.Models.PMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Gp_Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly PMSContext _PMSContext;
        private readonly string _sha256Key;

        public TokenService(PMSContext PMSContext, IConfiguration configuration)
        {
            _PMSContext = PMSContext;
            _sha256Key = configuration["HashSettings:Sha256Key"]; // 讀取加密 key
        }

        // 密碼加密方法
        private string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return null;

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_sha256Key));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashBytes);
        }

        public bool ValidateUser(LoginViewModel loginViewModel)
        {
            string hashedPassword = HashPassword(loginViewModel.Password);

            var data = _PMSContext.LoginInfo
                .Where(p => p.Username == loginViewModel.Username &&
                            p.Password == hashedPassword &&
                            !p.Disabled)
                .ToList();

            if (data == null || data.Count != 1)
            {
                return false;
            }

            return true;
        }

        public LoginInfoViewModel GetData(string Username)
        {
            return _PMSContext.LoginInfo
                .Where(p => p.Username == Username)
                .Select(
                t => new LoginInfoViewModel
                {
                    Id = t.Id,
                    Username = t.Username,
                    Description = t.Description,
                    Disabled = t.Disabled,
                    Book_id = t.BookId,
                    Company_name = t.Book.Name
                }
                ).FirstOrDefault();
        }

        public int GetRoleId(int user_id)
        {
            var data = _PMSContext.LoginInfoRoles
                .Where(t => t.InfoId == user_id)
                .Select(t => t.Role)
                .OrderByDescending(a => a.IsAdmin)
                .FirstOrDefault();

            if (data == null) return -1;
            return data.Id;
        }

        public LoginRoles GetLoginRoles(int role_id)
        {
            var data = _PMSContext.LoginRoles.Include(r => r.Book).FirstOrDefault(t => t.Id == role_id);
            return data;
        }

        public int SwitchRoleId(int user_id, int role_id)
        {
            var data = _PMSContext.LoginInfoRoles
                .FirstOrDefault(t => t.InfoId == user_id && t.RoleId == role_id);

            if (data == null) return -1;
            return data.RoleId;
        }
    }
}
