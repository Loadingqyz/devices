using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Equipment.Entities.Equipment;
using Equipment.Models.Equipment;
using Equipment.Models.User;
using Equipment.Service.Common;
using Equipment.Service.Equipment;
using Equipment.Service.User;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Equipment.Controllers
{
    public class EquipmentController : BaseController
    {
        private readonly UserService _userService;
        private readonly EquipmentService _equipmentService;
        private readonly QRCodeService _qRCodeService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public EquipmentController(IWebHostEnvironment hostingEnvironment, UserService userService, EquipmentService equipmentService, QRCodeService qRCodeService)
        {
            _userService = userService;
            _equipmentService = equipmentService;
            _hostingEnvironment = hostingEnvironment;
            _qRCodeService = qRCodeService;
        }
        public IActionResult List()
        {
            EquipmentQueryModel queryModel = new EquipmentQueryModel();
            if (!string.IsNullOrEmpty(Request.Query["page"]))
                queryModel.PageIndex = Convert.ToInt32(Request.Query["page"]);
            if (!string.IsNullOrEmpty(Request.Query["args"]))
                queryModel.QueryArgs = Request.Query["args"];
            EquipmentListModel equipmentResult = _equipmentService.GetEquipmentByPagination(queryModel);
            equipmentResult.MatchQueryArgs();
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

            ViewBag.QRCodeUrl = $"http{(HttpContext.Request.IsHttps?"s":"")}://{HttpContext.Request.Host}/Equipment/RunQRCode?eid={equipmentModel.EquipmentId}&isShowName=false&pixel=6"; 
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

        public IActionResult ShowQRCode(string eid)
        {
            ViewBag.QRCodeUrl = $"http{(HttpContext.Request.IsHttps ? "s" : "")}://{HttpContext.Request.Host}/Equipment/RunQRCode?eid={eid}";
            return View();
        }

        [HttpGet]
        public void RunQRCode(string eid,bool isShowName=true, int pixel=6)
        {
            string message = string.Empty;
            EquipmentEntity oldEntity = _equipmentService.GetEquipmentById(Convert.ToInt64(eid));
            if (oldEntity == null)
                message = "二维码异常";
            else
                message = oldEntity.EquipmentName;

            Response.ContentType = "image/jpeg";

            Bitmap bitmap = _qRCodeService.GetQRCode($"http://{Request.Host}/Equipment/detail?eid={eid}", pixel);
            Graphics g = null;
            if (isShowName)
            {
                int x = bitmap.Width;
                int y = bitmap.Height;
                g = Graphics.FromImage(bitmap);
                Font f = new Font("Verdana", 10, FontStyle.Bold);//字体  
                Brush b = new SolidBrush(Color.Red);//颜色  
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                g.DrawString($"{message}", f, b, x/2 - message.Length/2, y - 20, stringFormat);
            }

            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Jpeg);
            //bitmap.Dispose();
            //g?.Dispose();
            Response.Body.WriteAsync(ms.GetBuffer(), 0, Convert.ToInt32(ms.Length));
            Response.Body.Close();
        }

        [HttpGet]
        public IActionResult SVGQRCode(string eid)
        {
            EquipmentEntity oldEntity = _equipmentService.GetEquipmentById(Convert.ToInt64(eid));
            if (oldEntity == null)
                return RedirectToAction("System", "Error");

            var svgQrCode = _qRCodeService.GetSvgQRCode($"http://{Request.Host}/Equipment/detail?eid={eid}", 4);
            var rootPath = _hostingEnvironment.ContentRootPath;
            var svgName = $"{oldEntity.FixedAssetId}.svg";
            System.IO.File.WriteAllText($@"{rootPath}\{svgName}", svgQrCode);
            var readByte = System.IO.File.ReadAllBytes($@"{rootPath}\{svgName}");
            return File(readByte, "image/svg", svgName);
        }
    }
}