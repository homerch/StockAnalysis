using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    public class WorkItem
    {
        public DateTime receiveDate;
        public string stockId;
        public int current, total;
    }

    public class QueryStockItem
    {
        public string stockId { set; get; }
        public string stockName { set; get; }
        public decimal? tradeAmt { set; get; }
        public decimal? netVolume { set; get; }
        public decimal? avgValue { set; get; }
    }

    public class QueryBrokerItem
    {
        public string brokerName { set; get; }
        public string brokerBranch { set; get; }
        public decimal? netVolume { set; get; }
        public decimal? totalVolume { set; get; }
        public decimal? avgValue { set; get; }
        public decimal? latestVolume { set; get; }
    }
}
