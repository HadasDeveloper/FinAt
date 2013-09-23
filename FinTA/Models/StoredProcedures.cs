
namespace FinTA.Models
{
    public class StoredProcedures
    {
        ////all days from HistoryEndOfDay
        //public const string SqlGetMarketData = "select * from HistoryEndOfDay where instrument = '{0}' order by date";

        ////one day back from Sentiment_HistoryIntraDay
        //public const string SqlGetMarketData =
        //    "select * from Sentiment_HistoryIntraDay where Instrument = '{0}' and date >= (select dateadd(HOUR,-7, (select max (date) from Sentiment_HistoryIntraDay ))) order by date "; 

        ////all dates from Sentiment_HistoryIntraDay for all instruments
        public const string SqlGetAllInstrumentsMarketData =
            "usp_FinAt_Get_HistoryIntraDay_Data '{0}'";

        ////all dates from Sentiment_HistoryIntraDay for one instrument
        public const string SqlGetOneInstrumentMarketData =
            //    "select * from Sentiment_HistoryIntraDay where Instrument = '{0}' and date >= (select dateadd(month,24, (select min (date) from Sentiment_HistoryIntraDay ))) order by date  ";
            "usp_FinAt_Get_HistoryIntraDay_Data '{0}'";

        public const string SqlWriteIndicatorsData =
            "usp_FinAt_Insert_Indicators_Values";
    }
}
