using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    public class StockTransfer
    {
        public void Copy(DateTime receiveDate)
        {
            //using (stockdbaEntities db = new stockdbaEntities())
            //{
            //    try
            //    {
            //        using (stockdbaEntitiesLite dbLite = new stockdbaEntitiesLite())
            //        {
            //            //
            //            foreach (var item in db.DailyDetail.Where(o => o.receiveDate == receiveDate))
            //            {
            //                if (dbLite.DailyDetail.Where(o=>o.receiveDate == item.receiveDate && o.stockId == item.stockId && o.no == item.no).Count()==0)
            //                {
            //                    dbLite.DailyDetail.Add(item);
            //                }
            //            }

            //            //
            //            db.SaveChanges();
            //        }
            //        db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("Copy:{0} Done", receiveDate) });
            //        db.SaveChanges();
            //    }
            //    catch (Exception ex)
            //    {
            //        db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("Copy:{0}", ex.Message) });
            //        db.SaveChanges();
            //    }
            //}
        }
    }
}
