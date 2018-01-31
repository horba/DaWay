﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;



namespace InstaBotPrototype.UI.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        [Route("/")]
        [HttpGet]
         public ActionResult Index()
         {
            //HttpContext.Session.SetInt32("user_id", 2);
            bool aval = HttpContext.Session.IsAvailable;
            int? id = HttpContext.Session.GetInt32("user_id");
            if (aval && id != null)
            {
                //return Content("User id is " + HttpContext.Session.GetInt32("user_id"));
                return Redirect("/home");
            }
            return Redirect("/login");
         }
		 
        [Route("/home")]
        public IActionResult Home()
        {
            return View();
        }
        [Route("/exit")]
        public IActionResult Exit()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }
    }
}