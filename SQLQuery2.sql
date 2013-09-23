select * from HistoryEndOfDay where Instrument = 'spy' order by date desc

select * from [Sentiment_HistoryIntraDay] where Instrument = 'spy'  and date >=  (select dateadd(HOUR,-7, (select max (date) from [Sentiment_HistoryIntraDay] ))) order by date  


select * from [Sentiment_HistoryIntraDay] where Instrument = 'spy'  order by date

select DATEADD((select Max(date) from  Sentiment_HistoryIntraDay),-1 , 


select count(date),instrument 
from Sentiment_HistoryIntraDay
where date >= (select dateadd(HOUR,-7, (select max (date) from [Sentiment_HistoryIntraDay] )))
group by instrument


select * from #temp

select * 
from Sentiment_HistoryIntraDay where Instrument = 'spy'   order by date 

select t.* , rank (partition by )


select count (date) ,day
from
(SELECT date, DATEADD(dd, 0, DATEDIFF(dd, 0, (select date from Sentiment_HistoryIntraDay where Instrument = 'spy' and date > '2013-6-27' and sh.date = date))) as day
from [Sentiment_HistoryIntraDay] sh where Instrument = 'spy' and date > '2013-6-27')q
group by day



select * from [Sentiment_HistoryIntraDay] where date > '2013-08-05'  and Instrument = 'spy' order by date desc

select max (date) 
from [Sentiment_HistoryIntraDay]
where 

select * from Sentiment_HistoryIntraDay where Instrument = 'AGQ' and date >= (select dateadd(month ,24, (select min (date) from [Sentiment_HistoryIntraDay] ))) order by closePrice  

select * 

 table FinAT_Indicators_Values
(
	Instrument nvarchar(100) NOT NULL,
	Date smalldatetime NOT NULL,
	Indicator nvarchar(100) NOT NULL,
	Value [decimal](18, 4) NOT NULL,
	CONSTRAINT [pk_FinAT_Indicators_Instrument_Date_Indicator] PRIMARY KEY CLUSTERED 
(
	Instrument ASC,
	Date ASC,
	Indicator
))


USE [Dev]
GO

INSERT INTO FinAT_Indicators_Values(Instrument,Date,Indicator,Value)VALUES( , , , )
GO


INSERT INTO FinAT_Indicators_Values(Instrument,Date,Indicator,Value)VALUES( 'SPY',05/08/2013 16:00:00,'BollingerBands',6.01665505749132000000000000)

select * from FinAT_Indicators_Values

--truncate table FinAT_Indicators_Values

usp_FinAt_Insert_Indicators_Values 'SPY','06/13/2013 09:35:00','BollingerBands',0.62909950373529200000


select distinct instrument from Sentiment_HistoryIntraDay