using ServiceLibrary;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataControl
{
    public partial class Form1 : Form
    {
        static int taskNum = 0;
        static ConcurrentStack<DailyWork> workStack = new ConcurrentStack<DailyWork>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dtpRate.Value = DateTime.Parse(DateTime.Now.ToShortDateString());

            //if (Environment.GetCommandLineArgs().Length > 1)
            //{
            //    if (AutoWorker.IsBusy == false)
            //    {
            //        AutoWorker.RunWorkerAsync();
            //    }
            //}

            // 計算啟動時間
            //TimeSpan startInterval = DateTime.Now - DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd 16:00:00"));
            //MainTimer.Interval = startInterval.Milliseconds;
            //MainTimer.Enabled = true;

        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            if (MainWork.IsBusy == false)
            {
                MainWork.RunWorkerAsync();
            }

            // 計算下次啟動時間
            TimeSpan startInterval = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy/MM/dd 16:00:00")) - DateTime.Now;
            MainTimer.Interval = startInterval.Milliseconds;
        }

        private void MainWork_DoWork(object sender, DoWorkEventArgs e)
        {
            StockQuery query = new StockQuery();
            List<Task> taskList = new List<Task>();

            // 檢查是否為交易日

            // Step 1. Query Basic Data
            taskList.Add(new Task(() => { query.QueryBroker(); }));
            taskList.Add(new Task(() => { query.QueryBrokerBranch(); }));
            taskList.Add(new Task(() => { query.QueryCompany(); }));

            foreach (var item in taskList)
            {
                item.Start();
            }

            Task.WaitAll(taskList.ToArray());
            taskList.Clear();

            // Step 2. Query Stock Summary
            taskList.Add(new Task(() => { query.QueryDaliyWork(); }));

            foreach (var item in taskList)
            {
                item.Start();
            }

            Task.WaitAll(taskList.ToArray());


            // Check Progress
            // Daily Settlement
            // Daily Rate
            // Weekly Rate
        }

        private void DetailTimer_Tick(object sender, EventArgs e)
        {
            // Query Stock Detail


        }

        private void SummaryTimer_Tick(object sender, EventArgs e)
        {

        }

        private void btnWeeklyRate_Click(object sender, EventArgs e)
        {
            if (RateWorker.IsBusy == false)
            {
                RateWorker.RunWorkerAsync(dtpRate.Value);
            }
        }

        private void RateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                DateTime sTime = (DateTime)e.Argument;

                StockAnalyser analyser = new StockAnalyser();
                List<DateTime> dataList = new List<DateTime>();

                RateWorker.ReportProgress(0, "Rate Start");

                using (stockdbaEntities db = new stockdbaEntities())
                {
                    dataList.AddRange(db.DailySummary.Where(o => o.receiveDate >= sTime).GroupBy(o => o.receiveDate).Select(o => o.Key));
                }

                foreach (var item in dataList)
                //Parallel.ForEach(dataList, item =>
                {
                    RateWorker.ReportProgress(0, item);

                    analyser.DoDailyRate(item);

                    analyser.DoWeeklyRate(item);

                    analyser.DoTotalRate(item);
                }//);

                RateWorker.ReportProgress(0, "Rate Done");
            }
            catch (Exception ex)
            {
                RateWorker.ReportProgress(0, ex.Message);
            }
        }

        private void RateWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            listBox1.Items.Insert(0, string.Format("{0}:{1}", DateTime.Now.ToString("MM/dd hh:mm:ss.fff"), e.UserState));
        }

        private void btnTest_Click(object sender, EventArgs e)
        {

        }

        private void TestWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                TestWorker.ReportProgress(0, "Query Start");
                StockQuery query = new StockQuery();
                query.QueryOTC();
                query.QueryOTCWarrant(DateTime.Now);
                TestWorker.ReportProgress(0, "Query End");
            }
            catch (Exception ex)
            {
                TestWorker.ReportProgress(0, ex.Message);
            }
        }

        private void TestWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            listBox3.Items.Insert(0, string.Format("{0}:{1}", DateTime.Now.ToString("MM/dd hh:mm:ss.fff"), e.UserState));
        }

        private void TestWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void InitWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            StockQuery query = new StockQuery();
            switch ((int)e.Argument)
            {
                case 0:
                    InitWorker.ReportProgress(0, "BasicData Start");
                    query.QueryBroker();
                    query.QueryBrokerBranch();
                    query.QueryCompany();
                    //
                    query.QueryDaliyWork();
                    query.QueryStockWarrant();
                    InitWorker.ReportProgress(0, "BasicData Done");
                    break;
                case 1:
                    InitWorker.ReportProgress(0, "StockData Start");
                    query.QueryDaliyWork();
                    query.QueryStockWarrant();
                    InitWorker.ReportProgress(0, "StockData Done");
                    break;
                default:
                    break;
            }
        }

        private void InitWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lbStock.Items.Insert(0, string.Format("{0}:{1}", DateTime.Now.ToString("MM/dd hh:mm:ss.fff"), e.UserState));
        }

        private void InitWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void btnOQuery_Click(object sender, EventArgs e)
        {
            if (TestWorker.IsBusy == false)
            {
                TestWorker.RunWorkerAsync();
            }
        }

        private void btnBasic_Click(object sender, EventArgs e)
        {
            if (InitWorker.IsBusy == false)
            {
                InitWorker.RunWorkerAsync(0);
            }
        }

        private void btnStock_Click(object sender, EventArgs e)
        {
            if (InitWorker.IsBusy == false)
            {
                InitWorker.RunWorkerAsync(1);
            }
        }

        private void btnSQuery_Click(object sender, EventArgs e)
        {
            if (QueryWorker.IsBusy == false)
            {
                QueryWorker.RunWorkerAsync();
            }
        }

        private void QueryWorker_DoWork(object sender, DoWorkEventArgs e)
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

                                    QueryWorker.ReportProgress(0, string.Format("Run:{0}:{1}/{2}", item.stockId, i, item.totalPage));

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

                                        QueryWorker.ReportProgress(0, string.Format("Run:{0}: Settlement", item.stockId));
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

        private void QueryWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lbStock.Items.Insert(0, string.Format("{0}:{1}", DateTime.Now.ToString("MM/dd hh:mm:ss.fff"), e.UserState));
        }

        private void QueryWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void btnTotalRate_Click(object sender, EventArgs e)
        {
            //StockAnalyser analyser = new StockAnalyser();
            //analyser.DoTotalRate(dtpRate.Value);
        }

        private void SWWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                SWWorker.ReportProgress(0, "Query Start");
                StockQuery query = new StockQuery();
                query.QueryStockWarrant();
                SWWorker.ReportProgress(0, "Query End");
            }
            catch (Exception ex)
            {
                SWWorker.ReportProgress(0, ex.Message);
            }
        }

        private void OWWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                OWWorker.ReportProgress(0, "Query Start");
                StockQuery query = new StockQuery();
                query.QueryOTCWarrant(DateTime.Now);
                OWWorker.ReportProgress(0, "Query End");
            }
            catch (Exception ex)
            {
                OWWorker.ReportProgress(0, ex.Message);
            }
        }

        private void SWWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lbSW.Items.Insert(0, string.Format("{0}:{1}", DateTime.Now.ToString("MM/dd hh:mm:ss.fff"), e.UserState));
        }

        private void SWWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void OWWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lbOW.Items.Insert(0, string.Format("{0}:{1}", DateTime.Now.ToString("MM/dd hh:mm:ss.fff"), e.UserState));
        }

        private void OWWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void btnSWQuery_Click(object sender, EventArgs e)
        {
            if (SWWorker.IsBusy == false)
            {
                SWWorker.RunWorkerAsync();
            }
        }

        private void btnOWQuery_Click(object sender, EventArgs e)
        {
            if (OWWorker.IsBusy == false)
            {
                OWWorker.RunWorkerAsync();
            }

        }
    }
}
