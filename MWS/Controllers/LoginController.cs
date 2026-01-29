using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWS.Controllers
{
    class LoginController
    {
        // ============
        // Data Context
        // ============
        public DB.mwsdbDataContext db = new DB.mwsdbDataContext(Modules.SysConnectionStringModule.GetConnectionString());

        // =====
        // Login
        // =====
        public String[] Login(String username, String password)
        {
            try
            {
                var currentUser = from d in db.MstUsers
                                  where d.Username.Equals(username)
                                  && d.Password.Equals(password)
                                  select d;

                if (currentUser.Any())
                {
                    if (currentUser.FirstOrDefault().IsLocked == false)
                    {
                        return new String[] { "User account is already deactivated.", "0" };
                    }
                    else
                    {
                        Modules.SysCurrentModule.UpdateCurrentSettingsLogin(currentUser.FirstOrDefault().Id, currentUser.FirstOrDefault().Username);
                        return new String[] { "", currentUser.FirstOrDefault().Id.ToString() };
                    }
                }
                else
                {
                    return new String[] { "Username or password is incorrect.", "0" };
                }
            }
            catch (Exception e)
            {
                return new String[] { e.Message, "0" };
            }
        }
    }
}
