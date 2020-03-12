using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equipment.Core.Filters;
using Equipment.Service;
using Microsoft.AspNetCore.Mvc;

namespace Equipment.Controllers
{
    [AuthFilter]
    public abstract class BaseController : Controller
    {
        protected MySqlContext _dbContext = new MySqlContext();
    }
}