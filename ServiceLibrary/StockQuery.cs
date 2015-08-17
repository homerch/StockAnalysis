using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Threading;
using HtmlAgilityPack;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace ServiceLibrary
{
    // Define a class to hold custom event info
    public class CustomEventArgs : EventArgs
    {
        public CustomEventArgs(string s)
        {
            message = s;
        }
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }

    public class StockQuery
    {
        public event EventHandler<CustomEventArgs> RaiseCustomEvent;

        protected virtual void OnRaiseCustomEvent(CustomEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<CustomEventArgs> handler = RaiseCustomEvent;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                // Format the string to send inside the CustomEventArgs parameter
                e.Message += String.Format(" at {0}", DateTime.Now.ToString());

                // Use the () operator to raise the event.
                handler(this, e);
            }
        }

        public void QueryCompany()
        {
            using (stockdbaEntities db = new stockdbaEntities())
            {
                HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                HtmlDocument docContext = new HtmlAgilityPack.HtmlDocument();

                WebClient client = new WebClient();
                MemoryStream ms = new MemoryStream(client.DownloadData("http://isin.twse.com.tw/isin/C_public.jsp?strMode=2"));

                try
                {
                    doc.Load(ms, Encoding.GetEncoding("Big5"));
                    docContext.LoadHtml(doc.DocumentNode.InnerHtml);

                    HtmlNodeCollection nodeHeaders = docContext.DocumentNode.SelectNodes("/body[1]/table[2]/tr");

                    String[] stock;
                    String stockId, name, stockType = "";
                    foreach (var trItem in nodeHeaders)
                    {
                        HtmlNodeCollection tdItems = trItem.SelectNodes("./td");

                        if (tdItems != null)
                        {
                            if (tdItems.Count == 1)
                            {
                                stockType = tdItems[0].InnerText.Trim();
                            }
                            else if (tdItems.Count == 7)
                            {
                                if (tdItems[0].InnerText.Trim() != "有價證券代號及名稱")
                                {
                                    stock = tdItems[0].InnerText.Trim().Split(' ');
                                    stockId = stock.First().Trim();
                                    name = stock.Last().Trim();
                                    if (db.Company.Where(o => o.stockId == stockId).Count() == 0)
                                    {
                                        db.Company.Add(new Company()
                                        {
                                            stockId = stockId,
                                            name = name,
                                            initTime = DateTime.Now,
                                            bzClass = tdItems[4].InnerText.Trim(),
                                            stockType = stockType
                                        });
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryCompany:{0}", ex.Message) });
                    db.SaveChanges();
                }

                db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryCompany:Done") });
                db.SaveChanges();
            }
        }

        public void QueryOTC()
        {
            StockAnalyser analyser = new StockAnalyser();

            WebClient client = new WebClient();
            MemoryStream ms = new MemoryStream(client.DownloadData("http://www.otc.org.tw/ch/stock/aftertrading/DAILY_CLOSE_quotes/stk_quote_download.php"));

            try
            {
                String line = "";
                StreamReader reader = new StreamReader(ms, Encoding.GetEncoding("Big5"));

                //attachment;filename=RSTA3104_1030212.csv
                string disposition = client.ResponseHeaders["content-disposition"];

                String strDate = disposition.Split('_')[1].Substring(0, 7);

                DateTime receiveDate = DateTime.Parse(string.Format("{0}/{1}/{2}", int.Parse(strDate.Substring(0, 3)) + 1911, strDate.Substring(3, 2), strDate.Substring(5, 2)));

                // 上櫃股票行情
                reader.ReadLine();

                // 資料日期:
                reader.ReadLine();

                //    0,           1,     2 ,      3,       4,      5,      6,     7,       8,              9,      10
                // 代號,        名稱,  收盤 ,    漲跌,  開盤 ,  最高 ,   最低,  均價 ,成交股數  ,成交金額(元),成交筆數
                reader.ReadLine();

                // "006201","寶富櫃","11.75","-0.21 ","11.95","11.95","11.70","11.80","48,000","566,480","25",
                while ((line = reader.ReadLine()) != null)
                {
                    String stockId, stockName;
                    String[] dummy = line.Remove(line.Length - 1, 1).Remove(0, 1).Replace("\",\"", "@").Split('@');

                    if (dummy.Length == 17)
                    {
                        stockId = dummy[0];
                        stockName = dummy[1];
                        using (stockdbaEntities db = new stockdbaEntities())
                        {
                            if (db.Company.Where(o => o.stockId == stockId).Count() == 0)
                            {
                                db.Company.Add(new Company()
                                {
                                    stockId = stockId,
                                    name = stockName,
                                    initTime = DateTime.Now,
                                    bzClass = "",
                                    stockType = "上櫃"
                                });
                                db.SaveChanges();
                            }
                        }

                        // Basic Data
                        if (dummy[2].Contains('-') == false)
                        {
                            using (stockdbaEntities db = new stockdbaEntities())
                            {
                                if (db.DailySummary.Where(o => o.receiveDate == receiveDate && o.stockId == stockId).Count() == 0)
                                {
                                    db.DailySummary.Add(new DailySummary()
                                    {
                                        receiveDate = receiveDate,
                                        stockId = stockId,
                                        tradeRec = decimal.Parse(dummy[10]),
                                        tradeAmt = decimal.Parse(dummy[9]),
                                        tradeQty = decimal.Parse(dummy[8]),
                                        openPrice = decimal.Parse(dummy[4]),
                                        highPrice = decimal.Parse(dummy[5]),
                                        lowPrice = decimal.Parse(dummy[6]),
                                        lastPrice = decimal.Parse(dummy[2]),
                                    });
                                    db.SaveChanges();
                                }

                                if (db.DailyDetail.Where(o => o.receiveDate == receiveDate && o.stockId == stockId).Count() != 0)
                                {
                                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryOTC:pass {0}", stockId) });
                                    db.SaveChanges();
                                    continue;
                                }
                            }

                            // Detail
                            StreamReader detailReader = new StreamReader(
                                new MemoryStream(
                                    client.DownloadData(
                                    String.Format("http://www.gretai.org.tw/ch/stock/aftertrading/broker_trading/download_ALLCSV.php?curstk={0}&stk_date={1}", stockId, strDate)
                                    )), Encoding.GetEncoding("Big5"));

                            //券商買賣證券成交價量資訊
                            detailReader.ReadLine();

                            //證券代碼,4946
                            detailReader.ReadLine();

                            //序號,券商,價格,買進股數,賣出股數,,序號,券商,價格,買進股數,賣出股數
                            detailReader.ReadLine();
                            //  0,           1,       2,      3,  4,5, 6,           7,       8,  9,    10
                            //"1","1020  合庫","111.00","2,000","0",,"2","1020  合庫","112.00","0","8,000"
                            while ((line = detailReader.ReadLine()) != null)
                            {
                                using (stockdbaEntities db = new stockdbaEntities())
                                {
                                    foreach (var item in line.Replace(",,", "@").Split('@'))
                                    {
                                        String[] detailDummy = item.Remove(item.Length - 1, 1).Remove(0, 1).Replace("\",\"", "@").Split('@');
                                        Decimal itemNo = decimal.Parse(detailDummy[0]);

                                        if (db.DailyDetail.Where(o => o.receiveDate == receiveDate && o.stockId == stockId && o.no == itemNo).Count() == 0)
                                        {
                                            db.DailyDetail.Add(new DailyDetail()
                                            {
                                                receiveDate = receiveDate,
                                                stockId = stockId,
                                                no = itemNo,
                                                brokerId = detailDummy[1].Split(' ')[0],
                                                value = decimal.Parse(detailDummy[2]),
                                                buyVolume = decimal.Parse(detailDummy[3]),
                                                sellVolume = decimal.Parse(detailDummy[4])
                                            });
                                        }
                                    }
                                    db.SaveChanges();
                                }
                            }

                            using (stockdbaEntities db = new stockdbaEntities())
                            {
                                db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryOTC:{0}", stockId) });
                                db.SaveChanges();
                            }

                            Thread.Sleep(1000);

                            // 結算
                            analyser.DoSettlement(receiveDate, stockId);
                            // 集中率
                            analyser.DoDailyRate(receiveDate, stockId, stockName);
                            analyser.DoWeeklyRate(receiveDate, stockId, stockName);
                            analyser.DoTotalRate(receiveDate, stockId, stockName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (stockdbaEntities db = new stockdbaEntities())
                {
                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryOTC:{0}", ex.Message) });
                    db.SaveChanges();
                }
            }

            using (stockdbaEntities db = new stockdbaEntities())
            {
                db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryOTC:Done") });
                db.SaveChanges();
            }
        }

        public void QueryStockWarrant()
        {
            String line = "", queryPath = "";
            WebClient client = new WebClient();
            MemoryStream ms = new MemoryStream(client.DownloadData("http://www.twse.com.tw/ch/stock_search/warrant_stock.php"));

            //""
            //"  <script language=\"javascript\">"
            //"    window.location.replace(\"genpage/genpage20140403.php?chk_date=103/04/03\");"
            //"  </script>"
            //" "
            StreamReader preReader = new StreamReader(ms, Encoding.GetEncoding("Big5"));
            while ((line = preReader.ReadLine()) != null)
            {
                if (line.Contains("genpage") == true)
                {
                    queryPath = line.Split('?')[0].Split('"')[1];
                }
            }

            try
            {
                // http://www.twse.com.tw/ch/stock_search/warrant_stock_print.php?genpage=genpage/genpage20140403.php&type=csv
                StreamReader reader = new StreamReader(
                                new MemoryStream(
                                    client.DownloadData(
                                    String.Format("http://www.twse.com.tw/ch/stock_search/warrant_stock_print.php?genpage={0}&type=csv", queryPath)
                                    )), Encoding.GetEncoding("Big5"));

                //attachment;filename=genpage20140403.csv
                string disposition = client.ResponseHeaders["content-disposition"];

                String strDate = disposition.Split('=')[1].Substring(7, 8);

                DateTime receiveDate = DateTime.Parse(string.Format("{0}/{1}/{2}", int.Parse(strDate.Substring(0, 4)), strDate.Substring(4, 2), strDate.Substring(6, 2)));

                //
                // ,上市認購(售)權證每日收盤行情資訊彙總表103年04月03日
                reader.ReadLine();
                reader.ReadLine();

                // 權證收盤資訊,標的證券/指數 收盤資訊,權證基本資訊
                reader.ReadLine();

                //    0,           1,     2 ,      3,       4,      5,      6,     7,       8,              9,      10
                // 權證代號,權證簡稱,收盤價,漲跌,標的代號,標的證券/指數,收盤價/指數,漲跌,權證類型,履約方式,履約開始日,最後交易日,履約截止日,行使比例,履約價格(元)/指數,上限價格(元)/指數,下限價格(元)/指數
                reader.ReadLine();

                // 03123B,元熊AL,1.58,＋,="0050",台灣50,60.65,－,認售,歐式, 103/09/11, 103/09/09, 103/09/11,0.307,63.95,61.80,
                while ((line = reader.ReadLine()) != null)
                {
                    String stockId;
                    String[] dummy = line.Split(',');

                    if (dummy.Length == 17)
                    {
                        stockId = dummy[0];
                        //="068048"
                        if (stockId.Contains("=") == true)
                        {
                            stockId = stockId.Replace("=", "").Replace("\"", "");
                        }

                        using (stockdbaEntities db = new stockdbaEntities())
                        {
                            if (db.Company.Where(o => o.stockId == stockId).Count() == 0)
                            {
                                db.Company.Add(new Company()
                                {
                                    stockId = stockId,
                                    name = dummy[1],
                                    initTime = DateTime.Now,
                                    bzClass = "",
                                    stockType = "上市認購(售)權證"
                                });
                                db.SaveChanges();
                            }
                        }

                        try
                        {
                            if (dummy[2] != "0")
                            {
                                using (stockdbaEntities db = new stockdbaEntities())
                                {
                                    if (db.DailyWork.Where(o => o.receiveDate == receiveDate && o.stockId == stockId).Count() == 0)
                                    {
                                        // Basic Data
                                        QueryBasicData(stockId);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (stockdbaEntities db = new stockdbaEntities())
                {
                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QuerySW:{0}", ex.Message) });
                    db.SaveChanges();
                }
            }

            using (stockdbaEntities db = new stockdbaEntities())
            {
                db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QuerySW:Done") });
                db.SaveChanges();
            }
        }

        public void QueryOTCWarrant(DateTime receiveDate)
        {
            StockAnalyser analyser = new StockAnalyser();

            WebClient client = new WebClient();
            MemoryStream ms = new MemoryStream(client.DownloadData("http://www.otc.org.tw/ch/extend/warrant/dailyQ/wntQuts_download.php?t=D"));

            try
            {
                String line = "";
                StreamReader reader = new StreamReader(ms, Encoding.GetEncoding("Big5"));

                //attachment;filename=NEWWT_1030403.csv
                string disposition = client.ResponseHeaders["content-disposition"];

                String strDate = disposition.Split('_')[1].Substring(0, 7);

                receiveDate = DateTime.Parse(string.Format("{0}/{1}/{2}", int.Parse(strDate.Substring(0, 3)) + 1911, strDate.Substring(3, 2), strDate.Substring(5, 2)));

                // 103年04月03日 櫃檯買賣認購售權證收盤行情
                reader.ReadLine();

                //    0,           1,     2 ,      3,       4,      5,      6,     7,       8,              9,      10
                // 代號,	名稱,	開市價,	最高價,	最低價,	收市價,	漲跌,	成交量(股,)	筆數,	成交金額,	標的代號,	證券標的/指數收盤,	標的/指數漲跌
                reader.ReadLine();

                //"707825","國泰P6","3.76","3.76","3.76","3.76","0.06","1000","1","3760","4162","智擎 ","244","-2.5"
                while ((line = reader.ReadLine()) != null)
                {
                    String stockId, stockName;
                    //String[] dummy = line.Remove(line.Length - 1, 1).Replace("\",\"", "@").Split('@');
                    String[] dummy = line.Replace("\"", "").Split(',');

                    if (dummy.Length == 14)
                    {
                        stockId = dummy[0];
                        stockName = dummy[1];
                        using (stockdbaEntities db = new stockdbaEntities())
                        {
                            if (db.Company.Where(o => o.stockId == stockId).Count() == 0)
                            {
                                db.Company.Add(new Company()
                                {
                                    stockId = stockId,
                                    name = stockName,
                                    initTime = DateTime.Now,
                                    bzClass = "",
                                    stockType = "上櫃權證"
                                });
                                db.SaveChanges();
                            }
                        }

                        // Basic Data
                        if (dummy[2].Contains('-') == false)
                        {
                            using (stockdbaEntities db = new stockdbaEntities())
                            {
                                if (db.DailySummary.Where(o => o.receiveDate == receiveDate && o.stockId == stockId).Count() == 0)
                                {
                                    db.DailySummary.Add(new DailySummary()
                                    {
                                        receiveDate = receiveDate,
                                        stockId = stockId,
                                        tradeRec = decimal.Parse(dummy[8]),
                                        tradeAmt = decimal.Parse(dummy[9]),
                                        tradeQty = decimal.Parse(dummy[7]),
                                        openPrice = decimal.Parse(dummy[2]),
                                        highPrice = decimal.Parse(dummy[3]),
                                        lowPrice = decimal.Parse(dummy[4]),
                                        lastPrice = decimal.Parse(dummy[5]),
                                    });
                                    db.SaveChanges();
                                }

                                if (db.DailyDetail.Where(o => o.receiveDate == receiveDate && o.stockId == stockId).Count() != 0)
                                {
                                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryOW:pass {0}", stockId) });
                                    db.SaveChanges();
                                    continue;
                                }
                            }

                            // Detail
                            StreamReader detailReader = new StreamReader(
                                new MemoryStream(
                                    client.DownloadData(
                                    String.Format("http://www.gretai.org.tw/ch/stock/aftertrading/broker_trading/download_ALLCSV.php?curstk={0}&stk_date={1}", stockId, strDate)
                                    )), Encoding.GetEncoding("Big5"));

                            //券商買賣證券成交價量資訊
                            detailReader.ReadLine();

                            //證券代碼,4946
                            detailReader.ReadLine();

                            //序號,券商,價格,買進股數,賣出股數,,序號,券商,價格,買進股數,賣出股數
                            detailReader.ReadLine();
                            //  0,           1,       2,      3,  4,5, 6,           7,       8,  9,    10
                            //"1","1020  合庫","111.00","2,000","0",,"2","1020  合庫","112.00","0","8,000"
                            while ((line = detailReader.ReadLine()) != null)
                            {
                                using (stockdbaEntities db = new stockdbaEntities())
                                {
                                    foreach (var item in line.Replace(",,", "@").Split('@'))
                                    {
                                        String[] detailDummy = item.Remove(item.Length - 1, 1).Remove(0, 1).Replace("\",\"", "@").Split('@');
                                        Decimal itemNo = decimal.Parse(detailDummy[0]);

                                        if (db.DailyDetail.Where(o => o.receiveDate == receiveDate && o.stockId == stockId && o.no == itemNo).Count() == 0)
                                        {
                                            db.DailyDetail.Add(new DailyDetail()
                                            {
                                                receiveDate = receiveDate,
                                                stockId = stockId,
                                                no = itemNo,
                                                brokerId = detailDummy[1].Split(' ')[0],
                                                value = decimal.Parse(detailDummy[2]),
                                                buyVolume = decimal.Parse(detailDummy[3]),
                                                sellVolume = decimal.Parse(detailDummy[4])
                                            });
                                        }
                                    }
                                    db.SaveChanges();
                                }
                            }

                            using (stockdbaEntities db = new stockdbaEntities())
                            {
                                db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryOW:{0}", stockId) });
                                db.SaveChanges();
                            }

                            Thread.Sleep(1000);

                            // 結算
                            analyser.DoSettlement(receiveDate, stockId);
                            // 集中率
                            analyser.DoDailyRate(receiveDate, stockId, stockName);
                            analyser.DoWeeklyRate(receiveDate, stockId, stockName);
                            analyser.DoTotalRate(receiveDate, stockId, stockName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (stockdbaEntities db = new stockdbaEntities())
                {
                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryOW:{0}", ex.Message) });
                    db.SaveChanges();
                }
            }

            using (stockdbaEntities db = new stockdbaEntities())
            {
                db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryOW:Done") });
                db.SaveChanges();
            }
        }

        public void QueryBroker()
        {
            bool need2Update = false;

            HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            HtmlDocument docContext = new HtmlAgilityPack.HtmlDocument();

            WebClient client = new WebClient();
            MemoryStream ms = new MemoryStream(client.DownloadData("http://www.twse.com.tw/ch/products/broker_service/broker_list.php"));

            using (stockdbaEntities db = new stockdbaEntities())
            {
                try
                {
                    doc.Load(ms, Encoding.GetEncoding("Big5"));
                    docContext.LoadHtml(doc.DocumentNode.InnerHtml);

                    HtmlNodeCollection nodeHeaders = docContext.DocumentNode.SelectNodes("//*[@id='contentblock']/td/table[2]/tr");

                    foreach (var trItem in nodeHeaders)
                    {
                        HtmlNodeCollection tdItems = trItem.SelectNodes("./td");

                        if (tdItems != null)
                        {
                            if (tdItems.Count == 6)
                            {
                                if (tdItems[0].InnerText.Trim() != "證券商代號")
                                {
                                    string id = tdItems[0].InnerText.Trim();
                                    if (db.Broker.Where(o => o.id == id).Count() == 0)
                                    {
                                        db.Broker.Add(new Broker()
                                        {
                                            id = id,
                                            name = tdItems[1].InnerText.Trim(),
                                            branch = "總公司"
                                        });
                                        need2Update = true;
                                    }
                                }
                            }
                        }
                    }

                    if (need2Update == true)
                    {
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryBroker:{0}", ex.Message) });
                    db.SaveChanges();
                }

                db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryBroker:Done") });
                db.SaveChanges();
            }
        }

        public void QueryBrokerBranch()
        {
            bool need2Update = false;

            HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            HtmlDocument docContext = new HtmlAgilityPack.HtmlDocument();

            WebClient client = new WebClient();
            MemoryStream ms = new MemoryStream(client.DownloadData("http://www.twse.com.tw/ch/products/broker_service/broker2_list.php"));

            using (stockdbaEntities db = new stockdbaEntities())
            {
                try
                {
                    doc.Load(ms, Encoding.GetEncoding("Big5"));
                    docContext.LoadHtml(doc.DocumentNode.InnerHtml);

                    HtmlNodeCollection nodeHeaders = docContext.DocumentNode.SelectNodes("//*[@id='contentblock']/td/table[2]/tr");

                    foreach (var trItem in nodeHeaders)
                    {
                        HtmlNodeCollection tdItems = trItem.SelectNodes("./td");

                        if (tdItems != null)
                        {
                            if (tdItems.Count == 5)
                            {
                                if (tdItems[0].InnerText.Trim() != "證券商代號")
                                {
                                    string id = tdItems[0].InnerText.Trim();
                                    string[] name = tdItems[1].InnerText.Trim().Split('-');

                                    if (db.Broker.Where(o => o.id == id).Count() == 0)
                                    {
                                        db.Broker.Add(new Broker()
                                        {
                                            id = id,
                                            name = name.First().Trim(),
                                            branch = name.Last().Trim()
                                        });

                                        need2Update = true;
                                    }
                                    else
                                    {
                                        Console.WriteLine(id);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine(tdItems[0]);
                            }
                        }
                    }

                    if (need2Update == true)
                    {
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryBrokerBranch:{0}", ex.Message) });
                    db.SaveChanges();
                }

                db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryBrokerBranch:Done") });
                db.SaveChanges();
            }
        }

        public void QueryDaliyWork()
        {
            using (stockdbaEntities db = new stockdbaEntities())
            {
                List<Company> stockList = db.Company.Where(o => ((o.stockType == "股票") || (o.stockType == "ETF"))).ToList();

                foreach (var item in stockList)
                {
                    try
                    {
                        if (item.updateTime != null)
                        {
                            continue;
                        }

                        QueryBasicData(item.stockId);

                        item.updateTime = DateTime.Now;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryDaliyWork:{0}", ex.Message) });
                        db.SaveChanges();
                    }

                    Thread.Sleep(1000);
                }

                db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryDaliyWork:Done") });
                db.SaveChanges();
            }
        }

        //public void QueryPage()
        //{
        //    using (stockdbaEntities db = new stockdbaEntities())
        //    {

        //        DateTime rDate;
        //        DailyWork dailyWork = db.DailyWork.OrderByDescending(o => o.receiveDate).First();

        //        try
        //        {
        //            if (dailyWork != null)
        //            {
        //                rDate = dailyWork.receiveDate;
        //                while (db.DailyWork.Where(o => o.currentPage != o.totalPage && o.receiveDate == rDate).Count() != 0)
        //                {
        //                    dailyWork = db.DailyWork.Where(o => o.currentPage != o.totalPage && o.receiveDate == rDate).First();
        //                    int current = (int)dailyWork.currentPage;
        //                    int total = (int)dailyWork.totalPage;
        //                    try
        //                    {
        //                        for (int i = current + 1; i <= total; i++)
        //                        {
        //                            db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryPage:{0}/({1}:{2})", dailyWork.stockId, i, dailyWork.totalPage) });
        //                            db.SaveChanges();

        //                            QuerySinglePage(i, dailyWork.stockId);

        //                            dailyWork.currentPage = i;
        //                            db.SaveChanges();

        //                            Thread.Sleep(10000);
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryPage:{0}", ex.Message) });
        //                        db.SaveChanges();
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryPage:{0}", ex.Message) });
        //            db.SaveChanges();

        //            throw;
        //        }
        //    }
        //}

        void QueryBasicData(string stockId)
        {
            string Url = "http://bsr.twse.com.tw/bshtm/bsMenu.aspx";
            //string formData = "__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE=%2FwEPDwUKLTQzNzI3ODE3MQ9kFgICAQ9kFg4CBQ8WAh4JaW5uZXJodG1sBQoy" +
            //                "MDEzLzA5LzE0ZAIGDxYCHwAFCDIwMTMwOTE0ZAIIDw8WBh4JRm9udF9Cb2xkZx4EXyFTQgKEEB4JRm9yZUNvbG9yCj1kZAIKD2QWAgIBDw9" +
            //                "kFgIeB09uQ2xpY2sFHGphdmFzY3JpcHQ6YnV0Q2xlYXJfQ2xpY2soKTtkAgwPDxYGHwFoHwIChBAfAwpHZGQCDg8PFgIeB1Zpc2libGVoZGQ" +
            //                "CEA8PFgYfAWgfAgKEEB8DCkdkZG" +
            //                "Si2zjocmrGJllnPH1VNPbh&__EVENTVALIDATION=%2FwEdAAkJ3iIwU1Mi8o7slM8DAmCjib%2BVrHjO6GeEEDcmd50Vv%2FHinYr2havdthGBI4bn%2FuVafk25GKI%2BxFnm8toEIF08OdRfizmino3LPjd4bEI%2Fa8n%2BshoO65Mgov1Gk1LiWrVFJyI109haYg0KuKnXCrDTo3DdQysje%2Ft8sfeZbMNL%2FQdIuRSdO0Evx9MQYYVhnqHnlqy1Mf5yy3u9S%2Fzb2lLx" +
            //                "&HiddenField_spDate=&" +
            //                "HiddenField_page=PAGE_BS&txtTASKNO={0}&hidTASKNO=&btnOK=%E6%9F%A5%E8%A9%A2";

            //string formData = "__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE=%2FwEPDwUKLTQzNzI3ODE3MQ9kFgICAQ9kFg4CBQ8WAh4JaW5uZXJodG1sBQoy" +
            //                  "MDE0LzA5LzA1ZAIGDxYCHwAFCDIwMTQwOTA1ZAIIDw8WBh4JRm9udF9Cb2xkZx4EXyFTQgKEEB4JRm9yZUNvbG9yCj1kZAIKD2QWAgIBDw" +
            //                  "9kFgIeB09uQ2xpY2sFHGphdmFzY3JpcHQ6YnV0Q2xlYXJfQ2xpY2soKTtkAgwPDxYGHwFoHwIChBAfAwpHZGQCDg8PFgIeB1Zpc2libGVo" +
            //                  "ZGQCEA8PFgYfAWgfAgKEEB8DCkdkZGSi2zjocmrGJllnPH1VNPbh&__EVENTVALIDATION=%2FwEdAAkJ3iIwU1Mi8o7slM8DAmCjib%2B" +
            //                  "VrHjO6GeEEDcmd50Vv%2FHinYr2havdthGBI4bn%2FuVafk25GKI%2BxFnm8toEIF08OdRfizmino3LPjd4bEI%2Fa8n%2BshoO65Mgov1" +
            //                  "Gk1LiWrVFJyI109haYg0KuKnXCrDTo3DdQysje%2Ft8sfeZbMNL%2FQdIuRSdO0Evx9MQYYVhnqHnlqy1Mf5yy3u9S%2Fzb2lLx&" +
            //                  "HiddenField_spDate=&HiddenField_page=PAGE_BS&txtTASKNO={0}&hidTASKNO=&btnOK=%E6%9F%A5%E8%A9%A2";

            string formData = "__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE=%2FwEPDwUKLTQzNzI3ODE3MQ9kFgICAQ9kFg4CBQ8WAh4JaW5uZXJodG1sBQoyMDE0LzA5LzExZAIGDxYCHwAFCDIwMTQwOTExZAIIDw8WBh4JRm9udF9Cb2xkZx4EXyFTQgKEEB4JRm9yZUNvbG9yCj1kZAIKD2QWAgIBDw9kFgIeB09uQ2xpY2sFHGphdmFzY3JpcHQ6YnV0Q2xlYXJfQ2xpY2soKTtkAgwPDxYGHwFoHwIChBAfAwpHZGQCDg8PFgIeB1Zpc2libGVoZGQCEA8PFgYfAWgfAgKEEB8DCkdkZGSkFDT4%2BBwEV%2BFPI%2B%2FF2Py0AAAAAA%3D%3D&HiddenField_spDate=&HiddenField_page=PAGE_BS&txtTASKNO={0}&hidTASKNO=&btnOK=%E6%9F%A5%E8%A9%A2&__EVENTVALIDATION=%2FwEWCQKjjbiOBALjpuXcAwKN4Ij0CwLB5ZfoCQLjk6TKBwKY8en5CwLdkpmPAQL6n7vzCwLAhrvLBXcajwPloy7gIyMjVj%2FNcicAAAAA";

            using (stockdbaEntities db = new stockdbaEntities())
            {
                try
                {
                    HttpWebRequest request = HttpWebRequest.Create(Url) as HttpWebRequest;
                    request.Method = "POST";
                    request.KeepAlive = false;
                    request.Timeout = 300000;
                    request.ContentType = "application/x-www-form-urlencoded";

                    byte[] bs = Encoding.ASCII.GetBytes(String.Format(formData, stockId));
                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(bs, 0, bs.Length);
                    }

                    using (WebResponse response = request.GetResponse())
                    {
                        int pages;
                        DateTime rDate;

                        HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                        HtmlDocument docContext = new HtmlAgilityPack.HtmlDocument();
                        doc.Load(response.GetResponseStream());
                        HtmlNode node = doc.DocumentNode.SelectSingleNode("//*[@id='sp_ListCount']");
                        HtmlNode nodeDate = doc.DocumentNode.SelectSingleNode("//*[@id='HiddenField_spDate']");

                        if (node != null)
                        {
                            if (node.InnerHtml != "")
                            {
                                pages = int.Parse(node.InnerHtml);
                                rDate = DateTime.Parse(nodeDate.GetAttributeValue("value", "20000101").Insert(6, "/").Insert(4, "/"));

                                if (db.DailyWork.Where(o => o.receiveDate == rDate && o.stockId == stockId).Count() == 0)
                                {
                                    db.DailyWork.Add(new DailyWork()
                                    {
                                        receiveDate = rDate,
                                        stockId = stockId,
                                        currentPage = 0,
                                        totalPage = pages,
                                        updateTime = DateTime.Now
                                    });
                                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryBasicData:{0}/{1}", stockId, pages) });
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                throw new Exception(String.Format("QueryBasicData:查無資料:{0}", stockId));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QueryBasicData:{0}", ex.Message) });
                    db.SaveChanges();

                    throw;
                }
            }
        }

        public void QuerySinglePage(int page, string stockId)
        {
            Dictionary<int, StockItem> stockData = new Dictionary<int, StockItem>();
            string[] xPaths = { "//*[@id='table2']/tr/td[1]/table", "//*[@id='table2']/tr/td[2]/table" };
            DateTime receiveDate;
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            HtmlAgilityPack.HtmlDocument docContext = new HtmlAgilityPack.HtmlDocument();


            try
            {
                HttpWebRequest request = HttpWebRequest.Create(String.Format("http://bsr.twse.com.tw/bshtm/bsContent.aspx?StartNumber={0}&FocusIndex={1}", stockId, page)) as HttpWebRequest;
                request.Method = "GET";
                request.Timeout = 300000;

                using (WebResponse response = request.GetResponse())
                {
                    doc.Load(response.GetResponseStream(), Encoding.UTF8);
                    receiveDate = DateTime.Parse(doc.DocumentNode.SelectSingleNode("//*[@id='receive_date']").InnerHtml.Trim());
                    if (page == 1)
                    {
                        using (stockdbaEntities db = new stockdbaEntities())
                        {
                            // Basic Data
                            if (db.DailySummary.Where(o => o.receiveDate == receiveDate && o.stockId == stockId).Count() == 0)
                            {
                                db.DailySummary.Add(new DailySummary()
                                {
                                    receiveDate = receiveDate,
                                    stockId = doc.DocumentNode.SelectSingleNode("//*[@id='stock_id']").InnerHtml.Replace("&nbsp;", " ").Trim().Split(' ')[0],
                                    tradeRec = decimal.Parse(doc.DocumentNode.SelectSingleNode("//*[@id='trade_rec']").InnerHtml.Replace(",", "").Trim()),
                                    tradeAmt = decimal.Parse(doc.DocumentNode.SelectSingleNode("//*[@id='trade_amt']").InnerHtml.Replace(",", "").Trim()),
                                    tradeQty = decimal.Parse(doc.DocumentNode.SelectSingleNode("//*[@id='trade_qty']").InnerHtml.Replace(",", "").Trim()),
                                    openPrice = decimal.Parse(doc.DocumentNode.SelectSingleNode("//*[@id='open_price']").InnerHtml.Trim()),
                                    highPrice = decimal.Parse(doc.DocumentNode.SelectSingleNode("//*[@id='high_price']").InnerHtml.Trim()),
                                    lowPrice = decimal.Parse(doc.DocumentNode.SelectSingleNode("//*[@id='low_price']").InnerHtml.Trim()),
                                    lastPrice = decimal.Parse(doc.DocumentNode.SelectSingleNode("//*[@id='last_price']").InnerHtml.Trim())
                                });
                                db.SaveChanges();
                            }
                        }
                    }

                    foreach (var path in xPaths)
                    {
                        docContext.LoadHtml(doc.DocumentNode.SelectSingleNode(path).InnerHtml);
                        HtmlNodeCollection nodeHeaders = docContext.DocumentNode.SelectNodes("./tr");
                        foreach (HtmlNode trItem in nodeHeaders)
                        {
                            if (trItem.Attributes["class"].Value.Contains("column_value_price"))
                            {
                                int serialNo;
                                StockItem item = new StockItem();
                                HtmlNodeCollection tdItems = trItem.SelectNodes("./td");

                                if (int.TryParse(tdItems[0].InnerText.Trim(), out serialNo) == true)
                                {
                                    item.No = serialNo;
                                    item.BrokerId = tdItems[1].InnerText.Trim().Split(' ')[0];
                                    item.Value = float.Parse(tdItems[2].InnerText.Trim());
                                    item.InVolume = int.Parse(tdItems[3].InnerText.Replace(",", "").Trim());
                                    item.OutVolume = int.Parse(tdItems[4].InnerText.Replace(",", "").Trim());
                                    stockData.Add(item.No, item);
                                }
                            }
                        }
                    }
                }

                // Add to DB
                if (stockData.Count != 0)
                {
                    using (stockdbaEntities db = new stockdbaEntities())
                    {
                        foreach (var item in stockData.Values)
                        {
                            if (db.DailyDetail.Where(o => o.receiveDate == receiveDate && o.stockId == stockId && o.no == item.No).Count() == 0)
                            {
                                db.DailyDetail.Add(new DailyDetail()
                                {
                                    receiveDate = receiveDate,
                                    stockId = stockId,
                                    no = item.No,
                                    brokerId = item.BrokerId,
                                    value = (decimal)item.Value,
                                    buyVolume = item.InVolume,
                                    sellVolume = item.OutVolume
                                });
                                //db.SaveChanges();
                            }
                        }
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                using (stockdbaEntities db = new stockdbaEntities())
                {
                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("QuerySinglePage:{0}", ex.Message) });
                    db.SaveChanges();
                }
                throw;
            }
        }

        // http://www.gretai.org.tw/ch/stock/aftertrading/broker_trading/download_ALLCSV.php?curstk=4946&stk_date=1030127
        // http://www.otc.org.tw/ch/stock/aftertrading/DAILY_CLOSE_quotes/stk_quote_download.php?d=103/01/27&s=0,asc,0
    }

    class StockItem
    {
        public int No;
        public string BrokerId;
        public float Value;
        public int InVolume;
        public int OutVolume;
    }
}
