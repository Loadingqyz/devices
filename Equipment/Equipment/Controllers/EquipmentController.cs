using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equipment.Entities.Equipment;
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
            
            EquipmentQueryModel queryModel = new EquipmentQueryModel();
            if (!string.IsNullOrEmpty(Request.Query["page"]))
                queryModel.PageIndex = Convert.ToInt32(Request.Query["page"]);
            EquipmentListModel equipmentResult = _equipmentService.GetEquipmentByPagination(queryModel);
            ViewBag.Equipment = equipmentResult;
            return View();
        }

        [HttpPost]
        public IActionResult RunList([FromBody] EquipmentQueryModel queryMode)
        {
            EquipmentListModel equipmentResult = _equipmentService.GetEquipmentByPagination(queryMode);
            return new JsonResult(equipmentResult);
        }

        public IActionResult Update()
        {
            EquipmentEntity equipmentEntity = _equipmentService.GetEquipmentById(Convert.ToInt64(Request.Query["eid"]));
            if (equipmentEntity == null)
                return RedirectToAction("List", "Equipment");
            EquipmentUpdateViewModel equipmentModel = new EquipmentUpdateViewModel()
            {
                EquipmentName = equipmentEntity.EquipmentName,
                FixedAssetId = equipmentEntity.FixedAssetId,
                LastMaintenanceNames = equipmentEntity.LastMaintenanceNames,
                LastMaintenanceTime = equipmentEntity.LastMaintenanceTime,
                LastMaintenanceIds = equipmentEntity.LastMaintenanceIds,
                Remark = equipmentEntity.Remark,
                ScrapDate = equipmentEntity.ScrapDate,
                EquipmentId = equipmentEntity.Id
            };
            ViewBag.Equipment = equipmentModel;
            List<UserEntity> userEntities = _userService.GetUserList();
            ViewBag.UserList = userEntities;
            return View();
        }

        [HttpPost]
        public IActionResult RunUpdate([FromBody]EquipmentUpdateModel equipmentUpdateModel)
        {
            if (!ModelState.IsValid)
                return new JsonResult("IsValid");
            EquipmentEntity oldEntity = _equipmentService.GetEquipmentById(Convert.ToInt64(equipmentUpdateModel.EquipmentId));
            if(oldEntity==null)
                return new JsonResult("该设备不存在，可能已经被删除了");

            EquipmentEntity entity = equipmentUpdateModel.ConvertToEntity();
            entity.UpdateUserId = Convert.ToInt64(HttpContext.Request.Cookies["UserId"]);
            int code = _equipmentService.UpdateEquipment(entity);
            return new JsonResult(code);
        }

        public IActionResult Detail()
        {
            EquipmentEntity equipmentEntity = _equipmentService.GetEquipmentById(Convert.ToInt64(Request.Query["eid"]));
            if (equipmentEntity == null)
                return RedirectToAction("List", "Equipment");
            EquipmentDetailViewModel equipmentModel = new EquipmentDetailViewModel()
            {
                EquipmentName = equipmentEntity.EquipmentName,
                FixedAssetId = equipmentEntity.FixedAssetId,
                LastMaintenanceNames = equipmentEntity.LastMaintenanceNames,
                LastMaintenanceTime = equipmentEntity.LastMaintenanceTime,
                LastMaintenanceIds = equipmentEntity.LastMaintenanceIds,
                Remark = equipmentEntity.Remark,
                ScrapDate = equipmentEntity.ScrapDate,
                EquipmentId = equipmentEntity.Id
            };
            ViewBag.Equipment = equipmentModel;
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

        [HttpPost]
        public IActionResult RunDelete(string eid)
        {
            if (string.IsNullOrEmpty(eid))
                return new JsonResult("IsValid");
            EquipmentEntity oldEntity = _equipmentService.GetEquipmentById(Convert.ToInt64(eid));
            if (oldEntity == null)
                return new JsonResult("该设备不存在，可能已经被删除了");

            EquipmentEntity entity = new EquipmentEntity()
            {
                Id = Convert.ToInt64(eid),
                UpdateUserId = Convert.ToInt64(HttpContext.Request.Cookies["UserId"]),
                IsDelete = 1
            };
            int code = _equipmentService.UpdateEquipment(entity,"删除设备");
            return new JsonResult(code);
        }
    }
}