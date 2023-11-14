using App.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace App.Pages.Account
{
    public class RegisterModel : PageModel
    
       {
private readonly ILogger _logger;
        private readonly appContext _context;

        [BindProperty]
            public ApplicationUser ApplicationUser { get; set; }

            public RegisterModel(ILogger<RegisterModel> logger, appContext context)
            {
                _logger = logger;
                _context = context;
            }

            public IActionResult OnGet()
            {
                return Page();
            }

            public async Task<IActionResult> OnPostAsync()
            {
                // Check if the username is already taken
                var existingUser = await _context.UserLogins.FirstOrDefaultAsync(l => l.Username == ApplicationUser.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Username already exists.");
                    return Page();
                }

                // Create a new user
                var newUser = new ApplicationUser
                {
                    Username = ApplicationUser.Username,
                    Password = ApplicationUser.Password,
                    Email = ApplicationUser.Email,
                    Role = "User"
                };

                _context.UserLogins.Add(newUser);
                await _context.SaveChangesAsync();

                // The user is authenticated
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, newUser.Username),
            new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
            new Claim(ClaimTypes.Name, newUser.Email),
            new Claim(ClaimTypes.Role, newUser.Role)
        };

                var claimsIdentity = new ClaimsIdentity(claims, "LocalLogin");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync("LocalLogin", new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("");
            }
        }

    }

