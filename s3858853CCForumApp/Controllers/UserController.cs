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
using System.Collections.Generic;

namespace s3858853CCForumApp.Controllers
{
    public class UserController : Controller
    {

        private string UserID => HttpContext.Session.GetString("sessionID");

        private string UserName => HttpContext.Session.GetString("user_name");

        public UserController()
        {
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
            var entities = _context.RunQueryLazilyAsync(query);

            var queryUser = "";

            var tempUser = new User();

            await entities.ForEachAsync(x =>
            {
                
                queryUser = (string)x["id"];

                tempUser.id = (string)x["id"];
                tempUser.user_name = (string)x["user_name"];
                tempUser.password = (string)x["password"];
                tempUser.image = (string)x["image"];

            });

            ViewBag.user = tempUser;

            KeyFactory _newKeyFactory = _context.CreateKeyFactory("post");

            Key newKey = _keyFactory.CreateKey("default");

            Query secondQuery = new Query("Post")
            {
                Filter = Filter.Equal("UserID", queryUser)
            };

            //lazy loading
            var posts = _context.RunQueryLazilyAsync(secondQuery);

            var tempPost = new Post();

            var userPosts = new List<Post>();

            await posts.ForEachAsync(x =>
            {
                tempPost.subject = (string)x["subject"];
                tempPost.UserID = (string)x["UserID"];
                tempPost.messageText = (string)x["messageText"];
                tempPost.postTimeUTC = (string)x["postTimeUTC"];
                tempPost.Image = (string)x["Image"];

                userPosts.Add(tempPost);

            });

            return View(userPosts);
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

            var tempUser = new User();

            await customer.ForEachAsync(x =>
            {
                
                queryUser = (string)x["id"];

                tempUser.id = (string)x["id"];
                tempUser.user_name = (string)x["user_name"];
                tempUser.password = (string)x["password"];
                tempUser.image = (string)x["image"];

            });

            return View(tempUser);
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
                return RedirectToAction("Login", "Login");
            }
            //finally if password not verified or another error, return error screen
            else
            {
                ModelState.AddModelError("PasswordError", "Old password is incorrect");
                return View(tempUser);
            }

        }

        public async Task<IActionResult> UpdatePost()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePost(string subject, string messageText, string Image, DateTime postTimeUTC)
        {
            
            DatastoreDb _context = new DatastoreDbBuilder
            {
                ProjectId = "s3858853-a1",
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.Build();        
            
            KeyFactory _newKeyFactory = _context.CreateKeyFactory("post");

            Key newKey = _keyFactory.CreateKey("default");

            Query secondQuery = new Query("Post")
            {
                Filter = Filter.Equal("postTimeUTC", postTimeUTC)
            };

            //lazy loading
            var posts = _context.RunQueryLazilyAsync(secondQuery);

            var tempPost = new Post();

            var userPosts = new List<Post>();

            await posts.ForEachAsync(x =>
            {
                tempPost.subject = (string)x["subject"];
                tempPost.UserID = (string)x["UserID"];
                tempPost.messageText = (string)x["messageText"];
                tempPost.postTimeUTC = (string)x["postTimeUTC"];
                tempPost.Image = (string)x["Image"];

                userPosts.Add(tempPost);

            });

            ViewBag.post = userPosts;

            return RedirectToAction("User", "EditPost");
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

            return RedirectToAction("Forum", "Forum");

        }
       
    }
}
