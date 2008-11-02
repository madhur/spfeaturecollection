using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.Office.Server;
using Microsoft.Office.Server.Administration;
using Microsoft.Office.Server.UserProfiles;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.WebPartPages;
using System.Configuration;
using System.Data;
using System.Reflection;

namespace MySiteLib
{
    public class MySiteLib
    {
        string _mySiteHostUrl;
        ServerContext _context;
        static MySiteLib _singlInstance;
        private static Object syncObj=new object();

        public  string Initialize()
        {
               
                UserProfileManager profileManager = new UserProfileManager(_context);
                _mySiteHostUrl=profileManager.MySiteHostUrl;
                return _mySiteHostUrl;

        }
        public static MySiteLib GetInstance()
        {
            lock (syncObj)
            {


                if (_singlInstance == null)
                {
                    _singlInstance = new MySiteLib();
                    _singlInstance._context = ServerContext.Default;

                    if (_singlInstance._context == null)
                        return null;

                    return _singlInstance;
                }
                else
                {
                    return _singlInstance;
                }
            }
            

        }

        public string CommitChanges(DataTable dtChanges)
        {
            UserProfileManager profileManager = new UserProfileManager(_context);
            StringBuilder errors = new StringBuilder();
            foreach (UserProfile profile in profileManager)
            {

                if (profile.PersonalSite == null)
                {
                    continue;
                }

                SPSite mySite = profile.PersonalSite;
                using (SPWeb myWeb = mySite.OpenWeb())
                {
                    string defaultFileName;
                    if (ConfigurationSettings.AppSettings["DefaultFile"] != null)
                    {
                        defaultFileName = ConfigurationSettings.AppSettings["DefaultFile"];
                        if (string.IsNullOrEmpty(defaultFileName))
                            defaultFileName = "default.aspx";
                    }
                    else
                    {
                        defaultFileName = "default.aspx";
                    }
                    SPFile defaultFile;
                    try
                    {
                        defaultFile = myWeb.Files[defaultFileName];
                    }
                    catch (UnauthorizedAccessException)
                    {
                        errors.AppendFormat("Unauthorized for {0}\n", profile.PersonalUrl);
                        continue;
                    }

                    WebPartManager wpm = new WebPartManager();

                    SPLimitedWebPartManager lmWpm = defaultFile.GetLimitedWebPartManager(PersonalizationScope.Shared);

                    foreach (System.Web.UI.WebControls.WebParts.WebPart wp in lmWpm.WebParts)
                    {
                        DataRow[] propChanges = dtChanges.Select("WebPart='" + wp.GetType().AssemblyQualifiedName + "'");
                        foreach (DataRow changeRow in propChanges)
                        {
                            string propName = changeRow["Property"].ToString();

                            System.Reflection.PropertyInfo propInfo= wp.GetType().GetProperty(propName);
                            try
                            {
                                propInfo.SetValue(wp, changeRow["Value"].ToString(), null);
                            }
                            catch (TargetInvocationException)
                            {
                                errors.AppendFormat("Invocation Exception for property {0} on {1}\n", propName, wp.Title);



                            }
                            lmWpm.SaveChanges(wp);
                            

                        }




                    }

                }

                mySite.Dispose();
            }

            return errors.ToString();
        }
    



        public SPLimitedWebPartCollection GetWebPartsList()
        {
            string userAccount=string.Empty;
            UserProfileManager profileManager = new UserProfileManager(_context);
            if (ConfigurationSettings.AppSettings["UserAccount"] != null)
            {
                userAccount = ConfigurationSettings.AppSettings["UserAccount"];
                if (string.IsNullOrEmpty(userAccount))
                    userAccount = System.Environment.UserName;
            }
            else
            {
                userAccount = System.Environment.UserName;
            }

            UserProfile defProfile=profileManager.GetUserProfile(userAccount);

            if (defProfile.PersonalSite == null)
            {
                throw new MySiteDoesNotExistException();
                
            }

            SPSite mySite = defProfile.PersonalSite;

            using (SPWeb myWeb = mySite.OpenWeb())
            {
                string defaultFileName;
                if (ConfigurationSettings.AppSettings["DefaultFile"] != null)
                {
                    defaultFileName = ConfigurationSettings.AppSettings["DefaultFile"];
                    if (string.IsNullOrEmpty(defaultFileName))
                        defaultFileName = "default.aspx";
                }
                else
                {
                    defaultFileName = "default.aspx";
                }

                SPFile defaultFile = myWeb.Files[defaultFileName];

                WebPartManager wpm = new WebPartManager();
                SPLimitedWebPartManager lmWpm= defaultFile.GetLimitedWebPartManager(PersonalizationScope.Shared);

                return lmWpm.WebParts;
            }

            return null;

        }


    }
}
