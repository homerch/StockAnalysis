using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    public class StockAnalyser
    {
        public void DoDailyRate(DateTime sTime)
        {
            try
            {
                List<Company> stockList = new List<Company>();

                using (stockdbaEntities db = new stockdbaEntities())
                {
                    stockList = db.Company.Where(o => ((o.stockType == "股票") || (o.stockType == "ETF") || (o.stockType == "上櫃"))).ToList();
                }

                ParallelOptions option = new ParallelOptions();
                option.MaxDegreeOfParallelism = 10;

                //foreach (var item in stockList)
                Parallel.ForEach(stockList, item =>
                {
                    using (stockdbaEntities db = new stockdbaEntities())
                    {
                        if (db.DailyRate.Where(o => o.receiveDate == sTime && o.stockId == item.stockId).Count() != 0)
                        {
                            //continue;
                            return;
                        }

                        var objAllBroker = (from a in db.DailySettlement.Where(o => o.stockId == item.stockId && o.receiveDate == sTime)
                                            group a by new
                                            {
                                                a.brokerName,
                                                a.brokerBranch
                                            } into g
                                            select new
                                            {
                                                brokerName = g.Key.brokerName,
                                                brokerBranch = g.Key.brokerBranch,
                                                netVolume = g.Sum(o => o.buyVolume - o.sellVolume),
                                                totalVolume = g.Sum(o => o.buyVolume)
                                            });//.OrderByDescending(o => o.netVolume);

                        if (objAllBroker.Count() > 0)
                        {
                            double iTotal = (double)objAllBroker.Sum(o => o.totalVolume);

                            //double iRate = (double)(buyTop15.Sum(o => o.netVolume) + sellTop15.Sum(o => o.netVolume)) * 100 / iTotal;
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

                            if (db.DailyRate.Where(o => o.receiveDate == sTime && o.stockId == item.stockId).Count() == 0)
                            {
                                db.DailyRate.Add(new DailyRate() { receiveDate = sTime, stockId = item.stockId, stockName = item.name, rate = (decimal?)iRate, totalVolume = (decimal?)iTotal });
                                db.SaveChanges();
                            }
                        }
                    }
                });

                using (stockdbaEntities db = new stockdbaEntities())
                {
                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("DoDailyRate:Done {0}", sTime) });
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                using (stockdbaEntities db = new stockdbaEntities())
                {
                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("DoDailyRate:{0}", ex.Message) });
                    db.SaveChanges();
                }
            }
        }

        public void DoWeeklyRate(DateTime sTime)
        {
            try
            {
                List<Company> stockList = new List<Company>();

                using (stockdbaEntities db = new stockdbaEntities())
                {
                    stockList = db.Company.Where(o => ((o.stockType == "股票") || (o.stockType == "ETF") || (o.stockType == "上櫃"))).ToList();
                }

                //foreach (var item in stockList)
                Parallel.ForEach(stockList, item =>
                {
                    using (stockdbaEntities db = new stockdbaEntities())
                    {
                        if (db.WeeklyRate.Where(o => o.receiveDate == sTime && o.stockId == item.stockId).Count() != 0)
                        {
                            //continue;
                            return;
                        }

                        DateTime eTime = sTime.AddDays(-10);
                        DateTime sDummy;
                        DateTime eDummy;

                        var calcInterval = (from a in db.DailySettlement.Where(o => o.stockId == item.stockId && o.receiveDate <= sTime && o.receiveDate >= eTime)
                                            group a by new { a.receiveDate } into g
                                            select new { g.Key.receiveDate }).OrderByDescending(o => o.receiveDate).Take(5).ToList();

                        if (calcInterval.Count() == 0)
                        {
                            //continue;
                            return;
                        }

                        sDummy = calcInterval.First().receiveDate;
                        eDummy = calcInterval.Last().receiveDate;

                        var objAllBroker = (from a in db.DailySettlement.Where(o => o.stockId == item.stockId && o.receiveDate <= sDummy && o.receiveDate >= eDummy)
                                            group a by new
                                            {
                                                a.brokerName,
                                                a.brokerBranch
                                            } into g
                                            select new
                                            {
                                                brokerName = g.Key.brokerName,
                                                brokerBranch = g.Key.brokerBranch,
                                                netVolume = g.Sum(o => o.buyVolume - o.sellVolume),
                                                totalVolume = g.Sum(o => o.buyVolume)
                                            });//.OrderByDescending(o => o.netVolume);

                        if (objAllBroker.Count() > 0)
                        {
                            double iTotal = (double)objAllBroker.Sum(o => o.totalVolume);

                            //double iRate = (double)(objAllBroker.Take(15).Sum(o => o.netVolume) + objAllBroker.Skip(objAllBroker.Count() - 15).Sum(o => o.netVolume)) * 100 / iTotal;
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

                            if (db.WeeklyRate.Where(o => o.receiveDate == sTime && o.stockId == item.stockId).Count() == 0)
                            {
                                db.WeeklyRate.Add(new WeeklyRate() { receiveDate = sTime, stockId = item.stockId, stockName = item.name, rate = (decimal?)iRate, totalVolume = (decimal?)iTotal });
                                db.SaveChanges();
                            }
                        }
                    }
                });

                using (stockdbaEntities db = new stockdbaEntities())
                {
                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("DoWeeklyRate:Done {0}", sTime) });
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                using (stockdbaEntities db = new stockdbaEntities())
                {
                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("DoWeeklyRate:{0}", ex.Message) });
                    db.SaveChanges();
                }
            }
        }

        public void DoTotalRate(DateTime sTime)
        {
            try
            {
                List<Company> stockList = new List<Company>();

                using (stockdbaEntities db = new stockdbaEntities())
                {
                    stockList = db.Company.Where(o => ((o.stockType == "股票") || (o.stockType == "ETF") || (o.stockType == "上櫃"))).ToList();
                }

                Parallel.ForEach(stockList, item =>
                {
                    using (stockdbaEntities db = new stockdbaEntities())
                    {
                        try
                        {
                            if (db.TotalRate.Where(o => o.receiveDate == sTime && o.stockId == item.stockId).Count() != 0)
                            {
                                return;
                            }

                            var objAllBroker = (from a in db.DailySettlement.Where(o => o.stockId == item.stockId && o.receiveDate <= sTime)
                                                group a by new
                                                {
                                                    a.brokerName,
                                                    a.brokerBranch
                                                } into g
                                                select new
                                                {
                                                    brokerName = g.Key.brokerName,
                                                    brokerBranch = g.Key.brokerBranch,
                                                    netVolume = g.Sum(o => o.buyVolume - o.sellVolume),
                                                    totalVolume = g.Sum(o => o.buyVolume)
                                                });

                            if (objAllBroker.Count() > 0)
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

                                if (db.TotalRate.Where(o => o.receiveDate == sTime && o.stockId == item.stockId).Count() == 0)
                                {
                                    db.TotalRate.Add(new TotalRate() { receiveDate = sTime, stockId = item.stockId, stockName = item.name, rate = (decimal?)iRate, totalVolume = (decimal?)iTotal });
                                    db.SaveChanges();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("DoTotalRate:{0}", ex.Message) });
                            db.SaveChanges();
                        }
                    }
                });
                using (stockdbaEntities db = new stockdbaEntities())
                {
                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("DoTotalRate:Done {0}", sTime) });
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                using (stockdbaEntities db = new stockdbaEntities())
                {
                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("DoTotalRate:{0}", ex.Message) });
                    db.SaveChanges();
                }
            }
        }

        public void DoDailyRate(DateTime receiveDate, string stockId, string stockName)
        {
            using (stockdbaEntities db = new stockdbaEntities())
            {
                if (db.DailyRate.Where(o => o.receiveDate == receiveDate && o.stockId == stockId).Count() != 0)
                {
                    return;
                }

                var objAllBroker = (from a in db.DailySettlement.Where(o => o.stockId == stockId && o.receiveDate == receiveDate)
                                    group a by new
                                    {
                                        a.brokerName,
                                        a.brokerBranch
                                    } into g
                                    select new
                                    {
                                        brokerName = g.Key.brokerName,
                                        brokerBranch = g.Key.brokerBranch,
                                        netVolume = g.Sum(o => o.buyVolume - o.sellVolume),
                                        totalVolume = g.Sum(o => o.buyVolume)
                                    });

                if (objAllBroker.Count() > 0)
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

                    if (db.DailyRate.Where(o => o.receiveDate == receiveDate && o.stockId == stockId).Count() == 0)
                    {
                        try
                        {
                            db.DailyRate.Add(new DailyRate() { receiveDate = receiveDate, stockId = stockId, stockName = stockName, rate = (decimal?)iRate, totalVolume = (decimal?)iTotal });
                            db.SaveChanges();
                        }
                        catch (Exception)
                        {
                            
                            throw;
                        }

                    }
                }
            }
        }

        public void DoWeeklyRate(DateTime receiveDate, string stockId, string stockName)
        {
            using (stockdbaEntities db = new stockdbaEntities())
            {
                if (db.WeeklyRate.Where(o => o.receiveDate == receiveDate && o.stockId == stockId).Count() != 0)
                {
                    return;
                }

                //DateTime eTime = receiveDate.AddDays(-10);
                DateTime sDummy;
                DateTime eDummy;

                var calcInterval = (from a in db.DailySettlement.Where(o => o.stockId == stockId && o.receiveDate <= receiveDate)
                                    group a by new { a.receiveDate } into g
                                    select new { g.Key.receiveDate }).OrderByDescending(o => o.receiveDate).Take(5).ToList();

                if (calcInterval.Count() == 0)
                {
                    return;
                }

                sDummy = calcInterval.First().receiveDate;
                eDummy = calcInterval.Last().receiveDate;

                var objAllBroker = (from a in db.DailySettlement.Where(o => o.stockId == stockId && o.receiveDate <= sDummy && o.receiveDate >= eDummy)
                                    group a by new
                                    {
                                        a.brokerName,
                                        a.brokerBranch
                                    } into g
                                    select new
                                    {
                                        brokerName = g.Key.brokerName,
                                        brokerBranch = g.Key.brokerBranch,
                                        netVolume = g.Sum(o => o.buyVolume - o.sellVolume),
                                        totalVolume = g.Sum(o => o.buyVolume)
                                    });

                if (objAllBroker.Count() > 0)
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

                    if (db.WeeklyRate.Where(o => o.receiveDate == receiveDate && o.stockId == stockId).Count() == 0)
                    {
                        db.WeeklyRate.Add(new WeeklyRate() { receiveDate = receiveDate, stockId = stockId, stockName = stockName, rate = (decimal?)iRate, totalVolume = (decimal?)iTotal });
                        db.SaveChanges();
                    }
                }
            }
        }

        public void DoTotalRate(DateTime receiveDate, string stockId, string stockName)
        {
            using (stockdbaEntities db = new stockdbaEntities())
            {
                try
                {
                    if (db.TotalRate.Where(o => o.receiveDate == receiveDate && o.stockId == stockId).Count() != 0)
                    {
                        return;
                    }

                    var objAllBroker = (from a in db.DailySettlement.Where(o => o.stockId == stockId && o.receiveDate <= receiveDate)
                                        group a by new
                                        {
                                            a.brokerName,
                                            a.brokerBranch
                                        } into g
                                        select new
                                        {
                                            brokerName = g.Key.brokerName,
                                            brokerBranch = g.Key.brokerBranch,
                                            netVolume = g.Sum(o => o.buyVolume - o.sellVolume),
                                            totalVolume = g.Sum(o => o.buyVolume)
                                        });

                    if (objAllBroker.Count() > 0)
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

                        if (db.TotalRate.Where(o => o.receiveDate == receiveDate && o.stockId == stockId).Count() == 0)
                        {
                            db.TotalRate.Add(new TotalRate() { receiveDate = receiveDate, stockId = stockId, stockName = stockName, rate = (decimal?)iRate, totalVolume = (decimal?)iTotal });
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    db.ServiceLog.Add(new ServiceLog() { updateTime = DateTime.Now, updateLog = String.Format("DoTotalRate:{0}", ex.Message) });
                    db.SaveChanges();
                }
            }
        }

        public void DoSettlement(DateTime receiveDate, string stockId)
        {
            try
            {
                using (stockdbaEntities db = new stockdbaEntities())
                {
                    if (db.DailySettlement.Where(o => o.receiveDate == receiveDate && o.stockId == stockId).Count() != 0)
                    {
                        return;
                    }

                    var obj = from a in db.DailyDetail.Where(o => o.receiveDate == receiveDate && o.stockId == stockId)
                              group a by new
                              {
                                  a.receiveDate,
                                  stockId = a.stockId,
                                  brokerName = a.Broker.name,
                                  brokerBranch = a.Broker.branch,
                                  stockName = a.Company.name,
                              } into g
                              select new
                              {
                                  g.Key.receiveDate,
                                  g.Key.stockId,
                                  g.Key.brokerName,
                                  g.Key.brokerBranch,
                                  g.Key.stockName,
                                  buyVolume = g.Sum(o => o.buyVolume),
                                  sellVolume = g.Sum(o => o.sellVolume),
                                  avgValue = g.Sum(o => o.value * (o.buyVolume + o.sellVolume)) / g.Sum(o => o.buyVolume + o.sellVolume)
                              };
                    if (obj.Count() > 0)
                    {
                        foreach (var item in obj)
                        {
                            db.DailySettlement.Add(new DailySettlement()
                            {
                                receiveDate = item.receiveDate,
                                stockId = item.stockId,
                                stockName = item.stockName,
                                brokerName = item.brokerName,
                                brokerBranch = item.brokerBranch,
                                buyVolume = item.buyVolume,
                                sellVolume = item.sellVolume,
                                avgValue = item.avgValue
                            });
                        }
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
