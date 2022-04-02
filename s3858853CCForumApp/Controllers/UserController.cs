using Google.Api.Gax;
using Google.Cloud.Datastore.V1;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;
using s3858853CCForumApp.Data;
using s3858853CCForumApp.Models;
using System.Linq;
using System.Text;
using System.Security.Claims;

namespace s3858853CCForumApp.Controllers
{
    public class UserController : Controller
    {
        //private readonly s3858853CCForumAppContext _context;

        private string UserID => HttpContext.Session.GetString("sessionID");

        private string UserName => HttpContext.Session.GetString("user_name");

        //public UserController(s3858853CCForumAppContext context)
        //{
        //    _context = context;
        //}


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
            var customer = _context.RunQueryLazilyAsync(query);
            return View();
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

            return RedirectToAction("Forum", "User");

        }

        public async Task<IActionResult> NewPost()
        {
            DatastoreDb _context = new DatastoreDbBuilder
            {
                ProjectId = "s3858853-a1",
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.Build();

            KeyFactory _keyFactory = _context.CreateKeyFactory("post");

            Key key = _keyFactory.CreateKey("default");

            Query query = new Query("post")
            {
                Filter = Filter.Equal("User", UserName)
            };

            var customer = _context.RunQueryLazilyAsync(query);
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> NewPost(string subject, string messageText, string imageName, IFormFile image, string imagePath)
        {
            DatastoreDb _context = new DatastoreDbBuilder
            {
                ProjectId = "s3858853-a1",
                EmulatorDetection = EmulatorDetection.EmulatorOrProduction
            }.Build();

            KeyFactory _keyFactory = _context.CreateKeyFactory("post");

            Key key = _keyFactory.CreateKey("default");

            string imageString = "gs://s3858853-a1-storage/" + imageName;


            //var fileToTransfer = File.OpenRead(image);

            //var content = Encoding.UTF8.GetBytes(image);

            //using (var f = File.Open(imagePath, FileAccess.Read))
            //{
            //    string objectName = imageName;
            //    client.UploadObjectAsync(bucket, objectName, null, f);
            //}


            // Prepare bucket and image for upload

            var client = StorageClient.Create();

            var bucket = client.GetBucketAsync("s3858853-a1-storage");

            var content = new FileStream(imagePath, FileMode.Open, FileAccess.Read);


            // Upload file to bucket
            if (imageName.Contains(".png"))
            {
                var obj1 = client.UploadObjectAsync("s3858853-a1-storage", imageName, "image/png", content);
            }
            else
            {
                var obj1 = client.UploadObjectAsync("s3858853-a1-storage", imageName, "image/jpg", content);
            }

            Entity update = new Entity
            {
                Key = key,
                ["subject"] = subject,
                ["messageText"] = messageText,
                ["User"] = UserID,
                ["postTimeUTC"] = DateTime.Now,
                ["Image"] = imageString
            };
            await _context.InsertAsync(update);

            return RedirectToAction("Forum", "User");

        }
    }
}
