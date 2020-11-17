using MedicineTrackingSystem.Helper;
using MedicineTrackingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Linq;

namespace MedicineTrackingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly MedicineHelper _medicineHelper;
        public HomeController(MedicineHelper medicineHelper)
        {
            _medicineHelper = medicineHelper;
        }
        public IActionResult MedicineDetails()
        {
            return View();
        }

        public ActionResult getMedicineData()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                string sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                var v = _medicineHelper.GetMedicineData(sortColumn, sortColumnDir, searchValue);
                recordsTotal = v.Count();
                var data = v.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IActionResult AddMedicines()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMedicines(Medicine obj)
        {
            if (!ModelState.IsValid)
            {
                return View("AddMedicines", obj);
            }

            var status = _medicineHelper.AddMedicines(obj);

            if(status)
            {
                ViewBag.Message = "Medicine Added Successfully!!";
                ModelState.Clear();
            }
            else
            {
                return View("AddMedicines", obj);
            }
           
            return View("AddMedicines", new Medicine());
        }

        [HttpGet]
        public IActionResult EditMedicines(Medicine obj)
        {
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateMedicines(Medicine obj)
        {
            if (!ModelState.IsValid)
            {
                return View("EditMedicines", obj);
            }

            var status = _medicineHelper.UpdateMedicines(obj);

            if (status)
            {
                ViewBag.Message = "Medicine Notes Updated Successfully!!";
            }

            return View("EditMedicines", obj);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
