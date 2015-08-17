using ServiceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace MainService
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            using (stockdbaEntities db = new stockdbaEntities())
            {
                db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = "MainService_Start" });
                db.SaveChanges();
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            using (stockdbaEntities db = new stockdbaEntities())
            {
                db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = "MainService_End" });
                db.SaveChanges();
            }
        }
    }
}