using ShortUrlAppWebAPI.DAL;
namespace ShortUrlAppWebAPI.Services;
public class ShortUrlApp : IShortUrlApp
{
    private readonly IUrlDataStore _urlDataStore;
    public ShortUrlApp(IUrlDataStore urlDataStore)
    {

        _urlDataStore = urlDataStore;
    }
    public bool DeleteShortUrl(string shortUrl)
    {
        Uri uri = new Uri(shortUrl);
        string shortId = uri.Segments[1];
        return _urlDataStore.DeleteShortID(shortId);

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
            (var result, bool status) = _urlDataStore.GetLongUrl(shortId);
            return result;
        }

    }
    public string SaveUrl(string longUrl)
    {
        string shortUrl = "https://myshorturl.io/";
        string shortUrlId = _urlDataStore.SaveLongUrl(longUrl);
        Uri uriResult;
        bool result = Uri.TryCreate(longUrl, UriKind.Absolute, out uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        if (result == false)
        {
            throw new Exception("Полученная короткая ссылка от пользователя-неверного формата");
        }
        else
        {
            return string.Concat(shortUrl, shortUrlId);
        }
    }
}