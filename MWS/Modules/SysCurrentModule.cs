using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace MWS.Modules
{
    class SysCurrentModule
    {
        public static String path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings\SysCurrent.json");

        // Get Current Settings 
        public static Models.SysCurrentModel GetCurrentSettings()
        {
            if (!File.Exists(path))
                throw new Exception("Settings file not found: " + path);

            string json = File.ReadAllText(path).Trim();

            if (string.IsNullOrWhiteSpace(json))
                throw new Exception("Settings file is empty.");

            if (!json.StartsWith("{"))
                throw new Exception("Invalid JSON format. File content: " + json);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<Models.SysCurrentModel>(json);
        }


        // Update Current Settings - Login
        public static void UpdateCurrentSettingsLogin(Int32 currentUserId, String userName)
        {
            var currentSettings = GetCurrentSettings();

            Models.SysCurrentModel newCurrent = new Models.SysCurrentModel()
            {
                CompanyName = currentSettings.CompanyName,
                Address = currentSettings.Address,
                ContactNo = currentSettings.ContactNo,
                CurrentUserId = currentUserId,
                CurrentUserName = userName,
                CurrentVersion = currentSettings.CurrentVersion,
                CurrentDeveloper = currentSettings.CurrentDeveloper,
                CurrentSupport = currentSettings.CurrentSupport,
                BranchId = currentSettings.BranchId,
                IsReceiver = currentSettings.IsReceiver
            };

            String newJson = new JavaScriptSerializer().Serialize(newCurrent);
            File.WriteAllText(path, newJson);
        }

        // Update Current Settings - Login
        public static void UpdateCurrentSettings(Models.SysCurrentModel objSysCurrentModel)
        {
            var currentSettings = GetCurrentSettings();

            Models.SysCurrentModel newSysCurrents = new Models.SysCurrentModel()
            {
                CompanyName = objSysCurrentModel.CompanyName,
                Address = objSysCurrentModel.Address,
                ContactNo = objSysCurrentModel.ContactNo,
                CurrentUserId = currentSettings.CurrentUserId,
                CurrentUserName = currentSettings.CurrentUserName,
                CurrentVersion = objSysCurrentModel.CurrentVersion,
                CurrentDeveloper = objSysCurrentModel.CurrentDeveloper,
                CurrentSupport = objSysCurrentModel.CurrentSupport,
                BranchId = objSysCurrentModel.BranchId,
                IsReceiver = objSysCurrentModel.IsReceiver
            };

            String newJson = new JavaScriptSerializer().Serialize(newSysCurrents);
            File.WriteAllText(path, newJson);
        }
    }
}
