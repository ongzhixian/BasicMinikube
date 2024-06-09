using System.Diagnostics.Metrics;

namespace WareLogix.WebApi.Metrics;

public class RequestMetric
{
    private int totalRequestsCount;

    private readonly Counter<int> requestsReceivedRate;

    private readonly object dataLock = new();

    public RequestMetric(string meterName, IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(meterName);

        requestsReceivedRate = meter.CreateCounter<int>("Rate of requests received", "requests", "Number of requests received");
        
        _ = meter.CreateObservableGauge<int>("Total requests received", getTotalRequestsCount, "requests", "Total number of requests received");
    }

    public RequestMetric(IMeterFactory meterFactory) : this("WareLogix.WebApi", meterFactory) { }

    private int getTotalRequestsCount()
    {
        return totalRequestsCount;
    }

    public void Increment(int requestCount = 1)
    {
        lock(dataLock)
        {
            totalRequestsCount += requestCount;
            requestsReceivedRate.Add(requestCount);
        }
    }
}
