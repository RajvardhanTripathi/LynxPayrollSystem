using Lib.Models;
using Lib.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class EmployeeHomeController : Controller
    {
        private readonly EmployeeMasRepository _employeeRepository;
        private readonly DesignationMasRepository _designationRepository;
        private readonly HolidayMasRepository _holidayRepository;

        public EmployeeHomeController(EmployeeMasRepository employeeRepository, DesignationMasRepository designationRepository, HolidayMasRepository holidayRepository)
        {
            _employeeRepository = employeeRepository;
            _designationRepository = designationRepository;
            _holidayRepository = holidayRepository;
            _holidayRepository = holidayRepository;
        }

        public async Task<IActionResult> Index()
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

        public async Task<IActionResult> Holidays(string sortOrder, string searchTerm)
        {
            ViewBag.NameSortParam = string.IsNullOrEmpty(sortOrder) ? "asc" : "";

            IEnumerable<HolidayMas> holidays;

            if (string.IsNullOrEmpty(searchTerm))
            {
                holidays = await _holidayRepository.GetAllAsync();
            }
            else
            {
                holidays = await _holidayRepository.SearchAsync(searchTerm);
            }

            switch (sortOrder)
            {
                case "desc":
                    holidays = holidays.OrderByDescending(a => a.Name);
                    ViewBag.NameSortParam = "asc";
                    break;
                default:
                    holidays = holidays.OrderBy(a => a.Name);
                    ViewBag.NameSortParam = "desc";
                    break;
            }

            return View(holidays);
        }

		[HttpGet]
		public IActionResult Leaves()
		{
			return View();
		}
        [HttpGet]
		public IActionResult Attendances()
		{
			return View();
		}
	}
}
