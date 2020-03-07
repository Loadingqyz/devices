using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Equipment.Controllers
{
    public class EquipmentController : BaseController
    {
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
            return View();
        }
    }
}