using Equipment.Models.User;
using Equipment.Service.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;

namespace Equipment.Core.Filters
{
	public class AuthFilter : Attribute, IAuthorizationFilter
	{
		private readonly UserService _userService;
		private List<string> _loginUrl = new List<string>()
		{
			"/system/runlogin",
			"/system/login",
			 "/",
			""
		};
		/// <summary>
		/// 管理员才能访问的URL
		/// </summary>
		private List<string> _adminDomain = new List<string>()
		{
			"user"
		};
		public AuthFilter()
		{
			_userService = new UserService();
		}
		
		public void OnAuthorization(AuthorizationFilterContext context)
		{
			bool isAuth = false;
			var cookies=context.HttpContext.Request.Cookies;
			string requestUrl = context.HttpContext.Request.Path.Value.ToLower();
			if (cookies.ContainsKey("AuthInfo") && cookies.ContainsKey("UserName") && cookies.ContainsKey("UserId"))
			{
				UserEntity userEntity = _userService.GetUserById(cookies["UserId"]);
				if (userEntity != null)
				{ 
					string auth = _userService.GenerateAuthInfo(userEntity);
					if (auth == cookies["AuthInfo"])
					{
						string requestDomain = requestUrl.Split("/")[1];
						if (userEntity.IsSuperAdmin != 0 && _adminDomain.Contains(requestDomain)) //没有管理员权限的一律跳转到设备列表
						{
							context.Result = new RedirectResult("/Equipment/List");
						}

						//更新cookie生命周期
						CookieOptions cookieOptions = new CookieOptions()
						{
							Expires = DateTime.Now.AddDays(7)
						};
						context.HttpContext.Response.Cookies.Append("UserId", userEntity.Id.ToString(), cookieOptions);
						context.HttpContext.Response.Cookies.Append("UserName", userEntity.UserName, cookieOptions);
						context.HttpContext.Response.Cookies.Append("AuthInfo", auth, cookieOptions);
						context.HttpContext.Items.Add($"UserId_{userEntity.Id}", userEntity);
						isAuth = true;
					}
				}
			}

			if (!isAuth)//没有通过验证
			{
				if (!_loginUrl.Contains(requestUrl))
				{
					context.HttpContext.Response.Cookies.Append("RedirectUrl", context.HttpContext.Request.GetDisplayUrl(), new CookieOptions()
					{
						Expires = DateTime.Now.AddDays(7)
					});
					context.Result = new RedirectResult("/System/Login");
				}
			}
			else 
			{
				if (_loginUrl.Contains(requestUrl))
				{
					context.Result = new RedirectResult("/Equipment/List");
				}
			}
		}
	}
}
