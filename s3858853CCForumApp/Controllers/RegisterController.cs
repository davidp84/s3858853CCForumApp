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
    [Route("/Register")]
    public class RegisterController : Controller
    {


        public IActionResult Register() => View();

        //attempt user registration
        [HttpPost]
        public async Task<IActionResult> Register(string loginID, string username, string password)
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
                Filter = Filter.Equal("id", loginID)
            };
            //find login id
            var login = _context.RunQueryLazilyAsync(query);

            if (login != null)
            {
                ModelState.AddModelError("RegistrationFailure", "The ID already exists");
                return View();
            }

            Query usernameQuery = new Query("user")
            {
                Filter = Filter.Equal("user_name", username)
            };

            //find username
            var userName = _context.RunQueryLazilyAsync(usernameQuery);

            if (userName != null)
            {
                ModelState.AddModelError("RegistrationFailure", "The username already exists");
                return View();
            }

            Entity user = new Entity()
            {
                Key = _context.CreateKeyFactory("user").CreateKey("default"),
                ["id"] = loginID,
                ["user_name"] = username,
                ["password"] = password
            };

            await _context.InsertAsync(user);

            // File uploaded to Cloud Storage

            return RedirectToAction("Login", "User");

        }

    }
}
