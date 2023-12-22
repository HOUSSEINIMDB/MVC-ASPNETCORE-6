using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMVCapp1.Models;
using WebMVCapp1.Data;
[Controller]
public class GameController : Controller//le dossier de vue sera obligatoirement 'Game', ainsi que le nom du controleur
{
    private readonly AppDbContext _context;
    private readonly ILogger<GameController> _logger;//logging

    public GameController(AppDbContext context,ILogger<GameController> logger)
    {
        _context = context;
        _logger = logger;
    }
    string filePath = "logfile.log";

    public IActionResult AddGame()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AddGame(Game game)
    {
        if (ModelState.IsValid)
        {
            // Add to the database
            _context.Games.Add(game);
            _context.SaveChanges();

            return RedirectToAction("GameAdded", new { id = game.GameId });
        }

        return View(game);
    }

    public IActionResult Gameadded(int id)
    {  //Confirmation
        var addedGame = _context.Games.FirstOrDefault(g => g.GameId == id);
        if (addedGame == null)
            return NotFound();
        return View(addedGame);
    }
    public IActionResult GamesList()
    {
        var games = _context.Games.ToList();
        //logging
        _logger.LogInformation("Listing games logging at {DT}"
            ,DateTime.UtcNow.ToLongTimeString());
        
        System.IO.File.AppendAllText(filePath, $"{DateTime.UtcNow.ToLongTimeString()} - Admin Listing games logging\n");
        return View(games);
    }
    [HttpGet]
    public IActionResult EditGame(int id)
    {
        var game = _context.Games.Find(id);
        if (game == null)
            return NotFound();
        return View(game);
    }

    [HttpPost]
    public IActionResult EditGame(Game game)
    {
        if (ModelState.IsValid)
        {
            _context.Update(game);
            _context.SaveChanges();
            return RedirectToAction("Gameadded", new { id = game.GameId });
        }
        return View(game);
    }
    //Game Item
    public IActionResult AddGameItem()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AddGameItem(GameItem gameItem)
    {
        if (ModelState.IsValid)
        {
            var associatedGame = _context.Games.FirstOrDefault(g => g.GameId == gameItem.GameId);
            if (associatedGame != null)
            {
                associatedGame.GameItems.Add(gameItem);
                _context.SaveChanges();
                //On retourne à la liste on a reussi
                return RedirectToAction("GameItemList");
            }
            //Si model invalide donné  au formulaire
            ModelState.AddModelError(string.Empty, "Associated game not found.");
        }
        //On retourne avec les messages d'erreurs
        return View(gameItem);
    }

    //Liste des Games Items
    public IActionResult GameItemList()
    {
        var gameItems = _context.GameItems.ToList();
        return View(gameItems);
    }
    
    public IActionResult UserStore(int? clientId)
    {
        if (clientId.HasValue)
        {
            ViewData["ClientId"] = clientId.Value;
        }
        //log
        var games = _context.Games.ToList();
        _logger.LogInformation("Listing games To shop logging at {dt}"
            ,DateTime.UtcNow.ToLongTimeString());
        System.IO.File.AppendAllText(filePath, $"{DateTime.UtcNow.ToLongTimeString()} - Listing games  in shop By client {clientId}\n");
        //page
        return View(games);
    }


public IActionResult Buy(int gameId, int? clientId)
{
        // Check if the client is authenticated
        if (!User.Identity.IsAuthenticated)
        {
            // Handle the case where the user is not authenticated (not logged in)
            return RedirectToAction("Login", "Access");
        }
        
        // Check if the game item is available
        var availableGameItem = _context.GameItems
            .FirstOrDefault(item => item.GameId == gameId && item.CartId == null);

        if (availableGameItem == null)
        {
            return RedirectToAction("GameNotAvailable", "Game");
        }

             var client = _context.Clients
            .Include(c => c.CartofCient)
            .FirstOrDefault(c => c.ClientId == clientId);

            if (client.CartofCient == null)
            {
                client.CartofCient = new Cart();
                _context.Carts.Add(client.CartofCient);
                _context.SaveChanges();
            }
            var cart = client.CartofCient;
            cart.CartItems.Add(availableGameItem);
            _context.SaveChanges();
            return RedirectToAction("AddedToCart","Game");
}

    public IActionResult GameNotAvailable()
    {
        return View();
    }

    public IActionResult AddedToCart(int gameId)
    {
        return View();
    }
    }