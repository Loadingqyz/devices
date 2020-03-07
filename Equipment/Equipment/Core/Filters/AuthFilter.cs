using Equipment.Models.User;
using Equipment.Service.User;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Equipment.Core.Filters
{
	public class AuthFilter : Attribute,IAuthorizationFilter
	{
		private readonly UserService _userService;
		private List<string> _url = new List<string>()
				{
					"/user/runlogin",
					"/user/login",
					 "/",
					 ""
				};
		public AuthFilter()
		{
			_userService = new UserService();
		}
		public void OnAuthorization(AuthorizationFilterContext context)
		{
			bool isRedirect = true;
			var cookies=context.HttpContext.Request.Cookies;

			if (cookies.ContainsKey("AuthInfo") && cookies.ContainsKey("UserName") && cookies.ContainsKey("UserId"))
			{
				UserEntity userEntity = _userService.GetUserById(cookies["UserId"]);
				if (userEntity != null)
				{ 
					string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userEntity.UserName}:{userEntity.Password}"));
					if (auth == cookies["AuthInfo"])
					{
						isRedirect = false;
					}
				}
			}

			if (isRedirect)
			{
				if (!_url.Contains(context.HttpContext.Request.Path.Value.ToLower()))
					context.HttpContext.Response.Redirect("/User/Login");
			}
			else 
			{
				if (_url.Contains(context.HttpContext.Request.Path.Value.ToLower()))
				{
					context.HttpContext.Response.Redirect("/Equipment/List");
				}
			}
		}
	}
}
