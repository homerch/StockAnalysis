using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using ServiceLibrary;

namespace MainService
{
    /// <summary>
    ///MainService 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://cocoin.info/MainService")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class MainService : System.Web.Services.WebService
    {
        static ConcurrentStack<DailyWork> workStack = new ConcurrentStack<DailyWork>();
        static Task initTask = new Task(dailyInit);
        static Task workTask = new Task(workInit);

        [WebMethod]
        public WorkItem GetWork()
        {
            WorkItem item = null;
            DateTime receiveDate;
            DailyWork dailyWork;

            try
            {
                if (workStack.Count == 0)
                {
                    using (stockdbaEntities db = new stockdbaEntities())
                    {
                        receiveDate = db.DailyWork.OrderByDescending(o => o.receiveDate).First().receiveDate;
                        workStack.PushRange(db.DailyWork.Where(o => o.currentPage != o.totalPage && o.receiveDate == receiveDate).ToArray());
                    }
                }

                if (workStack.Count != 0)
                {
                    if (workStack.TryPop(out dailyWork) == true)
                    {
                        item = new WorkItem();
                        item.receiveDate = dailyWork.receiveDate;
                        item.stockId = dailyWork.stockId;
                        item.current = (int)dailyWork.currentPage;
                        item.total = (int)dailyWork.totalPage;

                        using (stockdbaEntities db = new stockdbaEntities())
                        {
                            db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = string.Format("GetWork:{0}", item.stockId) });
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (stockdbaEntities db = new stockdbaEntities())
                {
                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("GetWork:{0}", ex.Message) });
                    db.SaveChanges();
                }
            }

            return item;
        }

        [WebMethod]
        public string RunInit()
        {
            TaskStatus before = initTask.Status;
            try
            {
                switch (before)
                {
                    case TaskStatus.Created:
                    case TaskStatus.WaitingToRun:
                        initTask.Start();
                        break;
                    case TaskStatus.Faulted:
                    case TaskStatus.Canceled:
                    case TaskStatus.RanToCompletion:
                        initTask = new Task(dailyInit);
                        initTask.Start();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return String.Format("{0}/{1}", before.ToString(), initTask.Status.ToString());
        }

        [WebMethod]
        public string RunWorkInit()
        {
            TaskStatus before = workTask.Status;
            try
            {
                switch (before)
                {
                    case TaskStatus.Created:
                    case TaskStatus.WaitingToRun:
                        workTask.Start();
                        break;
                    case TaskStatus.Faulted:
                    case TaskStatus.Canceled:
                    case TaskStatus.RanToCompletion:
                        workTask = new Task(workInit);
                        workTask.Start();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return String.Format("{0}/{1}", before.ToString(), workTask.Status.ToString());
        }

        static void dailyInit()
        {
            StockQuery query = new StockQuery();
            List<Task> taskList = new List<Task>();

            taskList.Add(new Task(() => { query.QueryBroker(); }));
            taskList.Add(new Task(() => { query.QueryBrokerBranch(); }));
            taskList.Add(new Task(() => { query.QueryCompany(); }));

            foreach (var item in taskList)
            {
                item.Start();
            }

            Task.WaitAll(taskList.ToArray());

            taskList.Clear();
            taskList.Add(new Task(() => { query.QueryDaliyWork(); }));

            foreach (var item in taskList)
            {
                item.Start();
            }

            Task.WaitAll(taskList.ToArray());
        }

        static void workInit()
        {
            StockQuery query = new StockQuery();
            List<Task> taskList = new List<Task>();

            taskList.Add(new Task(() => { query.QueryDaliyWork(); }));
            foreach (var item in taskList)
            {
                item.Start();
            }
            Task.WaitAll(taskList.ToArray());
        }
    }
}
