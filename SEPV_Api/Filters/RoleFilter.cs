using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Gp_Api.IServices;
using System;
using System.Linq;

namespace Gp_Api.Middlewares
{

    public class RoleFilterAttribute : TypeFilterAttribute
    {
        public RoleFilterAttribute(int menuId) : base(typeof(RoleFilter))
        {
            Arguments = new object[] { menuId };
        }
    }
    public class RoleFilter : IAuthorizationFilter
    {
        private readonly ILoginMenusService _menuService;
        private readonly int _menuId;

        public RoleFilter (int menuId, ILoginMenusService menuService)
        {
            _menuId = menuId;
            _menuService = menuService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user_id = context.HttpContext.User.Claims.FirstOrDefault(t => t.Type == "user_id").Value;
            var role_id = context.HttpContext.User.Claims.FirstOrDefault(t => t.Type == "role_id").Value;
            var book_id = context.HttpContext.User.Claims.FirstOrDefault(t => t.Type == "book_id").Value;
            if (user_id == null || role_id == null || book_id == null) context.Result = new UnauthorizedObjectResult(new { a = user_id, b = role_id });
            var result = false;
            _menuService.BookId = short.Parse(book_id);
            var menus = _menuService.GetRoleMenus(int.Parse(user_id), int.Parse(role_id));
            foreach (var menu in menus)
            {
                if (menu.Id == _menuId)
                {
                    result = true;
                    break;
                }
            }
            if (!result) context.Result = new UnauthorizedObjectResult(new { result="Permission Denied" });
        }
    }
}
