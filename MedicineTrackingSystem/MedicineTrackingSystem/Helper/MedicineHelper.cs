using MedicineTrackingSystem.Models;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace MedicineTrackingSystem.Helper
{
    public class MedicineHelper
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public MedicineHelper(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IQueryable<Medicine> GetMedicineData(string sortColumn, string sortColumnDir, string search)
        {
            var obj = GetMedicineData();

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                obj = obj.OrderBy(sortColumn + " " + sortColumnDir);
            }

            if (!string.IsNullOrEmpty(search))
            {
                obj = obj.Where(m => m.MedicineName == search);
            }

            return obj;
        }

        private IQueryable<Medicine> GetMedicineData()
        {
           var data = GetMedicineJsondata();
           return data.ToObject<List<Medicine>>().AsQueryable();
        }

        private JArray GetMedicineJsondata()
        {
            string folderAndfileName = "JsonData\\Medicine.json";

            string webRootPath = $"{_hostingEnvironment.ContentRootPath}";

            string newPath = Path.Combine(webRootPath, "wwwroot", folderAndfileName);


            string data = string.Empty;
            JArray jr = new JArray();
            using (StreamReader r = new StreamReader(newPath))
            {
                data = r.ReadToEnd();
            }
            if (!string.IsNullOrEmpty(data))
            {
                //Read Entire Json file
                 jr = (JArray)JsonConvert.DeserializeObject(data);
            }

            return jr;
        }

        internal bool AddMedicines(Medicine obj)
        {
            string folderAndfileName = "JsonData\\Medicine.json";

            string webRootPath = $"{_hostingEnvironment.ContentRootPath}";

            string newPath = Path.Combine(webRootPath, "wwwroot", folderAndfileName);

            var medicineData = GetMedicineJsondata(); 

            string objJson = JsonConvert.SerializeObject(obj,
                          Formatting.Indented);
            var newMedicine = JObject.Parse(objJson);
            medicineData.Add(newMedicine);
            string newJsonResult = JsonConvert.SerializeObject(medicineData,
                         Formatting.Indented);
            File.WriteAllText(newPath, newJsonResult);

            return true;
        }
    }
}
