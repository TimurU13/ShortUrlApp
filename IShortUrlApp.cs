namespace CutReference
{
    public interface IShortUrlApp
    {
        public string SaveUrl(string longUrl);
        public string GetLongUrl(string shortUrl);
        public bool DeleteShortUrl(string shortUrl);
    }
}
