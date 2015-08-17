using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.DataVisualization.Charting;
using System.Threading;
using System.Threading.Tasks;
using ServiceLibrary;
using System.Data.Objects;
using System.Data.Entity;
using System.Collections.Concurrent;

namespace WebApplication.Analysis
{
    public partial class Lab_Branch_Lite : System.Web.UI.Page
    {
        private class BrokerItem
        {
            public string Name;
            public string Branch;
        }

        DBNoLock db = new DBNoLock();

        List<DateTime> DailyDateList = new List<DateTime>();
        List<BrokerItem> BrokerList = new List<BrokerItem>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack != true)
            {
                if (Request.QueryString["stockId"] != null)
                {
                    tbStockId.Text = Request.QueryString["stockId"];
                }
                btnQuery_Click(sender, e);
            }
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            List<QueryBrokerItem> objAllBroker = new List<QueryBrokerItem>();
            List<QueryBrokerItem> objTopOB = new List<QueryBrokerItem>();
            List<QueryBrokerItem> objTopOS = new List<QueryBrokerItem>();
            DateTime sTime = DateTime.Now, eTime = DateTime.Now;

            try
            {
                ChartOB1.Visible = false;
                ChartOS1.Visible = false;
                ChartFocus.Visible = false;

                foreach (var item in new Chart[] { MainChart, ChartOB1, ChartOS1, ChartBubble, ChartFocus, ChartBubbleDay })//, ChartBubbleWeek })
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

                sTime = DateTime.Parse(objLastTime.receiveDate.AddMonths(-6).ToString("yyyy/MM/01"));
                var objDaily = (from a in db.DailySummary.Where(o => o.stockId == tbStockId.Text && o.receiveDate >= sTime)
                                select a).OrderBy(o => o.receiveDate).ToList();

                if (objDaily.Count() != 0)
                {
                    sTime = objDaily.First().receiveDate;
                    eTime = objDaily.Last().receiveDate;

                    double high = 0, close = 0, low = 0, open = 0, volume = 0;

                    MainChart.Series["Price"]["PriceUpColor"] = "Red";
                    MainChart.Series["Price"]["PriceDownColor"] = "Green";
                    MainChart.Titles["Price"].Text = String.Format("{0} ({1})", objDaily.First().Company.name, tbStockId.Text);

                    Page.Title = MainChart.Titles["Price"].Text + " " + Page.Title;

                    int day = 0;
                    foreach (var dataItem in objDaily)
                    {
                        high = (double)dataItem.highPrice;
                        low = (double)dataItem.lastPrice;
                        open = (double)dataItem.openPrice;
                        close = (double)dataItem.lastPrice;
                        volume = (double)dataItem.tradeQty;

                        MainChart.Series["Price"].Points.AddXY(day, high);
                        MainChart.Series["Price"].Points[day].XValue = dataItem.receiveDate.ToOADate();
                        MainChart.Series["Price"].Points[day].YValues[1] = low;
                        MainChart.Series["Price"].Points[day].YValues[2] = open;
                        MainChart.Series["Price"].Points[day].YValues[3] = close;
                        MainChart.Series["Price"].Points[day].ToolTip = string.Format("{0}\nhigh:{1:f2}\nlow:{2:f2}\nopen:{3:f2}\nclose:{4:f2}",
                                                                        dataItem.receiveDate.ToString("yyyy/MM/dd"), high, low, open, close);
                        // Set volume values
                        MainChart.Series["Volume"].Points.AddXY(day, volume);
                        MainChart.Series["Volume"].Points[day].XValue = dataItem.receiveDate.ToOADate();
                        MainChart.Series["Volume"].Points[day].ToolTip = string.Format("{0}\nhigh:{1:f2}\nlow:{2:f2}\nopen:{3:f2}\nclose:{4:f2}\nvolume:{5:f2}",
                                                                               dataItem.receiveDate.ToString("yyyy/MM/dd"), high, low, open, close, volume);
                        MainChart.Series["Volume"].Points[day].PostBackValue = string.Format("{0},{1}", dataItem.receiveDate.ToString("yyyy/MM/dd"), tbStockId.Text);

                        // Set the X-axis of Rate
                        MainChart.Series["日集中度"].Points.AddXY(day, 0);
                        MainChart.Series["日集中度"].Points[day].XValue = dataItem.receiveDate.ToOADate();
                        MainChart.Series["日集中度"].Points[day].IsEmpty = true;

                        MainChart.Series["週集中度"].Points.AddXY(day, 0);
                        MainChart.Series["週集中度"].Points[day].XValue = dataItem.receiveDate.ToOADate();
                        MainChart.Series["週集中度"].Points[day].IsEmpty = true;

                        MainChart.Series["總集中度"].Points.AddXY(day, 0);
                        MainChart.Series["總集中度"].Points[day].XValue = dataItem.receiveDate.ToOADate();
                        MainChart.Series["總集中度"].Points[day].IsEmpty = true;

                        day++;
                        DailyDateList.Add(dataItem.receiveDate);
                    }
                    MainChart.Titles["TimeStamp"].Text = String.Format("統計區間:{0}-{1}", DailyDateList.First().ToShortDateString(), DailyDateList.Last().ToShortDateString());
                }
                else
                {
                    // 查無資料
                    Response.Redirect(String.Format("~/Analysis/Lab_Branch_Lite.aspx"));
                }

                objAllBroker = (from a in db.DailySettlement.Where(o => o.stockId == tbStockId.Text && o.receiveDate >= sTime && o.receiveDate <= eTime)
                                group a by new
                                {
                                    a.brokerName,
                                    a.brokerBranch
                                } into g
                                select new QueryBrokerItem
                                {
                                    brokerName = g.Key.brokerName,
                                    brokerBranch = g.Key.brokerBranch,
                                    netVolume = g.Sum(o => o.buyVolume - o.sellVolume),
                                    totalVolume = g.Sum(o => o.buyVolume),
                                    avgValue = g.Sum(o => o.avgValue * (o.buyVolume + o.sellVolume)) / g.Sum(o => o.buyVolume + o.sellVolume)
                                }).OrderByDescending(o => o.netVolume).ToList();


                var buyTop15 = (from a in objAllBroker.Where(o => o.netVolume > 0) select a).OrderByDescending(o => o.netVolume).Take(15);
                var sellTop15 = (from a in objAllBroker.Where(o => o.netVolume < 0) select a).OrderBy(o => o.netVolume).Take(15);

                objTopOB.AddRange(buyTop15);
                objTopOS.AddRange(sellTop15);

                foreach (var item in objTopOB)
                {

                    var dummy = (from a in db.DailySettlement.Where(o => o.stockId == tbStockId.Text && o.receiveDate == eTime &&
                                                                    o.brokerBranch == item.brokerBranch && o.brokerName == item.brokerName)
                                 select a).FirstOrDefault();
                    if (dummy != null)
                    {
                        item.latestVolume = dummy.buyVolume - dummy.sellVolume;
                    }
                    else
                    {
                        item.latestVolume = 0;
                    }

                }

                foreach (var item in objTopOS)
                {

                    var dummy = (from a in db.DailySettlement.Where(o => o.stockId == tbStockId.Text && o.receiveDate == eTime &&
                                                                    o.brokerBranch == item.brokerBranch && o.brokerName == item.brokerName)
                                 select a).FirstOrDefault();
                    if (dummy != null)
                    {
                        item.latestVolume = dummy.buyVolume - dummy.sellVolume;
                    }
                    else
                    {
                        item.latestVolume = 0;
                    }
                }

                //
                GridView1.DataSource = objTopOB;
                GridView1.DataBind();

                GridView2.DataSource = objTopOS;
                GridView2.DataBind();

                ChartBubble.Titles["Price"].Text = String.Format("{0}", MainChart.Titles["Price"].Text);
                ChartBubble.Titles["TimeStamp"].Text = String.Format("統計區間:{0}-{1}", sTime.ToShortDateString(), eTime.ToShortDateString());
                ChartBubble.Visible = true;

                foreach (var item in objAllBroker)
                {
                    double netVolume = (double)item.netVolume;
                    if (netVolume > 0)
                    {
                        ChartBubble.Series["總買超"].Points.Add(
                            new DataPoint()
                            {
                                XValue = netVolume,
                                YValues = new double[] { (double)item.avgValue, netVolume },
                                ToolTip = String.Format("{0}-{1}:{2}", item.brokerName, item.brokerBranch, netVolume),
                                PostBackValue = String.Format("{0}-{1}", item.brokerName, item.brokerBranch)
                            });
                    }
                    else if (netVolume < 0)
                    {
                        ChartBubble.Series["總賣超"].Points.Add(
                            new DataPoint()
                            {
                                XValue = netVolume,
                                YValues = new double[] { (double)item.avgValue, Math.Abs(netVolume) },
                                ToolTip = String.Format("{0}-{1}:{2}", item.brokerName, item.brokerBranch, Math.Abs(netVolume)),
                                PostBackValue = String.Format("{0}-{1}", item.brokerName, item.brokerBranch)
                            });
                    }
                }

                double iRate = (double)(objTopOB.Sum(o => o.netVolume) + objTopOS.Sum(o => o.netVolume)) * 100 / objAllBroker.Sum(o => (double)o.totalVolume);

                if (objAllBroker.Count > 0)
                {
                    if (iRate > 0)
                    {
                        ChartBubble.Series["總買超"].Points.First().Label = string.Format("{0:F2}%", Math.Abs(iRate));
                    }
                    else
                    {
                        ChartBubble.Series["總賣超"].Points.Last().Label = string.Format("{0:F2}%", Math.Abs(iRate));
                    }

                    DrawBubble(ChartBubble, DailyDateList.Last(), DailyDateList.Last());
                    DrawBubble(ChartBubble, DailyDateList.Skip(DailyDateList.Count() - 5).First(), DailyDateList.Last());
                    DrawDailyRate(sTime);
                    DrawWeeklyRate(sTime);
                    DrawTotalRate(sTime);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        void DrawBubble(Chart targetChart, DateTime sTime, DateTime eTime)
        {
            List<QueryBrokerItem> objAllBroker = new List<QueryBrokerItem>();

            String prefix = "日";
            if (sTime != eTime)
            {
                prefix = "週";
            }

            objAllBroker = (from a in db.DailySettlement.Where(o => o.stockId == tbStockId.Text && o.receiveDate >= sTime && o.receiveDate <= eTime)
                            group a by new
                            {
                                a.brokerName,
                                a.brokerBranch
                            } into g
                            select new QueryBrokerItem
                            {
                                brokerName = g.Key.brokerName,
                                brokerBranch = g.Key.brokerBranch,
                                netVolume = g.Sum(o => o.buyVolume - o.sellVolume),
                                totalVolume = g.Sum(o => o.buyVolume),
                                avgValue = g.Sum(o => o.avgValue * (o.buyVolume + o.sellVolume)) / g.Sum(o => o.buyVolume + o.sellVolume)
                            }).OrderByDescending(o => o.netVolume).ToList();

            //targetChart.Titles["Price"].Text = String.Format("{0}", MainChart.Titles["Price"].Text);
            targetChart.Titles[prefix + "TimeStamp"].Text = String.Format("統計區間:{0}-{1}", sTime.ToShortDateString(), eTime.ToShortDateString());
            targetChart.Visible = true;

            foreach (var item in objAllBroker)
            {
                double netVolume = (double)item.netVolume;
                if (netVolume > 0)
                {
                    targetChart.Series[prefix + "買超"].Points.Add(
                        new DataPoint()
                        {
                            XValue = netVolume,
                            YValues = new double[] { (double)item.avgValue, netVolume },
                            ToolTip = String.Format("{0}-{1}:{2}", item.brokerName, item.brokerBranch, netVolume),
                            PostBackValue = String.Format("{0}-{1}", item.brokerName, item.brokerBranch)
                        });
                }
                else if (netVolume < 0)
                {
                    targetChart.Series[prefix + "賣超"].Points.Add(
                        new DataPoint()
                        {
                            XValue = netVolume,
                            YValues = new double[] { (double)item.avgValue, Math.Abs(netVolume) },
                            ToolTip = String.Format("{0}-{1}:{2}", item.brokerName, item.brokerBranch, Math.Abs(netVolume)),
                            PostBackValue = String.Format("{0}-{1}", item.brokerName, item.brokerBranch)
                        });
                }
            }

            if (objAllBroker.Count > 0)
            {
                double iTotal = (double)objAllBroker.Sum(o => o.totalVolume);
                var buyTop15 = (from a in objAllBroker.Where(o => o.netVolume > 0) select a).OrderByDescending(o => o.netVolume).Take(15).Sum(o => o.netVolume);
                var sellTop15 = (from a in objAllBroker.Where(o => o.netVolume < 0) select a).OrderBy(o => o.netVolume).Take(15).Sum(o => o.netVolume);

                if (buyTop15 == null)
                {
                    buyTop15 = 0;
                }

                if (sellTop15 == null)
                {
                    sellTop15 = 0;
                }

                double iRate = (double)(buyTop15 + sellTop15) * 100 / iTotal;

                if (iRate > 0)
                {
                    if (targetChart.Series[prefix + "買超"].Points.Count > 0)
                    {
                        targetChart.Series[prefix + "買超"].Points.First().Label = string.Format("{0:F2}%", Math.Abs(iRate));
                    }
                }
                else
                {
                    if (targetChart.Series[prefix + "賣超"].Points.Count > 0)
                    {
                        targetChart.Series[prefix + "賣超"].Points.Last().Label = string.Format("{0:F2}%", Math.Abs(iRate));
                    }
                }
            }
        }

        void DrawSingleChart(BrokerItem bItem, Chart targetChart)
        {
            DateTime sTime = DateTime.Now, eTime = DateTime.Now;

            sTime = DailyDateList.First();
            eTime = DailyDateList.Last();

            targetChart.Titles["Price"].Text = String.Format("{0}-{1}", bItem.Name, bItem.Branch);
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
            Parallel.ForEach(db.DailySettlement.Where(o => o.stockId == tbStockId.Text && o.brokerName == bItem.Name && o.brokerBranch == bItem.Branch && o.receiveDate >= sTime && o.receiveDate <= eTime), dataItem =>
            //foreach (var dataItem in db.DailySettlement.Where(o => o.stockId == tbStockId.Text && o.brokerName == bItem.Name && o.brokerBranch == bItem.Branch && o.receiveDate >= sTime && o.receiveDate <= eTime))
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
                // -------------------------------------------------------------
                int changeIndex = 0;
                foreach (DataControlField item in GridView1.Columns)
                {
                    if (item.HeaderText.Contains("近日買賣超"))
                    {
                        changeIndex = GridView1.Columns.IndexOf(item);
                        break;
                    }
                }
                if (float.Parse(e.Row.Cells[changeIndex].Text) >= 0)
                {
                    e.Row.Cells[changeIndex].ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    e.Row.Cells[changeIndex].ForeColor = System.Drawing.Color.Green;
                }
            }
        }

        protected void GridView2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if ((e.Row.RowIndex & 1) == 1)
                {
                    e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='#99CCFF';";
                    e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='#CCFF99';";
                }
                else
                {
                    e.Row.Attributes["onmouseover"] = "this.style.backgroundColor='#99CCFF';";
                    e.Row.Attributes["onmouseout"] = "this.style.backgroundColor='transparent';";
                }
                // -------------------------------------------------------------
                int changeIndex = 0;
                foreach (DataControlField item in GridView1.Columns)
                {
                    if (item.HeaderText.Contains("近日買賣超"))
                    {
                        changeIndex = GridView1.Columns.IndexOf(item);
                        break;
                    }
                }
                if (float.Parse(e.Row.Cells[changeIndex].Text) >= 0)
                {
                    e.Row.Cells[changeIndex].ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    e.Row.Cells[changeIndex].ForeColor = System.Drawing.Color.Green;
                }
            }
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // X axis
            for (int i = 0; i < MainChart.Series[0].Points.Count(); i++)
            {
                DailyDateList.Add(DateTime.FromOADate(MainChart.Series[0].Points[i].XValue));
            }

            // Clear Data
            foreach (var item in ChartOB1.Series)
            {
                item.Points.Clear();
            }

            // Draw
            GridViewRow row = GridView1.SelectedRow;
            DrawSingleChart(new BrokerItem()
            {
                Name = row.Cells[0].Text,
                Branch = row.Cells[1].Text
            }, ChartOB1);

            // Copy
            ChartOB1.Series["Price"]["PriceUpColor"] = "Red";
            ChartOB1.Series["Price"]["PriceDownColor"] = "Green";
            foreach (var item in MainChart.Series["Price"].Points)
            {
                ChartOB1.Series["Price"].Points.Add(item);
            }
        }

        protected void GridView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // X axis
            for (int i = 0; i < MainChart.Series[0].Points.Count(); i++)
            {
                DailyDateList.Add(DateTime.FromOADate(MainChart.Series[0].Points[i].XValue));
            }

            // Clear Data
            foreach (var item in ChartOS1.Series)
            {
                item.Points.Clear();
            }

            GridViewRow row = GridView2.SelectedRow;
            DrawSingleChart(new BrokerItem()
            {
                Name = row.Cells[0].Text,
                Branch = row.Cells[1].Text
            }, ChartOS1);

            // Copy
            ChartOS1.Series["Price"]["PriceUpColor"] = "Red";
            ChartOS1.Series["Price"]["PriceDownColor"] = "Green";
            foreach (var item in MainChart.Series["Price"].Points)
            {
                ChartOS1.Series["Price"].Points.Add(item);
            }
        }

        protected void MainChart_Click(object sender, ImageMapEventArgs e)
        {
            try
            {
                DateTime targetDate = DateTime.Parse(e.PostBackValue.Split(',')[0]);

                // Clear Data
                foreach (var item in ChartBubbleDay.Series)
                {
                    item.Points.Clear();
                }

                // Day
                DrawBubble(ChartBubbleDay, targetDate, targetDate);
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected void ChartBubble_Click(object sender, ImageMapEventArgs e)
        {
            try
            {
                var paras = e.PostBackValue.Split('-');

                // X axis
                for (int i = 0; i < MainChart.Series[0].Points.Count(); i++)
                {
                    DailyDateList.Add(DateTime.FromOADate(MainChart.Series[0].Points[i].XValue));
                }

                // Clear Data
                foreach (var item in ChartFocus.Series)
                {
                    item.Points.Clear();
                }

                // Draw
                GridViewRow row = GridView1.SelectedRow;
                DrawSingleChart(new BrokerItem()
                {
                    Name = paras[0],
                    Branch = paras[1]
                }, ChartFocus);

                // Copy
                ChartFocus.Series["Price"]["PriceUpColor"] = "Red";
                ChartFocus.Series["Price"]["PriceDownColor"] = "Green";
                foreach (var item in MainChart.Series["Price"].Points)
                {
                    ChartFocus.Series["Price"].Points.Add(item);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        void DrawDailyRate(DateTime sTime)
        {
            //sTime = DateTime.Parse(DateTime.UtcNow.AddHours(8).AddMonths(-3).ToString("yyyy/MM/01"));
            var objDaily = (from a in db.DailyRate.Where(o => o.stockId == tbStockId.Text && o.receiveDate >= sTime)
                            select a).OrderBy(o => o.receiveDate).ToList();

            if (objDaily.Count() != 0)
            {
                sTime = objDaily.First().receiveDate;
                foreach (var dataItem in objDaily)
                {
                    foreach (var item in MainChart.Series["日集中度"].Points)
                    {
                        if (item.XValue == dataItem.receiveDate.ToOADate())
                        {
                            item.IsEmpty = false;
                            item.YValues = new double[] { (double)dataItem.rate };
                            item.ToolTip = string.Format("{0}\n日集中度:{1:f2}", dataItem.receiveDate.ToString("yyyy/MM/dd"), (double)dataItem.rate);
                            break;
                        }
                    }
                }
            }
        }

        void DrawWeeklyRate(DateTime sTime)
        {
            //sTime = DateTime.Parse(DateTime.UtcNow.AddHours(8).AddMonths(-3).ToString("yyyy/MM/01"));
            var objDaily = (from a in db.WeeklyRate.Where(o => o.stockId == tbStockId.Text && o.receiveDate >= sTime)
                            select a).OrderBy(o => o.receiveDate).ToList();

            if (objDaily.Count() != 0)
            {
                sTime = objDaily.First().receiveDate;
                foreach (var dataItem in objDaily)
                {
                    foreach (var item in MainChart.Series["週集中度"].Points)
                    {
                        if (item.XValue == dataItem.receiveDate.ToOADate())
                        {
                            item.IsEmpty = false;
                            item.YValues = new double[] { (double)dataItem.rate };
                            item.ToolTip = string.Format("{0}\n週集中度:{1:f2}", dataItem.receiveDate.ToString("yyyy/MM/dd"), (double)dataItem.rate);
                            break;
                        }
                    }
                }
            }
        }

        void DrawTotalRate(DateTime sTime)
        {

            var objDaily = (from a in db.TotalRate.Where(o => o.stockId == tbStockId.Text && o.receiveDate >= sTime)
                            select a).OrderBy(o => o.receiveDate).ToList();

            if (objDaily.Count() != 0)
            {
                sTime = objDaily.First().receiveDate;
                //eTime = objDaily.Last().receiveDate;

                foreach (var dataItem in objDaily)
                {
                    foreach (var item in MainChart.Series["總集中度"].Points)
                    {
                        if (item.XValue == dataItem.receiveDate.ToOADate())
                        {
                            item.IsEmpty = false;
                            item.YValues = new double[] { (double)dataItem.rate };
                            item.ToolTip = string.Format("{0}\n總集中度:{1:f2}", dataItem.receiveDate.ToString("yyyy/MM/dd"), (double)dataItem.rate);
                            break;
                        }
                    }
                }
            }
        }

    }
}