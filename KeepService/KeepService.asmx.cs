using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;
using ServiceLibrary;

namespace KeepService
{
    /// <summary>
    ///KeepService 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class KeepService : System.Web.Services.WebService
    {
        //static Task workTask;
        static Task dailyRateTask;
        static Task weeklyRateTask;

        [WebMethod]
        public string ExportCSV(string receiveDate)
        {
            Task workTask;
            try
            {
                workTask = new Task(() => exportTask(receiveDate));
                workTask.Start();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return workTask.Status.ToString();
        }

        void exportTask(string receiveDate)
        {
            StockUtility util = new StockUtility();
            DateTime fileDate = DateTime.Parse(receiveDate);

            util.Export2CSV(Server.MapPath(string.Format("//DailyData//data_export_{0}.csv", fileDate.ToString("yyyyMMdd"))),
                DateTime.Parse(fileDate.ToString("yyyy/MM/dd")));
        }

        //[WebMethod]
        //public string Settlement(string receiveDate)
        //{
        //    TaskStatus before;
        //    try
        //    {
        //        if (workTask == null)
        //        {
        //            workTask = new Task(() => DoDailyRate(receiveDate));
        //        }

        //        before = workTask.Status;
        //        switch (before)
        //        {
        //            case TaskStatus.Created:
        //            case TaskStatus.WaitingToRun:
        //                workTask.Start();
        //                break;
        //            case TaskStatus.Faulted:
        //            case TaskStatus.Canceled:
        //            case TaskStatus.RanToCompletion:
        //                workTask = new Task(() => DoDailyRate(receiveDate));
        //                workTask.Start();
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //    return String.Format("{0}/{1}", before.ToString(), workTask.Status.ToString());
        //}

        //void importTask(string receiveDate)
        //{
        //    StockUtility util = new StockUtility();
        //    DateTime fileDate = DateTime.Parse(receiveDate);

        //    util.ImportDailySettelment(Server.MapPath(string.Format("//DailySettlement//data_import_{0}.csv", fileDate.ToString("yyyyMMdd"))),
        //        DateTime.Parse(fileDate.ToString("yyyy/MM/dd")));
        //}

        [WebMethod]
        public string DailyRateTask(string receiveDate)
        {
            TaskStatus before;
            try
            {
                if (dailyRateTask == null)
                {
                    dailyRateTask = new Task(() => DoDailyRate(receiveDate));
                }

                before = dailyRateTask.Status;
                switch (before)
                {
                    case TaskStatus.Created:
                    case TaskStatus.WaitingToRun:
                        dailyRateTask.Start();
                        break;
                    case TaskStatus.Faulted:
                    case TaskStatus.Canceled:
                    case TaskStatus.RanToCompletion:
                        dailyRateTask = new Task(() => DoDailyRate(receiveDate));
                        dailyRateTask.Start();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return String.Format("{0}/{1}", before.ToString(), dailyRateTask.Status.ToString());
        }

        void DoDailyRate(string receiveDate)
        {
            StockAnalyser analyser = new StockAnalyser();
            analyser.DoDailyRate(DateTime.Parse(receiveDate));
        }

        [WebMethod]
        public string WeeklyRateTask(string receiveDate)
        {
            TaskStatus before;
            try
            {
                if (weeklyRateTask == null)
                {
                    weeklyRateTask = new Task(() => DoWeeklyRate(receiveDate));
                }

                before = weeklyRateTask.Status;
                switch (before)
                {
                    case TaskStatus.Created:
                    case TaskStatus.WaitingToRun:
                        weeklyRateTask.Start();
                        break;
                    case TaskStatus.Faulted:
                    case TaskStatus.Canceled:
                    case TaskStatus.RanToCompletion:
                        weeklyRateTask = new Task(() => DoWeeklyRate(receiveDate));
                        weeklyRateTask.Start();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return String.Format("{0}/{1}", before.ToString(), weeklyRateTask.Status.ToString());
        }

        void DoWeeklyRate(string receiveDate)
        {
            StockAnalyser analyser = new StockAnalyser();
            //analyser.DoWeeklyRate(DateTime.Parse(receiveDate));

            using (stockdbaEntities db = new stockdbaEntities())
            {
                DateTime sTime = DateTime.Parse(receiveDate);

                List<DateTime> dataList = new List<DateTime>();

                dataList.AddRange(db.DailySummary.Where(o => o.receiveDate >= sTime).GroupBy(o => o.receiveDate).Select(o => o.Key));
                foreach (var item in dataList)
                {
                    analyser.DoWeeklyRate(item);
                }
            }
        }
    }
}
