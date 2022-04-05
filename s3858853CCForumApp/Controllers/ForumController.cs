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
    public class ForumController : Controller
    {
        private string UserID => HttpContext.Session.GetString("sessionID");

        private string UserName => HttpContext.Session.GetString("user_name");

        public ForumController()
        {
        }

        public async Task<IActionResult> Forum(int id)
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

            var tempUser = new User();

            var queryUser = "";

            await user.ForEachAsync(x =>
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
                Limit = 10,
                Order = { { "created", PropertyOrder.Types.Direction.Descending } }
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

        public async Task<IActionResult> NewPost()
        {
            DatastoreDb _context = new DatastoreDbBuilder
            {
                ProjectId = "s3858853-a1",
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.Build();

            KeyFactory _keyFactory = _context.CreateKeyFactory("post");

            //Key key = _keyFactory.CreateKey("default");

            Query query = new Query("post")
            {
                Filter = Filter.Equal("User", UserName)
            };

            var customer = _context.RunQueryLazilyAsync(query);
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> NewPost(string subject, string messageText, IFormFile Image)
        {
            DatastoreDb _context = new DatastoreDbBuilder
            {
                ProjectId = "s3858853-a1",
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.Build();

            KeyFactory keyFactory = _context.CreateKeyFactory("post");

             var imageName = Image.FileName;

            string imageString = "gs://s3858853-a1-storage/" + imageName;

            // Prepare bucket and image for upload

            var client = StorageClient.Create();

            var bucket = client.GetBucketAsync("s3858853-a1-storage");

            var obj1 = "a";

            if (imageName.Contains(".png"))
            {
                obj1 = "image/png";
            }
            else
            {
                obj1 = "image/jpg";
            }

            // Upload file to bucket
            using (var memoryStream = new MemoryStream())
            {
                await Image.CopyToAsync(memoryStream);
                var dataObject = await client.UploadObjectAsync("s3858853-a1-storage", imageName, obj1, memoryStream);                
            }

            Entity update = new Entity
            {
                Key = keyFactory.CreateIncompleteKey(),
                ["subject"] = subject,
                ["messageText"] = messageText,
                ["UserID"] = UserID,
                ["postTimeUTC"] = DateTime.Now,
                ["Image"] = imageString
            };
            await _context.InsertAsync(update);

            return RedirectToAction("Forum", "Forum");

        }
    }
}
