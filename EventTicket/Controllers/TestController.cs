﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;

namespace EventTicket.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PrintCharacter()
        {
            int unicode = 65;
            char character = (char)unicode;
            string text = character.ToString();
            Debug.WriteLine(text);
            return View();
        }
    }
}