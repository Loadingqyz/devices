using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Equipment.Controllers
{
    public class CommonController : BaseController
    {
        public IActionResult QRCode(string eids)
        {
            if(string.IsNullOrEmpty(eids))
                RedirectToAction("System", "Error");
            var eidList = eids.Split(',').ToList();

            List<string> url = new List<string>();
            foreach (var eid in eidList)
            {
                url.Add($"http{(HttpContext.Request.IsHttps ? "s" : "")}://{HttpContext.Request.Host}/Equipment/RunQRCode?eid={eid}&pixel=6");
            }

            ViewBag.QRCodeUrls = url;

            return View();
        }
    }
}