using ShortUrlSrv.App.DAL;

namespace ShortUrlSrv.App;

class Program
{
    static void Main(string[] args)
    {
        IUrlDataStore urlDataStore = new UrlDataStore();
        IShortUrlApp shortUrlApp=new ShortUrlApp(urlDataStore);
        
        Console.WriteLine("Введите оригинальный URL:");
        string longUrl = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(longUrl))
        {
            Console.WriteLine("URL не может быть пустым или содержать только пробелы.");
            return;
        }

        string shortUrl = shortUrlApp.SaveUrl(longUrl);
        if (shortUrl != null)
        {
            Console.WriteLine($"Короткая ссылка: {shortUrl}");
        }
        else
        {
            Console.WriteLine("Не удалось создать короткую ссылку.");
        }

        try
        {
            string originalUrl = shortUrlApp.GetLongUrl(shortUrl);
            if (originalUrl != null)
            {
                Console.WriteLine($"Оригинальная ссылка: {originalUrl}");
            }
            else
            {
                Console.WriteLine("Не удалось найти оригинальную ссылку.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

}