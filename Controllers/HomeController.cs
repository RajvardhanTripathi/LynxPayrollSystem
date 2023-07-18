using Lib.Models;
using Lib.Repository;
using LynxPayrollSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LynxPayrollSystem.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly EmployeeMasRepository _employeeRepository;
		private readonly DesignationMasRepository _designationRepository;
		private readonly HolidayMasRepository _holidayRepository;

		public HomeController(ILogger<HomeController> logger, EmployeeMasRepository employeeRepository, DesignationMasRepository designationRepository, HolidayMasRepository holidayRepository)
		{
			_logger = logger;
			_employeeRepository = employeeRepository;
			_designationRepository = designationRepository;
			_holidayRepository = holidayRepository;
			_holidayRepository = holidayRepository;
		}

		

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> Privacy()
		{
			string empId = User.Identity.Name;
			IEnumerable<EmployeeMas> employees = await _employeeRepository.GetAllAsync();

			EmployeeMas employee = employees.FirstOrDefault(e => e.EmpId == empId);

			if (employee != null)
			{
				ViewBag.UserName = employee.Name;
				return View(employee);
			}

			return NotFound();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}