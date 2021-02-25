using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WeddingPlanner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyContext dbContext;

        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        // helper method to get the current user id
        public User GetCurrentUser()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return null;
            }
            return dbContext
                .Users
                .First(u => u.UserId == userId);
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User userToRegister)
        {
            if (dbContext.Users.Any(u => u.Email == userToRegister.Email))
            {
                ModelState.AddModelError("Email", "Please use a different email.");
            }

            if (ModelState.IsValid)
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                userToRegister.Password = Hasher.HashPassword(userToRegister, userToRegister.Password);
                dbContext.Add(userToRegister);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("UserId", userToRegister.UserId);

                return RedirectToAction("Dashboard");
            }

            return View("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser userToLogin)
        {
            if (ModelState.IsValid)
            {
                // look in the DB              if we don't find the user at all, the default is null
                var foundUser = dbContext.Users.FirstOrDefault(u => u.Email == userToLogin.LoginEmail);

                if (foundUser == null)
                {
                    ModelState.AddModelError("LoginEmail", "Please check your email and password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userToLogin, foundUser.Password, userToLogin.LoginPassword);

                if (result == 0)
                {
                    ModelState.AddModelError("LoginPassword", "Please check your email and password");
                    return View("Index");
                }

                // set ID in session
                HttpContext.Session.SetInt32("UserId", foundUser.UserId);
                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            // ** Enable Login session later **
            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.CurrentUser = currentUser;

            ViewBag.AllWedding = dbContext.Weddings
                .Include(w => w.CreatedBy)
                .Include(w => w.RSVPs)
                .OrderBy(d => d.Date);

            return View();
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet("create")]
        public IActionResult NewWeddingPage()
        {
            return View();
        }

        [HttpPost("create-wedding")]
        public IActionResult CreateWedding(Wedding newWed)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index");
            }

            if (newWed.Date <= DateTime.Now)
            {
                ModelState.AddModelError("Date", "Please ensure the date is in the future");
            }

            if (ModelState.IsValid)
            {
                newWed.UserId = (int)HttpContext.Session.GetInt32("UserId"); // so the movie created has the UserId linked to it

                dbContext.Add(newWed);
                dbContext.SaveChanges();

                return Redirect($"/wedding/{newWed.WeddingId}");
            }

            return View("NewWeddingPage");
        }

        [HttpGet("wedding/{weddingId}")]
        public IActionResult WeddingDetail(int weddingId)
        {
            // ** enable this later!!!
            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.OneWedding = dbContext.Weddings
            .Include(w => w.CreatedBy)
            .Include(w => w.RSVPs)
                .ThenInclude(u => u.UserWhoRSVPd)
            .First(u => u.WeddingId == weddingId);
            return View();
        }

        [HttpGet("map")]
        public IActionResult Map()
        {
            return View();
        }

        // Delete/ RSVP/ UnRSVP
        [HttpGet("delete/{weddingId}")]
        public IActionResult Delete(int weddingId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index");
            }

            Wedding DeleteWedding = dbContext.Weddings
                .SingleOrDefault(w => w.WeddingId == weddingId);
            dbContext.Weddings.Remove(DeleteWedding);
            dbContext.SaveChanges();
            Console.WriteLine("***deleting");

            return RedirectToAction("Dashboard");

        }

        [HttpGet("un-rsvp/{weddingId}")]
        public IActionResult UnRSVP(int weddingId)
        {
            var currentUser = GetCurrentUser();

            var RsvpToDelete = dbContext.RSVPs
                .First(r => r.WeddingId == weddingId && r.UserId == currentUser.UserId);

            dbContext.Remove(RsvpToDelete);
            dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }


        [HttpGet("rsvp/{weddingId}")]
        public IActionResult RSVP(int weddingId)
        {
            // creating an object without a constructor
            var RsvpToAdd = new RSVP
            {
                UserId = GetCurrentUser().UserId,
                WeddingId = weddingId
            };

            dbContext.Add(RsvpToAdd);
            dbContext.SaveChanges();

            return RedirectToAction("Dashboard");
        }

    }
}
