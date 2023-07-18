using Lib.Models;
using Lib.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeMasRepository _employeeRepository;
        private readonly DesignationMasRepository _designationRepository;

        public EmployeeController(EmployeeMasRepository employeeRepository, DesignationMasRepository designationRepository)
        {
            _employeeRepository = employeeRepository;
            _designationRepository = designationRepository;
        }

        public async Task<IActionResult> Employees(string searchTerm)
        {
            IEnumerable<EmployeeMas> employees;

            if (string.IsNullOrEmpty(searchTerm))
            {
                employees = await _employeeRepository.GetAllAsync();
            }
            else
            {
                employees = await _employeeRepository.SearchAsync(searchTerm);
            }

            foreach (var employee in employees)
            {
                var designationName = await _employeeRepository.GetDesignationNameAsync(employee.DesignationId);
                employee.DesignationName = designationName;
            }

            return View(employees);
        }


		[HttpGet]
		public async Task<IActionResult> AddEdit(int? id)
		{
			// Get the list of designations
			var designations = await _designationRepository.GetAllAsync();

			// Create a SelectList for the dropdown
			var designationList = new SelectList(designations, "Id", "Name");

			ViewBag.Designations = designationList;

			if (id == null)
			{
				// Add Employee - Return the Add view
				return View(new EmployeeMas());
			}
			else
			{
				// Edit Employee - Return the Edit view
				var employee = await _employeeRepository.GetByIdAsync(id.Value);
				if (employee == null)
				{
					return NotFound();
				}

				// Decrypt the password before displaying it in the view
				employee.Password = EmployeeMasRepository.Decrypt(employee.Password);

				return View(employee);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddEdit(EmployeeMas employee)
		{
			if (ModelState.IsValid)
			{
				if (employee.Id == 0)
				{
					employee.CreatedBy = "Raj Tripathi";
					// Insert Employee
					await _employeeRepository.InsertAsync(employee);
					return RedirectToAction("Employees");

				}
				else
				{
					employee.ModifiedBy = "Raj Tripathi";
					// Retrieve the existing employee from the repository
					var existingEmployee = await _employeeRepository.GetByIdAsync(employee.Id);
					if (existingEmployee == null)
					{
						return NotFound();
					}

					// Preserve the existing password if not provided in the new employee data
					if (string.IsNullOrEmpty(employee.Password))
					{
						employee.Password = existingEmployee.Password;
					}
				}

				// Encrypt the password before storing it in the database
				employee.Password = EmployeeMasRepository.Encrypt(employee.Password);

				var success = await _employeeRepository.UpdateAsync(employee);
				if (!success)
				{
					return NotFound();
				}

				return RedirectToAction("Employees"); // Redirect to the desired page
			}

			// If the ModelState is not valid, return the view with the validation errors
			var designations = await _designationRepository.GetAllAsync();
			var designationList = new SelectList(designations, "Id", "Name");
			ViewBag.Designations = designationList;

			return View(employee);
		}





		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _employeeRepository.DeleteAsync(id);
            if (success)
            {
                return RedirectToAction(nameof(Employees));
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Profile(int id)
        {
            // Retrieve the employee details from the repository
            EmployeeMas employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
            {
                // Employee not found, return appropriate response (e.g., 404 Not Found)
                return NotFound();
            }

            // Get the designation name for the employee
            var designationName = await _employeeRepository.GetDesignationNameAsync(employee.DesignationId);

            // Assign the designation name to the employee object
            employee.DesignationName = designationName;

            // Pass the employee object to the view for display
            return View(employee);
        }

        public async Task<IActionResult> Users(string searchTerm)
        {
            IEnumerable<EmployeeMas> employees;

            if (string.IsNullOrEmpty(searchTerm))
            {
                employees = await _employeeRepository.GetAllAsync();
            }
            else
            {
                employees = await _employeeRepository.SearchAsync(searchTerm);
            }

            foreach (var employee in employees)
            {
                var designationName = await _employeeRepository.GetDesignationNameAsync(employee.DesignationId);
                employee.DesignationName = designationName;
            }

            return View(employees);
        }



    }
}
