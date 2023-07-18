using Lib.Models;
using Lib.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class SalaryAssignController : Controller
    {
        private readonly SalaryAssignRepository _salaryAssignRepository;
        private readonly EmployeeMasRepository _employeeMasRepository;
        private readonly SalaryAddonMasRepository _salaryAddonMasRepository;

        public SalaryAssignController(
            SalaryAssignRepository salaryAssignRepository,
            EmployeeMasRepository employeeMasRepository,
            SalaryAddonMasRepository salaryAddonMasRepository)
        {
            _salaryAssignRepository = salaryAssignRepository;
            _employeeMasRepository = employeeMasRepository;
            _salaryAddonMasRepository = salaryAddonMasRepository;
        }

        public async Task<IActionResult> SalaryAssignments(string searchTerm)
        {
            IEnumerable<SalaryAssign> salaryAssignments;

            if (string.IsNullOrEmpty(searchTerm))
            {
                salaryAssignments = await _salaryAssignRepository.GetAllAsync();
            }
            else
            {
                salaryAssignments = await _salaryAssignRepository.SearchAsync(searchTerm);
            }

            return View(salaryAssignments);
        }

        [HttpGet]
        public async Task<IActionResult> AssignEdit(int? id)
        {
            var model = new SalaryAssign();

            if (id != null)
            {
                model = await _salaryAssignRepository.GetByIdAsync(id.Value);
                if (model == null)
                {
                    return NotFound();
                }
            }

            await PopulateDropdownLists(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignEdit(int? id, SalaryAssign salaryAssign)
        {
            if (ModelState.IsValid)
            {
                if (salaryAssign.IsPercentage && salaryAssign.Value > 100)
                {
                    ModelState.AddModelError("Value", "Value cannot be greater than 100 when Is Percentage is true.");
                    await PopulateDropdownLists(salaryAssign);
                    return View(salaryAssign);
                }

                if (id == null)
                {
                    await _salaryAssignRepository.InsertAsync(salaryAssign);
                }
                else
                {
                    salaryAssign.Id = id.Value;
                    var success = await _salaryAssignRepository.UpdateAsync(salaryAssign);
                    if (!success)
                    {
                        return NotFound();
                    }
                }

                return RedirectToAction(nameof(SalaryAssignments));
            }

            await PopulateDropdownLists(salaryAssign);
            return View(salaryAssign);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _salaryAssignRepository.DeleteAsync(id);
            if (success)
            {
                return RedirectToAction(nameof(SalaryAssignments));
            }
            else
            {
                return NotFound();
            }
        }

        private async Task PopulateDropdownLists(SalaryAssign salaryAssign)
        {
            var employees = await _employeeMasRepository.GetAllAsync();
            ViewBag.Employees = employees;

            var salaryAddons = await _salaryAddonMasRepository.GetAllAsync();
            ViewBag.SalaryAddons = salaryAddons;
        }
    }
}
