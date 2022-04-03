using Google.Api.Gax;
using Google.Cloud.Datastore.V1;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using s3858853CCForumApp.Data;
using s3858853CCForumApp.Models;
using System.Linq;
using System.Text;
using System.Security.Claims;

namespace s3858853CCForumApp.Controllers
{
    public class UserController : Controller
    {

        private string UserID => HttpContext.Session.GetString("sessionID");

        private string UserName => HttpContext.Session.GetString("user_name");

        public UserController()
        {
        }

        public IActionResult Index()
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
                Filter = Filter.Equal("id", UserID)
            };

            //lazy loading
            var user = _context.RunQueryLazilyAsync(query);
            return View(user);
        }

        public async Task<IActionResult> User(int id)
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
                Filter = Filter.Equal("id", UserID)
            };

            //lazy loading
            var user = _context.RunQueryLazilyAsync(query);
            return View(user);
        }

        public async Task<IActionResult> ChangePassword()
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
                Filter = Filter.Equal("id", UserID)
            };

            var customer = _context.RunQueryLazilyAsync(query);
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string password, string newPassword)
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
                Filter = Filter.Equal("id", UserID)
            };

            var customer = _context.RunQueryLazilyAsync(query);

            //check password is not null
            if ((newPassword == null))
            {
                ModelState.AddModelError("PasswordError", "New Password is needed");
                return View(customer);
            }

            bool confirmed = false;

            await customer.ForEachAsync(x =>
            {
                //Then check original password, if correct update password
                if (x["password"].Equals(password))
                {
                    confirmed = true;
                    Entity update = new Entity
                    {
                        Key = x.Key,
                        ["id"] = x["id"],
                        ["user_name"] = x["user_name"],
                        ["password"] = password
                    };
                    _context.UpdateAsync(update);
                }
            });


            if (confirmed == true)
            {
                return RedirectToAction("Login", "User");
            }
            //finally if password not verified or another error, return error screen
            else
            {
                ModelState.AddModelError("PasswordError", "Old password is incorrect");
                return View(customer);
            }

        }

        public async Task<IActionResult> EditPost()
        {
            DatastoreDb _context = new DatastoreDbBuilder
            {
                ProjectId = "s3858853-a1",
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.Build();

            KeyFactory _keyFactory = _context.CreateKeyFactory("user");

            Key key = _keyFactory.CreateKey("default");

            Query query = new Query("post")
            {
                Filter = Filter.Equal("User", UserName)
            };

            var customer = _context.RunQueryLazilyAsync(query);
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(string subject, string messageText, string Image, DateTime postTimeUTC)
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
                Filter = Filter.Equal("User", UserName)
            };

            var customer = _context.RunQueryLazilyAsync(query);

            await customer.ForEachAsync(x =>
            {
                //Then check original password, if correct update password
                if (x["postTimeUTC"].Equals(postTimeUTC))
                {

                    Entity update = new Entity
                    {
                        Key = x.Key,
                        ["subject"] = subject,
                        ["messageText"] = messageText,
                        ["User"] = x["User"],
                        ["postTimeUTC"] = DateTime.Now,
                        ["Image"] = Image
                    };
                    _context.UpdateAsync(update);
                }
            });

            return RedirectToAction("Index", "Forum");

        }
       
    }
}
