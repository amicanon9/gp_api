using Gp_Api.Models.ViewModels;
using SEPV_Api.Models.PMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gp_Api.IServices
{
    public interface ITokenService
    {
        bool ValidateUser(LoginViewModel loginViewModel);
        public LoginInfoViewModel GetData(string Username);
        public int GetRoleId(int user_id);
        public int SwitchRoleId(int user_id, int role_id);
        public LoginRoles GetLoginRoles(int role_id);
    }
}
