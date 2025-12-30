using Gp_Api.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gp_Api.IServices
{
    public interface ILoginRolesService
    {

        public short BookId { get; set; }
        public short UserId { get; set; }
        public short RoleId { get; set; }
        public List<LoginRolesViewModel> GetAllData();
        public LoginRolesViewModel GetData(int id);
        public void InsertData(LoginRolesViewModel viewModel);
        public string DeleteData(int id);
        public string EditData(int id, LoginRolesViewModel viewModel);
        // get current user the highest power roles
        public List<LoginRolesViewModel> GetUserRoles(int user_id);
        public List<LoginRolesSelectorViewModel> GetSelector();
    }
}
