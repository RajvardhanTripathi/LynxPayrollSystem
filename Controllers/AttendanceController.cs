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
    public class AttendanceController : Controller
    {
        private readonly IDbConnection _db;

        public AttendanceController(IDbConnection db)
        {
            _db = db;
        }

        public ActionResult Index()
        {
            try
            {
                _db.Open();

                string query = "SELECT * FROM Attendance";
                List<Attendance> attendanceList = _db.Query<Attendance>(query).ToList();

                ViewBag.Message = "";
                return View(attendanceList);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error occurred while retrieving attendance data: " + ex.Message;
                return View(new List<Attendance>());
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

                        List<Attendance> attendanceList = new List<Attendance>();

                        // Iterate through rows, starting from the second row (assuming the first row is header)
                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            Attendance attendance = new Attendance();
                            attendance.UserId = Convert.ToInt32(worksheet.Cells[row, 2].Value);
                            attendance.AttDate = Convert.ToDateTime(worksheet.Cells[row, 3].Value);
                            attendance.InOutFlag = Convert.ToBoolean(worksheet.Cells[row, 4].Value);
                            attendance.Remark = worksheet.Cells[row, 5].Value.ToString();
                            attendance.CreatedDate = Convert.ToDateTime(worksheet.Cells[row, 6].Value);
                            attendance.ModifiedDate = string.IsNullOrEmpty(worksheet.Cells[row, 7].Value.ToString()) ? (DateTime?)null : Convert.ToDateTime(worksheet.Cells[row, 7].Value);
                            attendance.DeviceId = worksheet.Cells[row, 8].Value?.ToString();

                            attendanceList.Add(attendance);
                        }

                        // Save the imported data to your database or perform any required processing

                        ViewBag.Message = "Import successful";
                        return RedirectToAction("Index", attendanceList);
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
