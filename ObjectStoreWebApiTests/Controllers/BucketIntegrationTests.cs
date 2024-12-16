using System.Net;

using Microsoft.AspNetCore.Mvc.Testing;

namespace ObjectStoreWebApi.Controllers.Tests;

[TestClass]
//[TestCategory("Integration")]
public class BucketIntegrationTests
{
    private WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>();
    private readonly HttpClient httpClient;


    public BucketIntegrationTests()
    {
        httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }
    

    [TestMethod]
    public async Task GetBucketListTestAsync()
    {
        var response = await httpClient.GetAsync("/");
        
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    //[TestMethod]
    //public async Task GetBucketListTestAsync()
    //{
    //    var response = await httpClient.PutAsync("/test-bucket", null);

    //    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    //}
}