using s3858853CCForumApp.Models;
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

        }

        public IActionResult Login() => View();

        //attempt logging in
        [HttpPost]
        public async Task<IActionResult> Login(string id, string Password)
        {
            DatastoreDb _context = new DatastoreDbBuilder
            {
                ProjectId = "s3858853-a1",
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.Build();

            KeyFactory _keyFactory = _context.CreateKeyFactory("user");

            Key key = _keyFactory.CreateKey("default");

            Query query = new Query("user")
            {
                Filter = Filter.Equal("id", id)
            };

            //find login id
            var login = _context.RunQueryLazilyAsync(query);

            var loginCheck = true;
            bool confirmed = false;
            var customer = "No Customer";

            await login.ForEachAsync(x =>
            {
                //attempt password check
                //password would typically be hashed
                if (x == null)
                {
                    loginCheck = false;
                }
                
                if (x["password"].Equals(Password))
                {
                    confirmed = true;
                    customer = (string)x["user_name"];
                }

            });

            if (loginCheck == false)
            {
                ModelState.AddModelError("LoginFailure", "ID or password is invalid");
                return View(new Login { id = id });
            }

            if (confirmed == false)
            {
                ModelState.AddModelError("LoginFailure", "ID or password is invalid");
                return View(new Login { id = id });
            }

            //customer login
            HttpContext.Session.SetString("sessionID", id);
            HttpContext.Session.SetString("username", customer);

            return RedirectToAction("Forum", "Forum");
        }

        [Route("LoggingOut")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }




    }
}