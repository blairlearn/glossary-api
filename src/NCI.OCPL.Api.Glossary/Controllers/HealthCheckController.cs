using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

using NCI.OCPL.Api.Common;


namespace NCI.OCPL.Api.Glossary.Controllers
{

  /// <summary>
  /// Controller for reporting on system health.
  /// </summary>
  [Route("HealthCheck")]
  public class HealthCheckController : ControllerBase
  {

    /// <summary>
    /// Message to return for a "healthy" status.
    /// </summary>
    public const string HEALTHY_STATUS = "alive!";

    /// <summary>
    /// Message to return for an "unhealthy" status.
    /// </summary>
    public const string UNHEALTHY_STATUS = "Service not healthy.";

    private readonly IHealthCheckService _healthSvc;
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="healthCheckSvc">Elasticsearch health check instance.</param>
    /// <returns></returns>
    public HealthCheckController(ILogger<HealthCheckController> logger, IHealthCheckService healthCheckSvc)
      => (_logger, _healthSvc) = (logger, healthCheckSvc);


    /// <summary>
    /// Provides an endpoint for checking that the glossary API and underlying Elasticsearch instance
    /// are in a good operational state.
    /// </summary>
    /// <returns>Status 200 and the message "alive!" if the service is healthy.
    ///
    /// Status 500  and the message "Service not healthy." otherwise.</returns>
    [HttpGet("status")]
    public async Task<string> IsHealthy()
    {
      try
      {
          bool isHealthy = await _healthSvc.IndexIsHealthy(); ;
          if (isHealthy)
              return HEALTHY_STATUS;
          else
              throw new APIErrorException(500, UNHEALTHY_STATUS);
      }
      catch (Exception ex)
      {
          _logger.LogError(ex, "Error checking health.");
          throw new APIErrorException(500, UNHEALTHY_STATUS);
      }
    }
  }
}