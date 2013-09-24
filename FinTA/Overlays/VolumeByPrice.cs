using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FinTA.Models;
using Logger;

namespace FinTA.Overlays
{
    public class VolumeByPrice
    {
        private readonly List<MarketData> marketdata;
        private readonly int numOfBlocks;
        public readonly DataTable Data = new DataTable();
        private readonly List<IndicatorsData> resultData = new List<IndicatorsData>();
        
        public VolumeByPrice(List<MarketData> marketdata, int numOfBlocks)
        {
            this.marketdata = marketdata;
            this.numOfBlocks = numOfBlocks;
        }

        public List<IndicatorsData> Calculate(string mode)
        {
            List<MarketData> orderedData = marketdata.OrderBy(mdata => mdata.ClosePrice).ToList(); //work correctly

            List<double> closedPrice = orderedData.Select(mdata => mdata.ClosePrice).ToList();
            List<double> volume = orderedData.Select(mdata => (double)mdata.Volume).ToList();
            List<DateTime> dates = orderedData.Select(mdata => mdata.Date).ToList();

            double blockRange = (closedPrice[closedPrice.Count-1] - closedPrice[0])/numOfBlocks; //divide into 12 blocks
            VolumeByPriceBlock[] blocks = new VolumeByPriceBlock[numOfBlocks];

            int mdIndex = 0;

            for (int i = 0; i < numOfBlocks && mdIndex < marketdata.Count; i++)
            {

                blocks[i] = new VolumeByPriceBlock();
                blocks[i].Price.Add(closedPrice[mdIndex]);
                
                blocks[i].BlockNum = i + 1;
                double posVbp = 0;
                double negVbp = 0;

                for (int j = 0; mdIndex < marketdata.Count; j++)
                {

                    if (closedPrice[mdIndex] <= blocks[i].Price[0] + blockRange)
                    {
                        if (blocks[i].Date.Count >= 1)
                            blocks[i].Price.Add(closedPrice[mdIndex]);

                        blocks[i].Date.Add(dates[mdIndex]);
                        blocks[i].Volume.Add(volume[mdIndex]);
                        double multiplier = mdIndex == 0 ? 0 : dates[mdIndex - 1] < dates[mdIndex] ? 1 : -1;
                        blocks[i].PosNegVolue.Add(volume[mdIndex]*multiplier);
                        
                        if(blocks[i].PosNegVolue[j] > 0)
                        {    
                            posVbp = posVbp + blocks[i].PosNegVolue[j];
                            negVbp = 0;
                        }
                        else
                        {    
                            negVbp = negVbp + blocks[i].PosNegVolue[j];
                            posVbp = 0;
                        }
                        
                        mdIndex++;
                    }
                    else
                    {
                        break;
                    }
                }
                blocks[i].TotalVbp = blocks[i].Volume.Sum();
                blocks[i].NegVbp = negVbp;
                blocks[i].PosVbp = posVbp;

            }

            for (int i = 0; i < numOfBlocks && blocks[i]!=null; i++)
            {
                for (int j = mode.Equals("0") ? 0 : blocks[i].Date.Count - 1; j < blocks[i].Date.Count; j++)
              {    
                  resultData.Add(new IndicatorsData
                  {
                      Instrument = marketdata[i].Instrument,
                      Date = blocks[i].Date[j],
                      Indicatore = "NegVolumeByPrice",
                      Value = blocks[i].NegVbp
                  });

                  resultData.Add(new IndicatorsData
                  {
                      Instrument = marketdata[i].Instrument,
                      Date = blocks[i].Date[j],
                      Indicatore = "PosVolumeByPrice",
                      Value = blocks[i].PosVbp
                  });

                  //FileLogWriter looger = new FileLogWriter();
                  //looger.WriteToLog(DateTime.Now, string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", blocks[i].BlockNum,
                  //              blocks[i].Date[j],
                  //              blocks[i].Price[j],
                  //              blocks[i].Volume[j],
                  //              blockRange,
                  //              blocks[i].TotalVbp,
                  //              blocks[i].PosNegVolue[j],
                  //              blocks[i].NegVbp,
                  //              blocks[i].PosVbp),
                  //              "FinTA");

              }

                
            }

             return resultData;
        }
    }
}



