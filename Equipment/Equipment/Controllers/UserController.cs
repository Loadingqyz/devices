using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Equipment.Models.User;
using Equipment.Service.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Equipment.Controllers
{
    public class UserController : BaseController
    {
        private readonly UserService _userService;

        public UserController()
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
            resultModel.AuthInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userEntity.UserName}:{userEntity.Password}"));
            resultModel.UserName = userEntity.UserName;
            resultModel.UserId = userEntity.Id.ToString();

            CookieOptions cookieOptions = new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(7)
            };
            HttpContext.Response.Cookies.Append("UserId", resultModel.UserId, cookieOptions);
            HttpContext.Response.Cookies.Append("UserName", resultModel.UserName, cookieOptions);
            HttpContext.Response.Cookies.Append("AuthInfo", resultModel.AuthInfo, cookieOptions);

            return new JsonResult(resultModel);
        }

        public IActionResult List()
        {
            ViewBag.UserList = _userService.GetUserList();
            return View();
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RunAdd([FromBody]UserInfoModel userInfoModel)
        {
            if (!ModelState.IsValid)
                return new JsonResult("IsValid");
            int code = _userService.AddUser(new UserEntity() { Password = userInfoModel.Password, Phone = userInfoModel.Phone, UserName = userInfoModel.UserName });
            return new JsonResult(code);
        }

        [HttpPost]
        public IActionResult RunDelete(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new JsonResult("IsValid");
            int code = _userService.DeleteUser(userId);
            return new JsonResult(code);
        }

        public IActionResult Update()
        {
            return View();
        }

        public IActionResult LoginOut()
        {
            HttpContext.Response.Cookies.Delete("UserId");
            HttpContext.Response.Cookies.Delete("UserName");
            HttpContext.Response.Cookies.Delete("AuthInfo");
            HttpContext.Response.Redirect("/User/Login");
            return new JsonResult("ok");
        }
    }
}