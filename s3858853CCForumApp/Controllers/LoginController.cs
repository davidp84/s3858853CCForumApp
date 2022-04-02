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

            KeyFactory _keyFactory = _context.CreateKeyFactory("user");

            Key key = _keyFactory.CreateKey("default");

            Query query = new Query("user")
            {
                Filter = Filter.Equal("id", loginID)
            };

            //find login id
            var login = _context.RunQueryLazilyAsync(query);
            //attempt password check
            //password would typically be hashed
            if (login == null)
            {
                ModelState.AddModelError("LoginFailure", "ID or password is invalid");
                return View(new Login { id = loginID });
            }

            bool confirmed = false;
            var customer = "No Customer";

            await login.ForEachAsync(x =>
            {
                if (x["password"].Equals(password))
                {
                    confirmed = true;
                    customer = (string)x["user_name"];
                }
            });

            if (confirmed == false)
            {
                ModelState.AddModelError("LoginFailure", "ID or password is invalid");
                return View(new Login { id = loginID });
            }

            //customer login
            HttpContext.Session.SetString(loginID, loginID);
            HttpContext.Session.SetString(customer, customer);

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