using Gp_Api.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gp_Api.IServices
{
    public interface ILoginInfoService
    {
        public short BookId { get; set; }
        public short UserId { get; set; }
        public short RoleId { get; set; }
        public List<LoginInfoViewModel> GetAllData();
        public LoginInfoViewModel GetData(int id);
        public void InsertData(LoginInfoViewModel viewModel);
        public string DeleteData(int id);
        public string EditData(int id, LoginInfoViewModel viewModel);
        public bool ChangePassword(int id, ChangePasswordViewModel viewModel, bool isAdmin = false);
    }
}
