using Microsoft.Extensions.Logging;

using Moq;

using ObjectStoreWebApi.Services;

namespace ObjectStoreWebApi.Controllers.Tests;

[TestClass()]
public class S3ControllerTests
{
    private readonly Mock<ILogger<S3Controller>> logger = new();
    private readonly Mock<IObjectStoreService> objectStoreService = new();

    public S3ControllerTests()
    {
    }

    

    public void GetBucketListTest()
    {
        objectStoreService.Setup(m => m.ListBuckets());

        S3Controller controller = new S3Controller(logger.Object, objectStoreService.Object);

        controller.GetBucketList();

        objectStoreService.Verify(mock => mock.ListBuckets(), Times.Once());
    }
}