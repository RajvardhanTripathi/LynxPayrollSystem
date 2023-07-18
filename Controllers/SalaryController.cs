using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
	public class SalaryController : Controller
	{
		[HttpGet]
		public IActionResult SalarySlip()
		{
			return View();
		}
		[HttpGet]
		public IActionResult SalaryDetails()
		{
			return View();
		}



	}
}
