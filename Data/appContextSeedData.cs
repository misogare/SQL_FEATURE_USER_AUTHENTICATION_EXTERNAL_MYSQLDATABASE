
namespace App.Data
{


    public class appContextSeedData
    {
        public static void Seed(appContext context)
        {
            context.Database.EnsureCreated();
            context.UserLogins.Add(new ApplicationUser
            {
                Username = "admin",
                Password = "password",
                Email = "admin@example.com",
                FirstName = "Admin",
                LastName = "User",
                Role = "Admin"
            });

            context.SaveChanges();
        }
    }

}