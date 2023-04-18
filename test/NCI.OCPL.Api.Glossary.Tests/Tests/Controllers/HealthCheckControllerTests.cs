using System;

using Microsoft.Extensions.Logging.Testing;

using Moq;
using Xunit;

using NCI.OCPL.Api.Common;
using NCI.OCPL.Api.Common.Testing;
using NCI.OCPL.Api.Glossary.Controllers;


namespace NCI.OCPL.Api.Glossary.Tests
{
  public class HealthCheckControllerTests
  {
    /// <summary>
    /// Handle the healthcheck reporting an unhealthy status.
    /// </summary>
    [Fact]
    public async void IsUnhealthy()
    {
      var logger = NullLogger<HealthCheckController>.Instance;
      var healthcheckService = new Mock<IHealthCheckService>();
      healthcheckService.Setup(
        svc => svc.IndexIsHealthy()
      )
      .ReturnsAsync(false);

      var controller = new HealthCheckController(logger, healthcheckService.Object);

      var exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.IsHealthy());

      Assert.Equal(500, exception.HttpStatusCode);
      Assert.Equal(HealthCheckController.UNHEALTHY_STATUS, exception.Message);
    }


    /// <summary>
    /// Handle the health service failing.
    /// </summary>
    [Fact]
    public async void IsVeryUnhealthy()
    {
      var logger = NullLogger<HealthCheckController>.Instance;
      var healthcheckService = new Mock<IHealthCheckService>();
      healthcheckService.Setup(
        svc => svc.IndexIsHealthy()
      )
      .ThrowsAsync(new Exception("kaboom!"));


      var controller = new HealthCheckController(logger, healthcheckService.Object);

      var exception = await Assert.ThrowsAsync<APIErrorException>(() => controller.IsHealthy());

      Assert.Equal(500, exception.HttpStatusCode);
      Assert.Equal(HealthCheckController.UNHEALTHY_STATUS, exception.Message);
    }


    /// <summary>
    /// Handle the healthcheck reporting a healthy status.
    /// </summary>
    [Fact]
    public async void IsHealthy()
    {
      var logger = NullLogger<HealthCheckController>.Instance;
      var healthcheckService = new Mock<IHealthCheckService>();
      healthcheckService.Setup(
        healthSvc => healthSvc.IndexIsHealthy()
      )
      .ReturnsAsync(true);

      var controller = new HealthCheckController(logger, healthcheckService.Object);

      string actual = await controller.IsHealthy();

      healthcheckService.Verify(
        svc => svc.IndexIsHealthy(),
        Times.Once
      );

      Assert.Equal(HealthCheckController.HEALTHY_STATUS, actual);
    }

  }
}