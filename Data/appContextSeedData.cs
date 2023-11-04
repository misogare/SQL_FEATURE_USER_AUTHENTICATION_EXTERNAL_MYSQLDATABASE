
namespace App.Data
{


    public class appContextSeedData
    {
        public static void Seed(appContext context)
        {
            if (context.UserLogins.Any())
            {
                return;   // DB has been seeded

            }
            context.Database.EnsureCreated();
            context.UserLogins.AddRange
                (new ApplicationUser
            {
                Username = "admin",
                Password = "password",
                Email = "admin@example.com",
                FirstName = "Admin",
                LastName = "User",
                Role = "Admin"
            },
            new ApplicationUser()
            {
                Username = "User",
                Password = "password",
                Email = "User@example.com",
                FirstName = "User",
                LastName = "1234",
                Role = "User"


            });

            context.SaveChanges();
        }
    }

}