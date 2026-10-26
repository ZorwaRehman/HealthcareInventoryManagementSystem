
using System.Collections.Generic;
using System.Threading.Tasks;
using HealthcareInventoryManagementSystem.Application.DTOs.Alerts;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareInventoryManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _alertService;

        public AlertController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        // GET: api/Alert
        // Returns all low-stock and expiry alerts.
        [HttpGet]
        [ProducesResponseType(
            typeof(IEnumerable<AlertDto>),
            StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AlertDto>>> GetAllAlerts()
        {
            var alerts = await _alertService.GetAllAlertsAsync();

            return Ok(alerts);
        }

        // GET: api/Alert/low-stock
        // Returns alerts for items at or below their reorder level.
        [HttpGet("low-stock")]
        [ProducesResponseType(
            typeof(IEnumerable<AlertDto>),
            StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AlertDto>>> GetLowStockAlerts()
        {
            var alerts = await _alertService.GetLowStockAlertsAsync();

            return Ok(alerts);
        }

        // GET: api/Alert/expiry
        // Returns alerts for expired batches and batches expiring within 30 days.
        [HttpGet("expiry")]
        [ProducesResponseType(
            typeof(IEnumerable<AlertDto>),
            StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AlertDto>>> GetExpiryAlerts()
        {
            var alerts = await _alertService.GetExpiryAlertsAsync();

            return Ok(alerts);
        }
    }
}
