using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace ServiceLibrary
{
    public class StockUtility
    {
        public void Export2CSV(string path, DateTime receiveDate)
        {
            using (stockdbaEntities db = new stockdbaEntities())
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(path))
                    {
                        writer.WriteLine("receiveDate,stockId,no,brokerId,value,buyVolume,sellVolume");
                        foreach (var item in from a in db.DailyDetail.Where(o => o.receiveDate == receiveDate)
                                             orderby a.stockId, a.no
                                             select a)
                        {
                            writer.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6}",
                                item.receiveDate.ToString("yyyy/MM/dd"), item.stockId, item.no, item.brokerId, item.value, item.buyVolume, item.sellVolume));
                        }
                    }

                    string startPath = @"D:\Hosting\11804480\html\DailyData";
                    string zipPath = @"D:\Hosting\11804480\html\DailyDataBackups\DailyData_{0}.zip";

                    ZipFile.CreateFromDirectory(startPath, string.Format(zipPath, receiveDate.ToString("yyyyMMdd")));

                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("Export2CSV:Done") });
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("Export2CSV:{0}", ex.Message) });
                    db.SaveChanges();
                }
            }
        }

        public void ImportDailySettelment(string path, DateTime receiveDate)
        {
            //using (stockdbaEntities db = new stockdbaEntities())
            //{
            //    using (stockdbaLiteEntities dbLite = new stockdbaLiteEntities())
            //    {
            //        try
            //        {
            //            using (StreamReader reader = new StreamReader(path))
            //            {
            //                string line;
            //                while ((line = reader.ReadLine()) != null)
            //                {
            //                    string[] datas = line.Split(',');

            //                    receiveDate = DateTime.Parse(datas[0]);
            //                    string stockId = datas[1];
            //                    string brokerName = datas[2];
            //                    string stockName = datas[3];
            //                    decimal buyVolume = decimal.Parse(datas[4]);
            //                    decimal sellVolume = decimal.Parse(datas[5]);
            //                    decimal avgValue = decimal.Parse(datas[6]);

            //                    if (dbLite.DailySettlementLite.Where(o => o.receiveDate == receiveDate && o.stockId == stockId && o.brokerName == brokerName).Count() == 0)
            //                    {
            //                        dbLite.DailySettlementLite.Add(new DailySettlementLite()
            //                        {
            //                            receiveDate = receiveDate,
            //                            stockId = stockId,
            //                            brokerName = brokerName,
            //                            stockName = stockName,
            //                            buyVolume = buyVolume,
            //                            sellVolume = sellVolume,
            //                            avgValue = avgValue
            //                        });
            //                    }
            //                }
            //            }
            //            dbLite.SaveChanges();

            //            db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("ImportDailySettelment:Done") });
            //            db.SaveChanges();
            //        }
            //        catch (Exception ex)
            //        {
            //            db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("ImportDailySettelment:{0}", ex.Message) });
            //            db.SaveChanges();
            //        }
            //    }
            //}
        }

        public void ZipData(string path, DateTime receiveDate)
        {
            //string startPath = @"c:\example\start";
            //string zipPath = @"c:\example\result.zip";

            //ZipFile.CreateFromDirectory(startPath, zipPath);
        }

        public void Reset()
        {
            using (stockdbaEntities db = new stockdbaEntities())
            {
                try
                {
                    foreach (var item in db.Company)
                    {
                        item.updateTime = null;
                    }
                    db.SaveChanges();

                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("Reset:done") });
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("Reset:{0}", ex.Message) });
                    db.SaveChanges();
                }
            }
        }
    }
}
