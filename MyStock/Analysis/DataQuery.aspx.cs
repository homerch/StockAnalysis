using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ServiceLibrary;
using System.Web.UI.DataVisualization.Charting;
using System.Threading.Tasks;

namespace WebApplication.Analysis
{
    public partial class DataQuery : System.Web.UI.Page
    {
        DBNoLock db = new DBNoLock();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack != true)
            {
                btnQuery_Click(sender, e);
            }
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            List<QueryStockItem> objAllStock = new List<QueryStockItem>();
            List<QueryStockItem> objTopOB = new List<QueryStockItem>();

            ChartOB1.Visible = false;

            foreach (var item in new Chart[] { ChartOB1 })
            {
                foreach (var pointItem in item.Series)
                {
                    pointItem.Points.Clear();
                }
            }

            var objLastTime = (from a in db.DailySummary select a).OrderBy(o => o.receiveDate).FirstOrDefault();
            if (objLastTime == null)
            {
                return;
            }

            DateTime sTime = DateTime.Parse(objLastTime.receiveDate.AddMonths(-6).ToString("yyyy/MM/01"));
            DateTime eTime;

            // X axis

            var objDaily = (from a in db.DailySummary.Where(o => o.stockId == "0050" && o.receiveDate >= sTime)
                            select a).OrderBy(o => o.receiveDate).ToList();

            sTime = objDaily.First().receiveDate;
            eTime = objDaily.Last().receiveDate;

            objAllStock = (from a in db.DailySettlement.Where(o => o.brokerName == tbBrokerName.Text && o.brokerBranch == tbBrokerBranch.Text && o.stockId == tbStockId.Text
                                                                                                     && o.receiveDate >= sTime && o.receiveDate <= eTime)
                           group a by new
                           {
                               a.stockId,
                               a.stockName
                           } into g
                           select new QueryStockItem
                           {
                               stockId = g.Key.stockId,
                               stockName = g.Key.stockName,
                               tradeAmt = g.Sum(o => o.avgValue * (o.buyVolume + o.sellVolume)),
                               netVolume = g.Sum(o => o.buyVolume - o.sellVolume),
                               avgValue = g.Sum(o => o.avgValue * (o.buyVolume + o.sellVolume)) / g.Sum(o => o.buyVolume + o.sellVolume)
                           }).ToList();

            if (objAllStock.Count > 0)
            {
                Page.Title = tbBrokerName.Text + "(" + tbBrokerBranch.Text + ") " + Page.Title;
                objTopOB.AddRange(objAllStock.Where(o => o.netVolume >= 0).OrderByDescending(o => o.tradeAmt));

                GridView1.DataSource = objTopOB;
                GridView1.DataBind();

                DrawSingleChart(objAllStock.FirstOrDefault().stockName, ChartOB1);
            }
        }

        void DrawSingleChart(String stockName, Chart targetChart)
        {
            List<DateTime> DailyDateList = new List<DateTime>();

            DateTime sTime = DateTime.Now, eTime = DateTime.Now;

            var objLastTime = (from a in db.DailySummary select a).OrderBy(o => o.receiveDate).FirstOrDefault();
            if (objLastTime == null)
            {
                return;
            }

            sTime = DateTime.Parse(objLastTime.receiveDate.AddMonths(-6).ToString("yyyy/MM/01"));
            var objDaily = (from a in db.DailySummary.Where(o => o.Company.name == stockName && o.receiveDate >= sTime)
                            select a).OrderBy(o => o.receiveDate).ToList();

            if (objDaily.Count() != 0)
            {
                sTime = objDaily.First().receiveDate;
                eTime = objDaily.Last().receiveDate;

                double high = 0, close = 0, low = 0, open = 0, volume = 0;

                targetChart.Series["Price"]["PriceUpColor"] = "Red";
                targetChart.Series["Price"]["PriceDownColor"] = "Green";
                targetChart.Titles["Price"].Text = String.Format("{0} ({1})", objDaily.First().Company.name, objDaily.First().stockId);

                //Page.Title = targetChart.Titles["Price"].Text + " " + Page.Title;

                int day = 0;
                foreach (var dataItem in objDaily)
                {
                    high = (double)dataItem.highPrice;
                    low = (double)dataItem.lastPrice;
                    open = (double)dataItem.openPrice;
                    close = (double)dataItem.lastPrice;
                    volume = (double)dataItem.tradeQty;

                    targetChart.Series["Price"].Points.AddXY(day, high);
                    targetChart.Series["Price"].Points[day].XValue = dataItem.receiveDate.ToOADate();
                    targetChart.Series["Price"].Points[day].YValues[1] = low;
                    targetChart.Series["Price"].Points[day].YValues[2] = open;
                    targetChart.Series["Price"].Points[day].YValues[3] = close;
                    //MainChart.Series["Price"].Points[day].ToolTip = string.Format("{0}\nhigh:{1:f2}\nlow:{2:f2}\nopen:{3:f2}\nclose:{4:f2}",
                    //                                                dataItem.receiveDate.ToString("yyyy/MM/dd"), high, low, open, close);

                    day++;
                    DailyDateList.Add(dataItem.receiveDate);
                }
                targetChart.Titles["TimeStamp"].Text = String.Format("統計區間:{0}-{1}", DailyDateList.First().ToShortDateString(), DailyDateList.Last().ToShortDateString());
            }
            else
            {
                // 查無資料
                Response.Redirect(String.Format("~/Analysis/DataQuery.aspx"));
            }


            targetChart.Titles["Price"].Text = String.Format("{0}", stockName);
            targetChart.Visible = true;

            for (int j = 0; j < DailyDateList.Count(); j++)
            {
                foreach (var item in targetChart.Series)
                {
                    if (item.Name == "Price")
                    {
                        continue;
                    }

                    item.Points.AddXY(j, 0);
                    item.Points[j].XValue = DailyDateList[j].ToOADate();
                    if (item.Name == "均價")
                    {
                        item.Points[j].IsEmpty = true;
                    }
                }
            }

            double initDay = DailyDateList[0].ToOADate();

            Parallel.ForEach(db.DailySettlement.Where(o => o.brokerName == tbBrokerName.Text && o.brokerBranch == tbBrokerBranch.Text && o.stockName == stockName && o.receiveDate >= sTime && o.receiveDate <= eTime), dataItem =>
            {
                int index_day = DailyDateList.IndexOf(dataItem.receiveDate);
                if (index_day < 0)
                {
                    return;
                    //break;
                }

                targetChart.Series["均價"].Points[index_day].IsEmpty = false;
                targetChart.Series["均價"].Points[index_day].YValues = new double[] { (double)dataItem.avgValue };
                targetChart.Series["均價"].Points[index_day].ToolTip = string.Format("{0}\n均價:{1:f2}", dataItem.receiveDate.ToString("yyyy/MM/dd"), dataItem.avgValue);

                targetChart.Series["買入"].Points[index_day].YValues = new double[] { (double)dataItem.buyVolume };
                targetChart.Series["買入"].Points[index_day].ToolTip = string.Format("{0}\n買入:{1:f2}", dataItem.receiveDate.ToString("yyyy/MM/dd"), dataItem.buyVolume);

                targetChart.Series["賣出"].Points[index_day].YValues = new double[] { (double)dataItem.sellVolume * -1 };
                targetChart.Series["賣出"].Points[index_day].ToolTip = string.Format("{0}\n賣出:{1:f2}", dataItem.receiveDate.ToString("yyyy/MM/dd"), dataItem.sellVolume);
            });

            double sum = 0;
            for (int j = 0; j < DailyDateList.Count(); j++)
            {
                double buyVolume = targetChart.Series["買入"].Points[j].YValues.First();
                double sellVolume = targetChart.Series["賣出"].Points[j].YValues.First();

                sum = sum + (buyVolume + sellVolume);
                targetChart.Series["合計"].Points[j].YValues = new double[] { sum };
                targetChart.Series["合計"].Points[j].ToolTip = string.Format("{0}\n合計:{1:f2}", DailyDateList[j].ToString("yyyy/MM/dd"), sum);
            }
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowIndex & 1) == 1)
                {
                    e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='#FF9999';";
                    e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='#FFCC99';";
                }
                else
                {
                    e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='#FF9999';";
                    e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='transparent';";
                }
            }
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Clear Data
            foreach (var item in ChartOB1.Series)
            {
                item.Points.Clear();
            }

            // Draw
            GridViewRow row = GridView1.SelectedRow;
            DrawSingleChart(row.Cells[1].Text, ChartOB1);
        }
    }
}