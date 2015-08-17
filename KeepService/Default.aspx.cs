using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ServiceLibrary;

namespace KeepService
{
    public partial class Default : System.Web.UI.Page
    {
        MainServiceReference.MainServiceSoapClient mainClient = new MainServiceReference.MainServiceSoapClient();
        NodeServiceReference.NodeServiceSoapClient nodeClient = new NodeServiceReference.NodeServiceSoapClient();
        KeepServiceReference.KeepServiceSoapClient keepClient = new KeepServiceReference.KeepServiceSoapClient();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                Calendar1.SelectedDate = DateTime.Now;
            }

        }

        protected void btnDailyInit_Click(object sender, EventArgs e)
        {
            lbResult.Text = mainClient.RunInit();
        }

        protected void btnWorkInit_Click(object sender, EventArgs e)
        {
            lbResult.Text = mainClient.RunWorkInit();
        }

        protected void btnNode_Click(object sender, EventArgs e)
        {
            lbResult.Text = nodeClient.Run();
        }

        protected void btnDaily_Click(object sender, EventArgs e)
        {
            lbResult.Text = keepClient.DailyRateTask(Calendar1.SelectedDate.ToString("yyyy/MM/dd"));

            //StockAnalyser analyser = new StockAnalyser();
            //analyser.DoDailyRate(Calendar1.SelectedDate);

            //using (stockdbaEntities db = new stockdbaEntities())
            //{
            //    DateTime sTime = new DateTime(2013, 11, 05);

            //    List<DateTime> dataList = new List<DateTime>();

            //    dataList.AddRange(db.DailySummary.Where(o => o.receiveDate > sTime).GroupBy(o => o.receiveDate).Select(o=>o.Key));
            //    foreach (var item in dataList)
            //    {
            //        analyser.DoDailyRate(item);
            //    }
            //}
        }

        protected void btnWeek_Click(object sender, EventArgs e)
        {
            lbResult.Text = keepClient.WeeklyRateTask(Calendar1.SelectedDate.ToString("yyyy/MM/dd"));

            //StockAnalyser analyser = new StockAnalyser();
            //analyser.DoWeeklyRate(Calendar1.SelectedDate);

            //using (stockdbaEntities db = new stockdbaEntities())
            //{
            //    DateTime sTime = new DateTime(2013, 10, 09);

            //    List<DateTime> dataList = new List<DateTime>();

            //    dataList.AddRange(db.DailySummary.Where(o => o.receiveDate >= sTime).GroupBy(o => o.receiveDate).Select(o => o.Key));
            //    foreach (var item in dataList)
            //    {
            //        analyser.DoWeeklyRate(item);
            //    }
            //}
        }
    }
}