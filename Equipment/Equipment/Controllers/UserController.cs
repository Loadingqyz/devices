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
            var entity = new UserEntity()
            {
                Password = userInfoModel.Password,
                Phone = userInfoModel.Phone,
                UserName = userInfoModel.UserName,
                CreateUserId = Convert.ToInt64(HttpContext.Request.Cookies["UserId"]),
                IsSuperAdmin = userInfoModel.IsSuperAdmin.Value
            };
            int code = _userService.AddUser(entity);
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

        public IActionResult Update(string userId)
        {
            UserEntity userEntity= _userService.GetUserById(userId);
            ViewBag.User = userEntity;
            return View();
        }

        [HttpPost]
        public IActionResult RunUpdate([FromBody]UserUpdateModel userUpdateModel)
        {
            int code = _userService.UpdateUser(userUpdateModel);
            if(code==-1)
                return new JsonResult("该手机号码已经被使用！");
            return new JsonResult(code);
        }

        
    }
}