namespace ShortUrlAppWebAPI.DTO
{
    public class ApiResponse<T>
    {
        public ApiStatusCode StatusCode { get; set; }
        public T Data { get; set; }
    }

    public enum ApiStatusCode
    {
        Success = 200,
        InvalidShortUrl,
        LinkNotFound,
        InvalidLongUrl,
        InternalError
    }

    public static class ApiResponseDTO
    {
        public static ApiResponse<T> CreateResponse<T>(this ApiStatusCode statusCode, T data = default)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Data = data
            };
        }
    }
}