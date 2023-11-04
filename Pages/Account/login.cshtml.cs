using App.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace App.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly appContext _context;


        public LoginModel(ILogger<LoginModel> logger, appContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public ApplicationUser? ApplicationUser { get; set; }


        public IActionResult OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {

            var userLogin = await _context.UserLogins.FirstOrDefaultAsync(l => l.Email == ApplicationUser.Email || l.Password == ApplicationUser.Password);
            if (userLogin == null)
            {
                ModelState.AddModelError("Email", "Invalid email address.");
                return Page();
            }

            if (userLogin.Password != ApplicationUser.Password)
            {
                ModelState.AddModelError("Password", "Invalid password.");
                return Page();
            }

            // The user is authenticated
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, userLogin.Username),
        // Add other claims as needed
    };

            var claimsIdentity = new ClaimsIdentity(claims, "LocalLogin");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // You can set this based on your requirements
            };

            await HttpContext.SignInAsync("LocalLogin", new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Index", "Home");


            // The user is not authenticated

        }
    }
}
