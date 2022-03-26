﻿using s3858853CCForumApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Datastore.V1;
using Google.Api.Gax;

namespace s3858853CCForumApp.Controllers
{
    //Mask URL
    [Route("/Login")]
    public class LoginController : Controller
    {

        public LoginController()
        {
            //DatastoreDb _context = DatastoreDb.Create("s3858853 - a1");
        }

        public IActionResult Login() => View();

        //attempt logging in
        [HttpPost]
        public async Task<IActionResult> Login(string loginID, string password)
        {
            //DatastoreDb _context = DatastoreDb.Create("s3858853-a1");
            DatastoreDb _context = new DatastoreDbBuilder
            {
                ProjectId = "s3858853-a1",
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.Build();


            //find login id
            var login = await _context.user.FindAsync(loginID);

            //attempt password check
            //password would typically be hashed
            if (login == null || login.password != password)
            {
                ModelState.AddModelError("LoginFailure", "ID or password is invalid");
                return View(new Login { LoginID = loginID });
            }

            //Customer login
            HttpContext.Session.SetInt32(nameof(User.id), login.id);
            HttpContext.Session.SetString(nameof(User.user_name), login.User.user_name);

            return RedirectToAction("Index", "User");
        }

        [Route("LoggingOut")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }




    }
}