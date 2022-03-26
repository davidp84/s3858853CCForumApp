using Microsoft.EntityFrameworkCore;
using s3858853CCForumApp.Models;

namespace s3858853CCForumApp.Data
{
    public class s3858853CCForumAppContext : DbContext
    {
        public s3858853CCForumAppContext(DbContextOptions<s3858853CCForumAppContext> options) : base(options)
        { }

        //Create Database sets for insertion
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }

    }
}
