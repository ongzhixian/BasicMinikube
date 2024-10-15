# Trade

1.  1 Manager 
2.  <N> Analysts
3.  <N> Traders

## Manager

Startup
1.  If daily tasks were not executed, execute daily tasks.

Daily tasks:
1.  Get list of tradable markets
2.  Get list of tradable instruments fpr each tradable market
3.  Get macro/micro market data for each tradable instrument
4.  Dispatch market data to analysts

Ad-hoc tasks:
1.  Wait for analyst projection
    If projection suggest trade, place older

## Analyst

1.  Wait for market data
2.  Analyze market data
3.  Generate projection
4.  Send projection to manager
