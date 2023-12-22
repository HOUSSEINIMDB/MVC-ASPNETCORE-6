using Microsoft.AspNetCore.Mvc;
using WebMVCapp1.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebMVCapp1.Models;

[Controller]
public class AccessController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<AccessController> _logger;

    public AccessController(AppDbContext context, ILogger<AccessController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private readonly string filePath = "accessLogFile.log";

    [HttpGet]
    public IActionResult Login()
    {
        ClaimsPrincipal claimUser = HttpContext.User;
        if (claimUser.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home");
        return View();
    }
    
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(Client client)
    {
        if (client.Login =="admin")
        {
            ModelState.AddModelError("Client.Login", "You cannot call yourself admin. Please choose a different login.");
            return View("Register", client);
        }
        if (_context.Clients.Any(c => c.Login == client.Login))
        {
            ModelState.AddModelError("Client.Login", "Login already exists. Please choose a different login.");
            return View("Register", client);
        }
        if (ModelState.IsValid)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        // Errors
        return View("Register", client);
    }



    [HttpPost]
    public async Task<IActionResult> Login(Client Client)
    {
        if (Client.Login != "admin")
        {
            var user = await _context.Clients
                .FirstOrDefaultAsync(u => u.Login == Client.Login && u.Password == Client.Password);
            if (user != null)
            {
                List<Claim> claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, Client.Login),
                    new("Role", "User")
                };
                ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = true,
                    //ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), properties);
                System.IO.File.AppendAllText(filePath, $"{DateTime.UtcNow.ToLongTimeString()} - Listing games  in shop By client {Client.Login}\n");
                return RedirectToAction("Index", "Home", new {clientId = user.ClientId});
            }

            
        }
        else//Cas du Admin
        {
            string adminPassword = System.IO.File.ReadAllText(".secrets").Trim();
            if(Client.Password == adminPassword){
                List<Claim> claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, Client.Login),
                    new("Role", "Admin")
                };
            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = true
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), properties);
            System.IO.File.AppendAllText(filePath, $"{DateTime.UtcNow.ToLongTimeString()} - Site Admin Access\n");
            return RedirectToAction("Index", "Home");
            }
            
        }
        return View();
        }
    
    //if (Client.Login == "placeholder" && Client.Password == "123")
    
}