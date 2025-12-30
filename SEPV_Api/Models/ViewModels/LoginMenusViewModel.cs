using System.Collections.Generic;

namespace Gp_Api.Models.ViewModels
{
    public class LoginMenusViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<object> Children { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public int Seq_no { get; set; }
    }

    public class LoginMenusFormViewModel
    {
        public int Id { get; set;}
        public string Name { get; set; }
        public List<LoginMenusViewModel> Menus { get; set; }
    }
}
