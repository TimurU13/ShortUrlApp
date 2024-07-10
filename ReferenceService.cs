namespace CutReference
{
    public class ShortUrlApp : IShortUrlApp
    {
        public bool DeleteShortUrl(string shortUrl)
        {
            throw new NotImplementedException();
        }

        public string GetLongUrl(string shortUrl)
        {
            Uri uri = new Uri(shortUrl);
            string checkValidUri = string.Concat("https://", uri.Authority, "/");
            string shortUrlDomain = "https://myshorturl.io/";
            if (checkValidUri != shortUrlDomain)
            {
                throw new Exception("Полученная короткая ссылка от пользователя-неверного формата");
            }
            else if (uri.Segments.Length != 2)
            {
                throw new Exception("Полученная короткая ссылка от пользователя-неверного формата");
            }
            else
            {
                string shortId = uri.Segments[1];
                return "dsdss";
            }

        }
        public string SaveUrl(string longUrl)
        {
            string shortUrl = "https://myshorturl.io/";
            int shortId = 1;
            Uri uriResult;
            bool result = Uri.TryCreate(longUrl, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (result == false)
            {
                throw new Exception("Полученная короткая ссылка от пользователя-неверного формата");
            }
            else
            {
                return string.Concat(shortUrl, shortId);
            }
        }
    }
}
