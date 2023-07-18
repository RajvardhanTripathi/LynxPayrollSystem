using Lib.Models;
using Lib.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class HolidayController : Controller
    {
        private readonly HolidayMasRepository _holidayRepository;

        public HolidayController(HolidayMasRepository holidayRepository)
        {
            _holidayRepository = holidayRepository;
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
        public async Task<IActionResult> AddEdit(int? id)
        {
            if (id == null)
            {
                // Create operation
                return View(new HolidayMas());
            }
            else
            {
                // Edit operation
                var holiday = await _holidayRepository.GetByIdAsync(id.Value);
                if (holiday == null)
                {
                    return NotFound();
                }
                return View(holiday);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(int? id, HolidayMas holiday)
        {
            if (id == null)
            {
                // Create operation
                if (ModelState.IsValid)
                {
                    holiday.CreatedBy = "Raj Tripathi";
                    await _holidayRepository.InsertAsync(holiday);
                    return RedirectToAction(nameof(Holidays));
                }
            }
            else
            {
                // Edit operation
                if (id.Value != holiday.Id)
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    // Set the ModifiedBy property value
                    holiday.ModifiedBy = "Raj Tripathi";

                    var success = await _holidayRepository.UpdateAsync(holiday);
                    if (success)
                    {
                        return RedirectToAction(nameof(Holidays));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return View(holiday);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _holidayRepository.DeleteAsync(id);
            if (success)
            {
                return RedirectToAction(nameof(Holidays));
            }
            else
            {
                return NotFound();
            }
        }
    }
}
