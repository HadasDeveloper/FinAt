namespace FinTA.Models
{
    class Timeframes
    {
        public int Roc1 { get; set; }
        public int Roc2 { get; set; }
        public int Roc3 { get; set; }
        public int Roc4 { get; set; }
        public int Sma1 { get; set; }
        public int Sma2 { get; set; }
        public int Sma3 { get; set; }
        public int Sma4 { get; set; }
        public int Signal { get; set; }

        public Timeframes GetShortTermDaily()
        {
            Timeframes frame = new Timeframes
            {
                Roc1 = 10,
                Roc2 = 15,
                Roc3 = 20,
                Roc4 = 30,
                Sma1 = 10,
                Sma2 = 10,
                Sma3 = 10,
                Sma4 = 15,
                Signal = 9
            };

            return frame;
        }


        public Timeframes MediumTermWeekly()
        {
            Timeframes frame = new Timeframes
            {
                Roc1 = 10,
                Roc2 = 13,
                Roc3 = 15,
                Roc4 = 20,
                Sma1 = 10,
                Sma2 = 13,
                Sma3 = 15,
                Sma4 = 20,
                Signal = 9,
            };

            return frame;
        }

        public Timeframes LongTermMonthly()
        {
            Timeframes frame = new Timeframes
            {
                Roc1 = 9,
                Roc2 = 12,
                Roc3 = 18,
                Roc4 = 24,
                Sma1 = 6,
                Sma2 = 6,
                Sma3 = 6,
                Sma4 = 9,
                Signal = 9,

            };

            return frame;
        }
    }
}
