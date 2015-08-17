using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Threading;
using System.Threading.Tasks;
using ServiceLibrary;

namespace NodeService
{
    /// <summary>
    ///NodeService 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://cocoin.info/NodeService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class NodeService : System.Web.Services.WebService
    {
        static int taskNum = 0;

        [WebMethod]
        public string Run()
        {
            Task nodeTask = new Task(() =>
            {
                MainServiceReference.MainServiceSoapClient client = new MainServiceReference.MainServiceSoapClient();
                MainServiceReference.WorkItem item;

                StockQuery worker = new StockQuery();

                while ((item = client.GetWork()) != null)
                {
                    try
                    {
                        using (stockdbaEntities db = new stockdbaEntities())
                        {
                            db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = string.Format("Run:{0}", item.stockId) });
                            db.SaveChanges();
                        }

                        for (int i = item.current + 1; i <= item.total; i++)
                        {
                            worker.QuerySinglePage(i, item.stockId);
                            using (stockdbaEntities db = new stockdbaEntities())
                            {
                                db.DailyWork.Where(o => o.receiveDate == item.receiveDate && o.stockId == item.stockId).First().currentPage = i;
                                db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = string.Format("Run:{0}:{1}/{2}", item.stockId, i, item.total) });
                                db.SaveChanges();
                            }
                            Thread.Sleep(10000);
                        }
                    }
                    catch (Exception ex)
                    {
                        using (stockdbaEntities db = new stockdbaEntities())
                        {
                            db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("Run:{0}", ex.Message) });
                            db.SaveChanges();
                        }
                    }
                }
                Interlocked.Decrement(ref taskNum);
            });

            if (taskNum < 10)
            {
                nodeTask.Start();
                Interlocked.Increment(ref taskNum);
                return String.Format("New Task, TaskNum:{0}", taskNum);
            }

            return String.Format("TaskNum:{0}", taskNum);
        }
    }
}
