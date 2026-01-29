using System;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;
using MWS.Models;

namespace MWS.Modules
{
    class SysConnectionStringModule
    {
        public static string GetConnectionString(Action<int, string> reportProgress = null)
        {
            string path = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"Settings\SysConnectionString.json"
            );
            string json = File.ReadAllText(path);
            var serializer = new JavaScriptSerializer();
            var model = serializer.Deserialize<SysConnectionStringModel>(json);
            string connectionString =
                $"Data Source={model.DataSource};" +
                $"Initial Catalog={model.InitialCatalog};" +
                $"Persist Security Info=True;" +
                $"User ID={model.UserId};" +
                $"Password={model.Password};";
            return connectionString;
        }
    }
}
