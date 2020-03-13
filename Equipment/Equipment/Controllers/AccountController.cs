using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equipment.Models.User;
using Equipment.Service;
using Equipment.Service.User;
using Microsoft.AspNetCore.Mvc;

namespace Equipment.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserService _userService;       
        public AccountController(UserService userService)
        {
            _userService = userService;
        }
        public IActionResult Info()
        {
           UserEntity userEntity = _userService.GetUserById(HttpContext.Request.Cookies["UserId"]);
            ViewBag.User = userEntity;
            return View();
        }


        [HttpPost]
        public IActionResult RunInfoUpdate([FromBody]UserUpdateModel userUpdateModel)
        {
            userUpdateModel.UserId = HttpContext.Request.Cookies["UserId"];
            int code = _userService.UpdateUser(userUpdateModel);
            if (code == -1)
                return new JsonResult("该手机号码已经被使用！");
            return new JsonResult(code);
        }

        public IActionResult Password()
        {
            return View();
        }
    }
}