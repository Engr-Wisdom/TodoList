using Microsoft.AspNetCore.Mvc;

namespace TodoList.Web.Controllers;

[Route("settings")]
public class SettingsController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View("~/Views/Pages/Settings.cshtml");
    }
}