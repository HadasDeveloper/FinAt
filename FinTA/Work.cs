using System;
using System.Collections.Generic;
using FinTA.Helper;
using FinTA.Indicators;
using FinTA.Models;
using FinTA.Overlays;
using Logger;

namespace FinTA
{
    public class Work
    {
        private List<List<MarketData>> allInstrumentsData = new List<List<MarketData>>();
        private List<IndicatorsData> resultData = new List<IndicatorsData>();

        public void Start(string mode)
        {
            FileLogWriter looger = new FileLogWriter();
            looger.WriteToLog(DateTime.Now, string.Format("{0: fff} start",DateTime.Now), "TimeTest-FinTA");

            allInstrumentsData = DataContext.GetAllInstrumentsMarketData();

            foreach (List<MarketData> data in allInstrumentsData)
            {
                    //-------------   Overlays   --------------

                looger.WriteToLog(DateTime.Now, string.Format("----------   {0}   -----------", data[0].Instrument), "TimeTest-FinTA");
                looger.WriteToLog(DateTime.Now, string.Format("{0: fff} BollingerBands", DateTime.Now), "TimeTest-FinTA");

                BollingerBands bollingerBands = new BollingerBands(data, 2, 20);
                resultData.AddRange(bollingerBands.Calculate(mode));

                looger.WriteToLog(DateTime.Now, string.Format("{0: fff} chandelierExit", DateTime.Now), "TimeTest-FinTA");

                ChandelierExit chandelierExit = new ChandelierExit(data, 22, 3);
                resultData.AddRange(chandelierExit.Calculate(mode));

                //looger.WriteToLog(DateTime.Now, string.Format("{0: fff} chandelierExitShort", DateTime.Now), "TimeTest-FinTA");

                //ChandelierExitShort chandelierExitShort = new ChandelierExitShort(data, 22, 3);
                //resultData.AddRange(chandelierExitShort.Calculate(mode));

                //looger.WriteToLog(DateTime.Now, string.Format("{0: fff} ChandelierExitLong", DateTime.Now), "TimeTest-FinTA");

                //ChandelierExitLong chandelierExitLong = new ChandelierExitLong(data, 22, 3);
                //resultData.AddRange(chandelierExitLong.Calculate(mode));

                looger.WriteToLog(DateTime.Now, string.Format("{0: fff} MovingAverages", DateTime.Now), "TimeTest-FinTA");

                MovingAverages movingAverages = new MovingAverages(data, 10);
                resultData.AddRange(movingAverages.Calculate(mode));

                looger.WriteToLog(DateTime.Now, string.Format("{0: fff} MovingAverageEnvelopes", DateTime.Now), "TimeTest-FinTA");

                MovingAverageEnvelopes movingAverageEnvelopes = new MovingAverageEnvelopes(data, 20, 2.5);
                resultData.AddRange(movingAverageEnvelopes.Calculate(mode));

                looger.WriteToLog(DateTime.Now, string.Format("{0: fff} PriceChannels", DateTime.Now), "TimeTest-FinTA");

                PriceChannels priceChannels = new PriceChannels(data, 20);
                resultData.AddRange(priceChannels.Calculate(mode));

                looger.WriteToLog(DateTime.Now, string.Format("{0: fff} VolumeByPrice", DateTime.Now), "TimeTest-FinTA");

                VolumeByPrice volumebyPrice = new VolumeByPrice(data, 12); // - Is the calculation correct??? //not work on small amount of data (like 78 rows) the rows not divided into 12 blocks
                resultData.AddRange(volumebyPrice.Calculate(mode));

                looger.WriteToLog(DateTime.Now, string.Format("{0: fff} VolumeWeightedAveragePrice",DateTime.Now), "TimeTest-FinTA");

                VolumeWeightedAveragePrice volumeWeightedAveragePrice = new VolumeWeightedAveragePrice(data,79); 
                resultData.AddRange(volumeWeightedAveragePrice.Calculate(mode));

                //-------------   Indicators   --------------

                looger.WriteToLog(DateTime.Now, string.Format("{0: fff} AccumulationDistributionLine",DateTime.Now), "TimeTest-FinTA");

                AccumulationDistributionLine accumulationDistributionLine = new AccumulationDistributionLine(data,79); 
                resultData.AddRange(accumulationDistributionLine.Calculate(mode));

                looger.WriteToLog(DateTime.Now, string.Format("{0: fff} Aroon",DateTime.Now), "TimeTest-FinTA");

                Aroon aroon = new Aroon(data, 25); // - Is the calculation correct???
                resultData.AddRange(aroon.Calculate(mode));

                looger.WriteToLog(DateTime.Now, string.Format("{0: fff} AverageDirectionalIndex",DateTime.Now), "TimeTest-FinTA");

                AverageDirectionalIndex averageDirectionalIndex = new AverageDirectionalIndex(data, 14); 
                resultData.AddRange(averageDirectionalIndex.Calculate(mode));

                looger.WriteToLog(DateTime.Now, string.Format("{0: fff} AverageTrueRange",DateTime.Now), "TimeTest-FinTA");

                AverageTrueRange averageTrueRange = new AverageTrueRange(data, 14); 
                resultData.AddRange(averageTrueRange.Calculate(mode));

                looger.WriteToLog(DateTime.Now, string.Format("{0: fff} BandWidth",DateTime.Now), "TimeTest-FinTA");

                BandWidth bandWidth = new BandWidth(data, 2, 20); 
                resultData.AddRange(bandWidth.Calculate(mode));

                looger.WriteToLog(DateTime.Now, string.Format("{0: fff} BIndicator",DateTime.Now), "TimeTest-FinTA");

                BIndicator bIndicator = new BIndicator(data, 2, 20); 
                resultData.AddRange(bIndicator.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} CommodityChannelIndex",DateTime.Now), "TimeTest-FinTA");

                CommodityChannelIndex commodityChannelIndex = new CommodityChannelIndex(data, 20); 
                resultData.AddRange(commodityChannelIndex.Calculate(mode));
                
                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} EaseOfMovement",DateTime.Now), "TimeTest-FinTA");

                EaseOfMovement easeOfMovement = new EaseOfMovement(data, 14);
                resultData.AddRange(easeOfMovement.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} ForceIndex",DateTime.Now), "TimeTest-FinTA");

                ForceIndex forceIndex = new ForceIndex(data, 13);  
                resultData.AddRange(forceIndex.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} KnowSureThing",DateTime.Now), "TimeTest-FinTA");

                KnowSureThing knowSureThing = new KnowSureThing(data); 
                resultData.AddRange(knowSureThing.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} MassIndex",DateTime.Now), "TimeTest-FinTA");

                MassIndex massIndex = new MassIndex(data, 9, 25); 
                resultData.AddRange(massIndex.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} MovingAverageConvergenceDivergence",DateTime.Now), "TimeTest-FinTA");

                MovingAverageConvergenceDivergence macd = new MovingAverageConvergenceDivergence(data, 12, 26, 9);
                resultData.AddRange(macd.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} MoneyFlowIndex",DateTime.Now), "TimeTest-FinTA");

                MoneyFlowIndex moneyFlowIndex = new MoneyFlowIndex(data, 14); 
                resultData.AddRange(moneyFlowIndex.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} NegativeVolumeIndex",DateTime.Now), "TimeTest-FinTA");

                NegativeVolumeIndex negativeVolumeIndex = new NegativeVolumeIndex(data);
                resultData.AddRange(negativeVolumeIndex.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} OnBalanceVolume",DateTime.Now), "TimeTest-FinTA");

                OnBalanceVolume onBalanceVolume = new OnBalanceVolume(data,79);  
                resultData.AddRange(onBalanceVolume.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} PercentagePriceOscillator",DateTime.Now), "TimeTest-FinTA");

                PercentagePriceOscillator percentagePriceOscillator = new PercentagePriceOscillator(data, 12, 26, 9); 
                resultData.AddRange(percentagePriceOscillator.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} PercentageVolumeOscillator",DateTime.Now), "TimeTest-FinTA");

                PercentageVolumeOscillator percentageVolumeOscillator = new PercentageVolumeOscillator(data, 12, 26, 9); 
                resultData.AddRange(percentageVolumeOscillator.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} PriceRelative",DateTime.Now), "TimeTest-FinTA");

                PriceRelative priceRelative = new PriceRelative(data, "SBUX");
                resultData.AddRange(priceRelative.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} RateOfChange",DateTime.Now), "TimeTest-FinTA");

                RateOfChange pateOfChange = new RateOfChange(data, 12);
                resultData.AddRange(pateOfChange.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} RelativeStrengthIndex",DateTime.Now), "TimeTest-FinTA");

                RelativeStrengthIndex relativeStrengthIndex = new RelativeStrengthIndex(data, 14);
                resultData.AddRange(relativeStrengthIndex.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} StandardDeviationVolatility",DateTime.Now), "TimeTest-FinTA");

                StandardDeviationVolatility standardDeviationVolatility = new StandardDeviationVolatility(data, 10); 
                resultData.AddRange(standardDeviationVolatility.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} StochasticOscillator",DateTime.Now), "TimeTest-FinTA");

                StochasticOscillator stochasticOscillator = new StochasticOscillator(data, 14);
                resultData.AddRange(stochasticOscillator.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} StochRSI",DateTime.Now), "TimeTest-FinTA");

                StochRSI stochRsi = new StochRSI(data, 14);    //Done new
                resultData.AddRange(stochRsi.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} Trix",DateTime.Now), "TimeTest-FinTA");

                Trix trix = new Trix(data, 15);    //Done new
                resultData.AddRange(trix.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} TrueStrengthIndex",DateTime.Now), "TimeTest-FinTA");

                TrueStrengthIndex trueStrengthIndex = new TrueStrengthIndex(data, 25, 13);  //Done new
                resultData.AddRange(trueStrengthIndex.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} UlcerIndex",DateTime.Now), "TimeTest-FinTA");

                UlcerIndex ulcerIndex = new UlcerIndex(data, 14);    //Done new
                resultData.AddRange(ulcerIndex.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} VortexIndicator",DateTime.Now), "TimeTest-FinTA");

                VortexIndicator vortexIndicator = new VortexIndicator(data, 14); //Done new
                resultData.AddRange(vortexIndicator.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} WilliamR",DateTime.Now), "TimeTest-FinTA");

                WilliamR williamR = new WilliamR(data, 14);  //Done new
                resultData.AddRange(williamR.Calculate(mode));

                looger.WriteToLog(DateTime.Now,string.Format("{0: fff} Write To DB",DateTime.Now), "TimeTest-FinTA");
                
                if(mode.Equals("0"))
                {
                    DataContext.WriteIndicatorsData(resultData);      
                    resultData = new List<IndicatorsData>();
                }
  
            }

            if (mode.Equals("1"))
                DataContext.WriteIndicatorsData(resultData);
            
            looger.WriteToLog(DateTime.Now,string.Format("{0: fff} Done",DateTime.Now), "TimeTest-FinTA");
                 

            Console.ReadKey();
        }
    }

}
