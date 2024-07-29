using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShortUrlAppWebAPI.Services;
using ShortUrlAppWebAPI.Models;
using ShortUrlAppWebAPI.DTO;
namespace ShortUrlAppWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShortUrlApiController : ControllerBase
    {
        private readonly IShortUrlApp _shortUrlApp;
        private readonly ILogger<ShortUrlApiController> _logger;

        public ShortUrlApiController(IShortUrlApp shortUrlApp, ILogger<ShortUrlApiController> logger)
        {
            _shortUrlApp = shortUrlApp;
            _logger = logger;
        }
        [HttpGet("long-url")]
        public IActionResult GetLongUrl([FromQuery] ShortUrl shortUrlRequest)
        {
            /*if (string.IsNullOrWhiteSpace(shortUrlRequest.ShortUrl))
            {
                _logger.LogWarning("Получен запрос с неверной короткой ссылкой {ShortUrl}", shortUrlRequest.ShortUrl);
                return BadRequest(ApiStatusCode.InvalidShortUrl.CreateResponse<string>());
            }
            */
            if (string.IsNullOrWhiteSpace(shortUrlRequest.shortUrl))
            {
                _logger.LogWarning("Получен запрос с неверной короткой ссылкой {ShortUrl}", shortUrlRequest.shortUrl);
                return this.CreateBadRequestResponse<string>(ApiStatusCode.InvalidShortUrl);
            }
            try
            {
                string longUrl = _shortUrlApp.GetLongUrl(shortUrlRequest.shortUrl);
                if (longUrl == null)
                {
                    _logger.LogWarning("Ссылка не найдена для короткой ссылки {ShortUrl}", shortUrlRequest.shortUrl);
                    return this.CreateNotFoundResponse<string>(ApiStatusCode.LinkNotFound);
                }

                _logger.LogInformation("Получен длинный URL для короткой ссылки {ShortUrl}: {LongUrl}", shortUrlRequest.shortUrl, longUrl);
                return this.CreateSuccessResponse(ApiStatusCode.Success, longUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении длинного URL для короткой ссылки {ShortUrl}", shortUrlRequest.shortUrl);
                return this.CreateErrorResponse<string>(ApiStatusCode.InternalError);
            }
        }

        [HttpPost("long-url")]
        public IActionResult SaveUrl([FromBody] LongUrl longUrlRequest)
        {
            if (longUrlRequest == null || string.IsNullOrWhiteSpace(longUrlRequest.longUrl))
            {
                _logger.LogWarning("Получен запрос с неверной длинной ссылкой {LongUrl}", longUrlRequest?.longUrl);
                return this.CreateBadRequestResponse<string>(ApiStatusCode.InvalidLongUrl);
            }

            try
            {
                string shortUrl = _shortUrlApp.SaveUrl(longUrlRequest.longUrl);
                _logger.LogInformation("Пользователь передал следующую ссылку {LongUrl}", longUrlRequest.longUrl);
                return this.CreateSuccessResponse(ApiStatusCode.Success, shortUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении длинного URL {LongUrl}", longUrlRequest.longUrl);
                return this.CreateErrorResponse<string>(ApiStatusCode.InternalError);
            }
        }

        [HttpDelete("url")]
        [Authorize]
        public IActionResult DeleteShortUrl([FromQuery] ShortUrl shortUrlRequest)
        {
            if (string.IsNullOrWhiteSpace(shortUrlRequest.shortUrl))
            {
                _logger.LogWarning("Получен запрос с неверной короткой ссылкой {ShortUrl}", shortUrlRequest.shortUrl);
                return this.CreateBadRequestResponse<string>(ApiStatusCode.InvalidShortUrl);
            }

            try
            {
                bool result = _shortUrlApp.DeleteShortUrl(shortUrlRequest.shortUrl);
                if (!result)
                {
                    _logger.LogWarning("Ссылка не найдена для удаления {ShortUrl}", shortUrlRequest.shortUrl);
                    return this.CreateNotFoundResponse<string>(ApiStatusCode.LinkNotFound);
                }

                _logger.LogInformation("Удалена короткая ссылка {ShortUrl}", shortUrlRequest.shortUrl);
                return this.CreateSuccessResponse<string>(ApiStatusCode.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении короткой ссылки {ShortUrl}", shortUrlRequest.shortUrl);
                return this.CreateErrorResponse<string>(ApiStatusCode.InternalError);
            }
        }
    }
}