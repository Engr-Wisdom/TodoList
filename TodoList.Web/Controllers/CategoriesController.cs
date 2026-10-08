using Microsoft.AspNetCore.Mvc;

namespace TodoList.Web.Controllers;

public class CategoriesController : Controller
{
    public IActionResult Work()
    {
        return View("~/Views/Pages/Work.cshtml");
    }

    public IActionResult Home()
    {
        return View("~/Views/Pages/Home.cshtml");
    }

    public IActionResult Church()
    {
        return View("~/Views/Pages/Church.cshtml");
    }

    public IActionResult School()
    {
        return View("~/Views/Pages/School.cshtml");
    }
}