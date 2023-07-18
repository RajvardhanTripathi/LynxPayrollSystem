using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Lib.Models;
using System.Data;
using Dapper;

namespace YourNamespace.Controllers
{
    public class LeaveController : Controller
    {
        private readonly IDbConnection _db;

        public LeaveController(IDbConnection db)
        {
            _db = db;
        }

        public ActionResult Index()
        {
            try
            {
                _db.Open();

                string query = "SELECT * FROM Leave";
                List<Leave> leaves = _db.Query<Leave>(query).AsList();

                ViewBag.Message = "";
                return View(leaves);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error occurred while retrieving leave data: " + ex.Message;
                return View(new List<Leave>());
            }
            finally
            {
                _db.Close();
            }
        }

        [HttpPost]
        public ActionResult Import(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                try
                {
                    using (var package = new ExcelPackage(file.OpenReadStream()))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[1]; // Assuming the worksheet is at position 1

                        List<Leave> leaves = new List<Leave>();

                        // Iterate through rows, starting from the second row (assuming the first row is header)
                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            Leave leave = new Leave();
                            leave.UserId = Convert.ToInt32(worksheet.Cells[row, 2].Value);
                            leave.LeaveDate = Convert.ToDateTime(worksheet.Cells[row, 3].Value);
                            leave.LeaveType = Convert.ToByte(worksheet.Cells[row, 4].Value);
                            leave.DurationFlag = Convert.ToBoolean(worksheet.Cells[row, 5].Value);
                            leave.IsApproved = Convert.ToBoolean(worksheet.Cells[row, 6].Value);
                            leave.Reason = worksheet.Cells[row, 7].Value.ToString();
                            leave.CreatedDate = Convert.ToDateTime(worksheet.Cells[row, 8].Value);
                            leave.ModifiedDate = string.IsNullOrEmpty(worksheet.Cells[row, 9].Value.ToString()) ? (DateTime?)null : Convert.ToDateTime(worksheet.Cells[row, 9].Value);
                            leave.DeviceId = worksheet.Cells[row, 10].Value?.ToString();
                            leave.Description = worksheet.Cells[row, 11].Value?.ToString();
                            leave.ApprovedBy = worksheet.Cells[row, 12].Value?.ToString();
                            leave.ModifiedBy = worksheet.Cells[row, 13].Value?.ToString();

                            leaves.Add(leave);
                        }

                        // Save the imported data to your database or perform any required processing

                        _db.Open();
                        _db.Execute("INSERT INTO Leave (UserId, LeaveDate, LeaveType, DurationFlag, IsApproved, Reason, CreatedDate, ModifiedDate, DeviceId, Description, ApprovedBy, ModifiedBy) " +
                                    "VALUES (@UserId, @LeaveDate, @LeaveType, @DurationFlag, @IsApproved, @Reason, @CreatedDate, @ModifiedDate, @DeviceId, @Description, @ApprovedBy, @ModifiedBy)",
                                    leaves);

                        ViewBag.Message = "Import successful";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Error occurred during import: " + ex.Message;
                }
            }
            else
            {
                ViewBag.Message = "Please select a file to import";
            }

            return RedirectToAction("Index");
        }
    }
}
