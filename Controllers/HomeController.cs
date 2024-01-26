using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CSharpExamArmandoP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace CSharpExamArmandoP.Controllers;

public class SessionCheckAttribute : ActionFilterAttribute {
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        int? userId = context.HttpContext.Session.GetInt32("UserId");
        if (userId == null) {
            context.Result = new RedirectToActionResult("Auth", "Home", null);
        }
    }
}

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context; 

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    [SessionCheck]
    public IActionResult Index()
    {
        var allHobbies = _context.Hobbies.Include(h => h.Enthusiasts).ToList();

        Dictionary<string, int> noviceHobbyCount = new Dictionary<string, int>();
        Dictionary<string, int> intermediateHobbyCount = new Dictionary<string, int>();
        Dictionary<string, int> expertHobbyCount = new Dictionary<string, int>();

        foreach (var hobby in allHobbies)
        {
            foreach (var enthusiast in hobby.Enthusiasts)
            {
                string proficiency = enthusiast.Proficiency;

                if (proficiency == "Novice")
                {
                    if (!noviceHobbyCount.ContainsKey(hobby.HobbyName))
                    {
                        noviceHobbyCount[hobby.HobbyName] = 0;
                    }
                    noviceHobbyCount[hobby.HobbyName]++;
                }
                else if (proficiency == "Intermediate")
                {
                    if (!intermediateHobbyCount.ContainsKey(hobby.HobbyName))
                    {
                        intermediateHobbyCount[hobby.HobbyName] = 0;
                    }
                    intermediateHobbyCount[hobby.HobbyName]++;
                }
                else if (proficiency == "Expert")
                {
                    if (!expertHobbyCount.ContainsKey(hobby.HobbyName))
                    {
                        expertHobbyCount[hobby.HobbyName] = 0;
                    }
                    expertHobbyCount[hobby.HobbyName]++;
                }
            }
        }

        ViewBag.SortedNoviceHobbies = noviceHobbyCount.OrderByDescending(entry => entry.Value).Select(entry => entry.Key).ToList();
        ViewBag.SortedIntermediateHobbies = intermediateHobbyCount.OrderByDescending(entry => entry.Value).Select(entry => entry.Key).ToList();
        ViewBag.SortedExpertHobbies = expertHobbyCount.OrderByDescending(entry => entry.Value).Select(entry => entry.Key).ToList();

        var topNoviceHobbies = GetTopHobbiesByProficiency(allHobbies, "Novice");
        var topIntermediateHobbies = GetTopHobbiesByProficiency(allHobbies, "Intermediate");
        var topExpertHobbies = GetTopHobbiesByProficiency(allHobbies, "Expert");

        ViewBag.AllHobbies = allHobbies;
        ViewBag.TopNoviceHobbies = topNoviceHobbies;
        ViewBag.TopIntermediateHobbies = topIntermediateHobbies;
        ViewBag.TopExpertHobbies = topExpertHobbies;

        return View();
    }

    private List<Hobby> GetTopHobbiesByProficiency(List<Hobby> allHobbies, string proficiency)
    {
        var filteredHobbies = allHobbies
            .Where(h => h.Enthusiasts.Any(e => e.Proficiency == proficiency))
            .ToList();

        var topHobbies = filteredHobbies
            .OrderByDescending(h => h.Enthusiasts.Count)
            .Take(5)
            .ToList();

        return topHobbies;
    }

    [HttpGet("Auth")]
    public IActionResult Auth() {
        return View();
    }

    [HttpPost("Register")]
    public IActionResult Register(User userFromForm) {
        if (ModelState.IsValid) {
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            userFromForm.Password = Hasher.HashPassword(userFromForm, userFromForm.Password);
            _context.Add(userFromForm);
            _context.SaveChanges();

            return RedirectToAction("Auth");
        }
        return View("Auth");
    }

    [HttpPost("Login")]
    public IActionResult Login(LoginUser registeredUser) {
        if (ModelState.IsValid) {
            User userFromDb = _context.Users.FirstOrDefault(e => e.Username == registeredUser.LoginUsername);

            if (userFromDb == null) {
                ModelState.AddModelError("LoginUsername", "Invalid username address.");
                return View("Auth");
            }

            PasswordHasher<LoginUser> Hasher = new PasswordHasher<LoginUser>();
            
            var result = Hasher.VerifyHashedPassword(registeredUser, userFromDb.Password, registeredUser.LoginPassword);

            if (result == 0) {
                ModelState.AddModelError("LoginPassword", "Invalid password.");
                return View("Auth");
            }

            HttpContext.Session.SetInt32("UserId", userFromDb.UserId);
            return RedirectToAction("Index");
        }

        return View("Auth");
    }

    [HttpGet("Logout")]
    public IActionResult Logout() {
        HttpContext.Session.Clear();
        return RedirectToAction("Auth");
    }

    [SessionCheck]
    [HttpGet("CreateHobby")]
    public IActionResult CreateHobby() {
        return View();
    }

    [SessionCheck]
    [HttpPost("CreatedHobby")]
    public IActionResult CreatedHobby(Hobby hobbyFromForm) {
        if (ModelState.IsValid) {
            if (hobbyFromForm.IsUnique(_context.Hobbies, null))
            {
                hobbyFromForm.UserId = HttpContext.Session.GetInt32("UserId");
                _context.Add(hobbyFromForm);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("HobbyName", "Hobby name already exists.");
        }
        return View("CreateHobby", hobbyFromForm);
    }

    [SessionCheck]
    [HttpGet("hobbies/{id}")]
    public IActionResult ViewHobbyDetails(int id) {
        int? userId = HttpContext.Session.GetInt32("UserId");
        ViewBag.userId = userId;

        var hobbiesAndEnthusiasts = _context.Hobbies.Include(e => e.Enthusiasts).ThenInclude(e => e.User).FirstOrDefault(e => e.HobbyId == id);
        ViewBag.ViewHobbyData = hobbiesAndEnthusiasts;

        User user = _context.Users.FirstOrDefault(e => e.UserId == HttpContext.Session.GetInt32("UserId"));
        ViewBag.UserId = user.UserId;

        return View("ViewHobbyDetails");
    }

    [SessionCheck]
    [HttpPost("AddHobbyEnthusiasts/{id}")]
    public IActionResult AddHobbyEnthusiasts(int id, string Proficiency)
    {
        User user = _context.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));

        bool isUserAlreadyAnEnthusiast = _context.HobbyEnthusiasts.Any(p => p.UserId == user.UserId && p.HobbyId == id);

        if (!isUserAlreadyAnEnthusiast)
        {
            HobbyEnthusiast enthusiastsFromDb = new HobbyEnthusiast
            {
                HobbyId = id,
                UserId = user.UserId,
                Proficiency = Proficiency
            };

            _context.Add(enthusiastsFromDb);
            _context.SaveChanges();
        }

        return RedirectToAction("ViewHobbyDetails", new { id = id });
    }

    [SessionCheck]
    [HttpGet("EditHobby/{id}")]
    public IActionResult EditHobby(int id)
    {
        var hobby = _context.Hobbies.FirstOrDefault(h => h.HobbyId == id);

        int? userId = HttpContext.Session.GetInt32("UserId");
        if (hobby == null || hobby.UserId != userId)
        {
            return RedirectToAction("EditHobby");
        }

        return View(hobby);
    }

    [SessionCheck]
    [HttpPost("UpdateHobby")]
    public IActionResult UpdateHobby(Hobby updatedHobby)
    {
        if (ModelState.IsValid)
        {
            if (updatedHobby.IsUnique(_context.Hobbies, updatedHobby.HobbyId))
            {
                var originalHobby = _context.Hobbies.FirstOrDefault(h => h.HobbyId == updatedHobby.HobbyId);

                int? userId = HttpContext.Session.GetInt32("UserId");
                if (originalHobby == null || originalHobby.UserId != userId)
                {
                    return RedirectToAction("Error");
                }

                originalHobby.HobbyName = updatedHobby.HobbyName;
                originalHobby.HobbyDescription = updatedHobby.HobbyDescription;

                _context.SaveChanges();

                return RedirectToAction("ViewHobbyDetails", new { id = updatedHobby.HobbyId });
            }

            ModelState.AddModelError("HobbyName", "Hobby name already exists.");
        }

        return View("EditHobby", updatedHobby);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}