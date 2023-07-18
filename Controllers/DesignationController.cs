using Lib.Models;
using Lib.Repository;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
namespace Web.Controllers
{
    public class DesignationController : Controller
    {
        private readonly DesignationMasRepository _designationRepository;

        public DesignationController(DesignationMasRepository designationRepository)
        {
            _designationRepository = designationRepository;
        }

        public async Task<IActionResult> Designations(string sortOrder, string searchTerm)
        {
            ViewBag.NameSortParam = string.IsNullOrEmpty(sortOrder) ? "asc" : "";

            IEnumerable<DesignationMas> designations;

            if (string.IsNullOrEmpty(searchTerm))
            {
                designations = await _designationRepository.GetAllAsync();
            }
            else
            {
                designations = await _designationRepository.SearchAsync(searchTerm);
            }

            switch (sortOrder)
            {
                case "desc":
                    designations = designations.OrderByDescending(d => d.Name);
                    ViewBag.NameSortParam = "asc";
                    break;
                default:
                    designations = designations.OrderBy(d => d.Name);
                    ViewBag.NameSortParam = "desc";
                    break;
            }

            return View(designations);
        }




        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            if (id == null)
            {
                // Create operation

                return View(new DesignationMas());
            }
            else
            {
                // Edit operation
                var designation = await _designationRepository.GetByIdAsync(id.Value);
                if (designation == null)
                {
                    return NotFound();
                }
                return View(designation);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(int? id, DesignationMas designation)
        {
            if (id == null)
            {
                // Create operation
                if (ModelState.IsValid)
                {
                    designation.CreatedBy = "Raj Tripathi";
                    await _designationRepository.InsertAsync(designation);
                    return RedirectToAction(nameof(Designations));
                }
            }
            else
            {
                // Edit operation
                if (id.Value != designation.Id)
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    // Set the ModifiedBy property value
                    designation.ModifiedBy = "Raj Tripathi";

                    var success = await _designationRepository.UpdateAsync(designation);
                    if (success)
                    {
                        return RedirectToAction(nameof(Designations));
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return View(designation);
        }





        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            var success = await _designationRepository.DeleteAsync(id);
            if (success)
                return RedirectToAction(nameof(Designations));
            else
                return NotFound();
        }

    }
}
