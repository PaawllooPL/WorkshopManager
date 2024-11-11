using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkshopManager.Model.DataModels;
using WorkshopManager.ViewModels.VM;

namespace WorkshopManager.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public HomeController(ILogger logger, SignInManager<User> signInManager, RoleManager<Role> roleManager, UserManager<User> userManager) : base(logger)
        {
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            if(_signInManager.IsSignedIn(User))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if(userIdClaim != null)
                {
                    var user = _userManager.FindByIdAsync(userIdClaim.Value).Result;
                    
                    if (user == null)
                    {
                        await _signInManager.SignOutAsync();
                        return View();
                    }

                    if (_userManager.IsInRoleAsync(user, Enum.GetName(typeof(RoleValue), (int)RoleValue.Client)!).Result)
                    {
                        return RedirectToAction("Index", "ClientRepair");
                    }
                    if (_userManager.IsInRoleAsync(user, Enum.GetName(typeof(RoleValue), (int)RoleValue.Owner)!).Result)
                    {
                        return RedirectToAction("Index", "OwnerRepair");
                    }
                }
            }
            //If not logged in
            return View();
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
