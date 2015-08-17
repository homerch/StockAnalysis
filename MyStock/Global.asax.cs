using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using WebApplication;
using ServiceLibrary;

namespace WebApplication
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // 應用程式啟動時執行的程式碼
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterOpenAuth();

            using (stockdbaEntities db = new stockdbaEntities())
            {
                db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = "MyStock_Start" });
                db.SaveChanges();
            }
        }

        void Application_End(object sender, EventArgs e)
        {
            //  應用程式關閉時執行的程式碼
            using (stockdbaEntities db = new stockdbaEntities())
            {
                db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = "MyStock_End" });
                db.SaveChanges();
            }
        }

        void Application_Error(object sender, EventArgs e)
        {
            // 發生未處理錯誤時執行的程式碼

        }
    }
}
