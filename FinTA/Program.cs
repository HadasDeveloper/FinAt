﻿
using System;
using Logger;

namespace FinTA
{
    public class Program
    {
        static void Main(string[] args)
        {
            Work work = new Work();

            FileLogWriter looger = new FileLogWriter();
            looger.WriteToLog(DateTime.Now, string.Format("{0: fff} start", DateTime.Now), "TimeTest-FinTA");
            
            if (args.Length > 0)
                if(args[0].Equals("0") || args[0].Equals("1")) // 0 for long mode , 1 for 1 day
                    work.Start(args);     
        }
    }
}
