using System;
using System.Collections.Generic;
using System.Data;
using FinTA.Models;
using Logger;

namespace FinTA.Helper
{
    class DataContext
    {
        public static List<MarketData> GetOneInstrumentMarketData(string symbol,string date)
        {
            DataTable table = DataHelper.GetMarketData(symbol, date);
            List<MarketData> marketData = new List<MarketData>();

            FileLogWriter logWriter = new FileLogWriter();

            foreach (DataRow row in table.Rows)
            {
                MarketData data = new MarketData {Instrument = row.Field<string>("Instrument")};

                try
                {
                    data.Date = row.Field<DateTime>("Date"); 
                }
                catch(Exception e)
                {
                    logWriter.WriteToLog(DateTime.Now, string.Format("DataContext.GetMarketData: {0}", e.Message), "FinTA");                              
                }
                data.OpenPrice = Convert.ToDouble(row.Field<double>("OpenPrice"));
                data.HighPrice = Convert.ToDouble(row.Field<double>("HighPrice"));
                data.LowPrice = Convert.ToDouble(row.Field<double>("LowPrice"));
                data.ClosePrice = Convert.ToDouble(row.Field<double>("ClosePrice"));
                data.Volume = row.Field<double>("Volume");                            
                                            
                marketData.Add(data);
            }

            return marketData;
        }

        public static List<List<MarketData>> GetAllInstrumentsMarketData(string date)
        {
            DataTable table = DataHelper.GetMarketData("", date);
            List<List<MarketData>> marketData = new List<List<MarketData>>();

            FileLogWriter logWriter = new FileLogWriter();
            List<MarketData> instrumentData = new List<MarketData>();

            string curentInstrument = table.Rows[0].Field<string>("Instrument");

            foreach (DataRow row in table.Rows)
            {
                MarketData data = new MarketData {Instrument = row.Field<string>("Instrument")};

                if (!data.Instrument.Equals(curentInstrument))
                {  
                    marketData.Add(instrumentData);
                    curentInstrument = data.Instrument;
                    instrumentData = new List<MarketData>();
                }

                try
                {
                    data.Date = row.Field<DateTime>("Date") ; 
                }
                catch(Exception e)
                {
                    logWriter.WriteToLog(DateTime.Now, string.Format("DataContext.GetMarketData: {0}", e.Message), "FinTA");                              
                }

                data.OpenPrice = Convert.ToDouble(row.Field<decimal>("OpenPrice"));
                data.HighPrice = Convert.ToDouble(row.Field<decimal>("HighPrice"));
                data.LowPrice = Convert.ToDouble(row.Field<decimal>("LowPrice"));
                data.ClosePrice = Convert.ToDouble(row.Field<decimal>("ClosePrice"));
                data.Volume = row.Field<double>("Volume");  

                instrumentData.Add(data);
            }
         
            if (instrumentData.Count > 0)
                marketData.Add(instrumentData);


            return marketData;
        }

        public static void WriteIndicatorsData(List<IndicatorsData> resultData)
        {
            DataTable data = new DataTable();

            data.Columns.Add("Instrument", typeof(string));
            data.Columns.Add("Date", typeof(DateTime));
            data.Columns.Add("Indicatore", typeof(string));
            data.Columns.Add("Value", typeof(double));

            foreach (IndicatorsData indicator in resultData)
            {
                data.Rows.Add(indicator.Instrument,
                              indicator.Date,
                              indicator.Indicatore,
                              indicator.Value);
            }

            DataHelper halper = new DataHelper();
            halper.WriteIndicatorsData(data);
   
        }
    }
}

