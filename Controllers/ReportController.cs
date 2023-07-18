using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
	public class ReportController : Controller
	{
		public IActionResult EmployeePayrollReports()
		{
			return View();
		}

		public IActionResult PayrollReports()
		{
			return View();
		}
	}
}
