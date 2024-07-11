using System;
using ShortUrlSrv.App.DAL;

namespace ShortUrlSrv.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IUrlDataStore urlDataStore = new UrlDataStore("store.txt");
            IShortUrlApp shortUrlApp = new ShortUrlApp(urlDataStore);

            while (true)
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1 - Получить короткую ссылку");
                Console.WriteLine("2 - Получить длинную ссылку");
                Console.WriteLine("3 - Завершить программу");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        GetShortUrl(shortUrlApp);
                        break;
                    case "2":
                        GetLongUrl(shortUrlApp);
                        break;
                    case "3":
                        urlDataStore.Dispose();
                        Console.WriteLine("Программа завершена.");
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        static void GetShortUrl(IShortUrlApp shortUrlApp)
        {
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
        }

        static void GetLongUrl(IShortUrlApp shortUrlApp)
        {
            Console.WriteLine("Введите короткую ссылку:");
            string shortUrl = Console.ReadLine();

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
}