using ServiceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleControl
{
    enum MyEnum
    {
        Stage1,
        Stage1A,
        Stage2,
        Stage2A,
        Stage3,
        Stage3A
    }

    class Program
    {
        static void Main(string[] args)
        {
            MyEnum workType;
            StockQuery query = new StockQuery();

            if (args.Length > 0)
            {
                Logger("---------------START---------------");
                if (Enum.TryParse(args[0], true, out workType) == true)
                {
                    switch (workType)
                    {
                        case MyEnum.Stage1:
                            StockUtility util = new StockUtility();

                            Logger("Reset");
                            util.Reset();

                            //Logger("QueryBroker");
                            //query.QueryBroker();

                            //Logger("QueryBrokerBranch");
                            //query.QueryBrokerBranch();

                            //Logger("QueryCompany");
                            //query.QueryCompany();

                            Logger("QueryDaliyWork");
                            query.QueryDaliyWork();

                            Logger("QueryStockWarrant");
                            query.QueryStockWarrant();
                            break;

                        case MyEnum.Stage1A:
                            Logger("QueryDaliyWork");
                            query.QueryDaliyWork();

                            Logger("QueryStockWarrant");
                            query.QueryStockWarrant();
                            break;

                        case MyEnum.Stage2:
                        case MyEnum.Stage2A:
                            DetailQuery();
                            break;

                        case MyEnum.Stage3:
                        case MyEnum.Stage3A:
                            Logger("QueryOTC");
                            query.QueryOTC();

                            Logger("QueryOTCWarrant");
                            query.QueryOTCWarrant(DateTime.Now);
                            break;

                        default:
                            break;
                    }
                }
                Logger("----------------END----------------");
            }
        }

        static void Logger(string str)
        {
            Console.WriteLine(string.Format("{0}:{1}", DateTime.Now.ToString("MM/dd hh:mm:ss.fff"), str));
        }

        static void DetailQuery()
        {
            StockQuery worker = new StockQuery();
            StockAnalyser analyser = new StockAnalyser();

            DateTime receiveDate;
            List<DailyWork> workList = new List<DailyWork>();

            ParallelOptions option = new ParallelOptions();
            option.MaxDegreeOfParallelism = 10;

            int retryCnt = 0;
            Random rnd = new Random();

            while (true)
            {
                using (stockdbaEntities db = new stockdbaEntities())
                {
                    receiveDate = db.DailyWork.OrderByDescending(o => o.receiveDate).First().receiveDate;
                    workList.AddRange(db.DailyWork.Where(o => o.currentPage != o.totalPage && o.receiveDate == receiveDate));
                }

                if (workList.Count() > 0)
                {
                    Parallel.ForEach(workList, option, item =>
                    {
                        try
                        {
                            Thread.Sleep(rnd.Next(100, 1000));
                            for (int i = (int)item.currentPage + 1; i <= (int)item.totalPage; i++)
                            {
                                worker.QuerySinglePage(i, item.stockId);
                                using (stockdbaEntities db = new stockdbaEntities())
                                {
                                    db.DailyWork.Where(o => o.receiveDate == item.receiveDate && o.stockId == item.stockId).First().currentPage = i;
                                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = string.Format("Run:{0}:{1}/{2}", item.stockId, i, item.totalPage) });
                                    db.SaveChanges();

                                    Logger(string.Format("Run:{0}:{1}/{2}", item.stockId, i, item.totalPage));

                                    if (i == item.totalPage)
                                    {
                                        string stockName = "-";
                                        var obj = db.Company.Where(o => o.stockId == item.stockId).FirstOrDefault();
                                        if (obj != null)
                                        {
                                            stockName = obj.name;
                                        }
                                        // 結算
                                        analyser.DoSettlement(item.receiveDate, item.stockId);
                                        // 集中率
                                        analyser.DoDailyRate(item.receiveDate, item.stockId, stockName);
                                        analyser.DoWeeklyRate(item.receiveDate, item.stockId, stockName);
                                        analyser.DoTotalRate(item.receiveDate, item.stockId, stockName);

                                        Logger(string.Format("Run:{0}: Settlement", item.stockId));
                                    }
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
                    });
                    workList.Clear();
                }
                else
                {
                    if (retryCnt < 60)
                    {
                        retryCnt++;
                        Thread.Sleep(60000); // 1min
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}
