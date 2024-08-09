using Crawler.Dto;
using Crawler.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Crawler.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CrawlerController : ControllerBase
    {
        private readonly IManager _manager;
        private readonly ILogger<CrawlerController> _logger;

        public CrawlerController(IManager manager, ILogger<CrawlerController> logger)
        {
            _manager = manager;
            _logger = logger;
        }

        [HttpPost("start")]
        public IActionResult StartCrawling([FromBody] UrlModel request)
        {
            if (request == null || string.IsNullOrEmpty(request.Url))
                return BadRequest("URL empty");

            var requestId = _manager.ImageRequest(request.Url);
            _logger.LogInformation($"Finding images {request.Url} with RequestId {requestId}");

            var response = new StartDTO
            {
                RequestId = requestId
            };

            return Ok(response);
        }

        [HttpGet("status/{id}")]
        public IActionResult GetStatus(string id)
        {
            var (status, downloadUrl) = _manager.GetStatus(id);

            if (status == Status.NotFound)
            {
                return NotFound("Request not found");
            }

            var response = new GetStatusDTO
            {
                Status = status.ToString(),
                DownloadUrl = downloadUrl
            };

            return Ok(response);
        }
    }
}