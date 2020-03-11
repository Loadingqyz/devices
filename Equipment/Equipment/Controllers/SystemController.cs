using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equipment.Models.User;
using Equipment.Service.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Equipment.Controllers
{
    public class SystemController : BaseController
    {
        private readonly UserService _userService;

        public SystemController()
        {
            _userService = new UserService();
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RunLogin([FromBody]UserLoginModel userLoginModel)
        {
            if (!ModelState.IsValid)
                return new JsonResult("IsValid");

            var userEntity = _userService.CheckUserLogin(userLoginModel);
            if (userEntity == null)
            {
                return new JsonResult("用户名或密码错误");
            }
            UserLoginResultModel resultModel = new UserLoginResultModel();
            resultModel.AuthInfo = _userService.GenerateAuthInfo(userEntity);
            resultModel.UserName = userEntity.UserName;
            resultModel.UserId = userEntity.Id.ToString();

            if(HttpContext.Request.Cookies.ContainsKey("RedirectUrl"))
                resultModel.RedirectUrl = HttpContext.Request.Cookies["RedirectUrl"];


            CookieOptions cookieOptions = new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(7)
            };
            HttpContext.Response.Cookies.Append("UserId", resultModel.UserId, cookieOptions);
            HttpContext.Response.Cookies.Append("UserName", resultModel.UserName, cookieOptions);
            HttpContext.Response.Cookies.Append("AuthInfo", resultModel.AuthInfo, cookieOptions);

            return new JsonResult(resultModel);
        }

        public IActionResult LoginOut()
        {
            HttpContext.Response.Cookies.Delete("UserId");
            HttpContext.Response.Cookies.Delete("UserName");
            HttpContext.Response.Cookies.Delete("AuthInfo");
            HttpContext.Response.Redirect("/System/Login");
            return new JsonResult("ok");
        }
    }
}