
namespace FinTA.Models
{
    internal class StoredProcedures
    {
        public const string SqlGetInstrumentsMarketData =
            "usp_FinAt_Get_HistoryIntraDay_Data '{0}', '{1}'";

        public const string SqlWriteIndicatorsData =
            "usp_FinAt_Insert_Indicators_Values";
    }
}
