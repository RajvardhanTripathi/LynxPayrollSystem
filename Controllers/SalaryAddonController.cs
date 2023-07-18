using Lib.Models;
using Lib.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class SalaryAddonController : Controller
    {
        private readonly SalaryAddonMasRepository _salaryAddonRepository;

        public SalaryAddonController(SalaryAddonMasRepository salaryAddonRepository)
        {
            _salaryAddonRepository = salaryAddonRepository;
        }

        public async Task<IActionResult> SalaryAddons(string searchTerm)
        {
            IEnumerable<SalaryAddonMas> salaryAddons;

            if (string.IsNullOrEmpty(searchTerm))
            {
                salaryAddons = await _salaryAddonRepository.GetAllAsync();
            }
            else
            {
                salaryAddons = await _salaryAddonRepository.SearchAsync(searchTerm);
            }

            return View(salaryAddons);
        }


        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            if (id == null)
            {
                // Create operation
                return View(new SalaryAddonMas());
            }
            else
            {
                // Edit operation
                var salaryAddon = await _salaryAddonRepository.GetByIdAsync(id.Value);
                if (salaryAddon == null)
                {
                    return NotFound();
                }
                return View(salaryAddon);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(int? id, SalaryAddonMas salaryAddon)
        {
            if (id == null)
            {
                // Create operation
                if (ModelState.IsValid)
                {
                    salaryAddon.CreatedBy = "Raj Tripathi";

                    await _salaryAddonRepository.InsertAsync(salaryAddon);
                    return RedirectToAction(nameof(SalaryAddons));
                }
            }
            else
            {
                // Edit operation
                if (id.Value != salaryAddon.Id)
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    // Set the ModifiedBy property value
                    salaryAddon.ModifiedBy = "Raj Tripathi";

                    // Perform additional validation


                    var success = await _salaryAddonRepository.UpdateAsync(salaryAddon);
                    if (success)
                    {
                        return RedirectToAction(nameof(SalaryAddons));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return View(salaryAddon);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _salaryAddonRepository.DeleteAsync(id);
            if (success)
            {
                return RedirectToAction(nameof(SalaryAddons));
            }
            else
            {
                return NotFound();
            }
        }
    }
}
