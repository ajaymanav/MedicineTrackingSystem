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
        private readonly string folderAndfileName = "JsonData\\Medicine.json";
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
            string newPath = GetJsonFilesPath();

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
            string jsonFilePath = GetJsonFilesPath();

            var medicineData = GetMedicineJsondata();

            string objJson = JsonConvert.SerializeObject(obj,
                          Formatting.Indented);
            var newMedicine = JObject.Parse(objJson);
            medicineData.Add(newMedicine);
            string newJsonResult = JsonConvert.SerializeObject(medicineData,
                         Formatting.Indented);
            File.WriteAllText(jsonFilePath, newJsonResult);

            return true;
        }

        private string GetJsonFilesPath()
        {
            string webRootPath = $"{_hostingEnvironment.ContentRootPath}";

            string jsonFilePath = Path.Combine(webRootPath, "wwwroot", folderAndfileName);

            return jsonFilePath;
        }

        internal bool UpdateMedicines(Medicine obj)
        {
            string newPath = GetJsonFilesPath();

            var medicineData = GetMedicineData();

            foreach( var p in medicineData.Where(x => x.MedicineName == obj.MedicineName && x.Brand == obj.Brand && x.Price == obj.Price && x.ExpiryDate == obj.ExpiryDate))
            {
                p.Notes = obj.Notes;
            }

            string objJson = JsonConvert.SerializeObject(obj,
                          Formatting.Indented);
            var newMedicine = JObject.Parse(objJson);
            string newJsonResult = JsonConvert.SerializeObject(medicineData,
                         Formatting.Indented);
            File.WriteAllText(newPath, newJsonResult);

            return true;
        }
    }
}
