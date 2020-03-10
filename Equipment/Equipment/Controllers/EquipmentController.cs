using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equipment.Models.Equipment;
using Equipment.Models.User;
using Equipment.Service.Equipment;
using Equipment.Service.User;
using Microsoft.AspNetCore.Mvc;

namespace Equipment.Controllers
{
    public class EquipmentController : BaseController
    {
        private readonly UserService _userService;
        private readonly EquipmentService _equipmentService;
        public EquipmentController()
        {
            _userService = new UserService();
            _equipmentService = new EquipmentService();
        }
        public IActionResult List()
        {
            return View();
        }

        public IActionResult Detail()
        {
            return View();
        }

        public IActionResult Update()
        {
            return View();
        }

        public IActionResult Add()
        {
            List<UserEntity> userEntities = _userService.GetUserList();
            ViewBag.UserList = userEntities;
            return View();
        }

        [HttpPost]
        public IActionResult RunAdd([FromBody]EquipmentAddModel equipmentAddModel)
        {
            if (!ModelState.IsValid)
                return new JsonResult("IsValid");
            var entity = equipmentAddModel.ConvertToEntity();
            entity.CreateUserId = Convert.ToInt64(HttpContext.Request.Cookies["UserId"]);
            int code = _equipmentService.AddEquipment(entity);
            return new JsonResult(code);
        }
    }
}