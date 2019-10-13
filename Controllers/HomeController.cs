using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BankAccounts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace BankAccounts.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpPost("")]
        public IActionResult RegisterUser(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                dbContext.Add(newUser);
                dbContext.SaveChanges();
                HttpContext.Session.SetString("LoginUserEmail", newUser.Email);
                HttpContext.Session.SetInt32("CurUserId", newUser.UserId);
                return RedirectToAction("AccountPage", new { newUser.UserId });
            }
            return View("Index");
        }

        [HttpGet("login")]
        public IActionResult LoginPage()
        {
            return View("Login");
        }

        [HttpPost("login")]
        public IActionResult LoginUser(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                if(result ==0)
                {
                    ModelState.AddModelError("Password", "Password is incorrect, please try again.");
                    return View("Login");
                }
                HttpContext.Session.SetString("LoginUserEmail", userSubmission.Email);
                HttpContext.Session.SetInt32("CurUserId", userInDb.UserId);
                return RedirectToAction("AccountPage", new {userInDb.UserId});
            }
            return View("Login");
        }

        [HttpGet("logout")]
        public IActionResult LogoutUser()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LoginPage");
        }

        [HttpGet("account/{userId}")]
        public IActionResult AccountPage(int userId)
        {
            string UserSubmissionEmail = HttpContext.Session.GetString("LoginUserEmail");
            if(UserSubmissionEmail == null)
            {
                return RedirectToAction("LoginPage");
            }
            if(userId != HttpContext.Session.GetInt32("CurUserId"))
            {
                return Redirect($"/account/{HttpContext.Session.GetInt32("CurUserId")}");
            }
            User LoggedUser = dbContext.Users.Include(u => u.MyTransactions)
                .FirstOrDefault(u => u.UserId == userId);
            LoggedUser.MyTransactions.Reverse();
            ViewBag.CurUser = LoggedUser;
            return View("Account");
        }

        [HttpPost("transaction/new")]
        public IActionResult CreateTransaction(Transaction newTransaction)
        {
            if(HttpContext.Session.GetString("LoginUserEmail") == null)
            {
                return RedirectToAction("LoginPage");
            }
            string CurUser = HttpContext.Session.GetString("LoginUserEmail");
            User user = dbContext.Users.Include(u => u.MyTransactions)
                .FirstOrDefault(u => u.Email == CurUser);
            user.MyTransactions.Reverse();
            ViewBag.CurUser = user;
            if(ModelState.IsValid)
            {
                if(newTransaction.Amount < 0 && (Math.Abs(newTransaction.Amount) > user.Balance))
                {
                    ModelState.AddModelError("Amount", "Cannot withdraw more than the current balance!");
                    return View("Account");
                }
                if(newTransaction.Amount == 0)
                {
                    ModelState.AddModelError("Amount", "Please select an amount.");
                    return View("Account");
                }
                newTransaction.UserId = user.UserId;
                dbContext.Transactions.Add(newTransaction);
                user.Balance += newTransaction.Amount;
                dbContext.SaveChanges();
                return Redirect($"/account/{user.UserId}");
            }
            else
            {
                return View("Account");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
