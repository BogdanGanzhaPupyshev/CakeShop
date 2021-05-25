using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CakeStore.Viewcs;
using CakeStore.Repositories;
using CakeStore.Models;
using CakeStore.Data;

namespace CakeStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
     
        public IActionResult Index()
        {
            return View();
        }

    }
}