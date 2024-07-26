using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ShortUrlAppWebAPI.DAL
{
    public class UrlDataStore : IUrlDataStore, IDisposable
    {
        private Dictionary<int, string> _urlStore;
        private int _currentKey;
        private readonly string _path;

        public UrlDataStore(string path)
        {
            _urlStore = new Dictionary<int, string>();
            _currentKey = 0;
            _path = path;

            if (!File.Exists(_path))
            {
                using (var fileStream = File.Create(_path)) { }
            }
            else
            {
                string json = File.ReadAllText(_path);
                if (json.Length > 0)
                {
                    _urlStore = JsonSerializer.Deserialize<Dictionary<int, string>>(json) ?? new Dictionary<int, string>();
                    _currentKey = _urlStore.Keys.Any() ? _urlStore.Keys.Max() : 0;
                }
            }
        }

        public void Dispose()
        {
            string json = JsonSerializer.Serialize(_urlStore);
            File.WriteAllText(_path, json);
        }


        public string SaveLongUrl(string longUrl)
        {
            if (string.IsNullOrEmpty(longUrl))
                throw new Exception("Ссылка пустая!");

            _currentKey++;
            _urlStore[_currentKey] = longUrl;
 

            return _currentKey.ToString();
        }

        public (string, bool) GetLongUrl(string shortID)
        {
            if (int.TryParse(shortID, out int key) && _urlStore.ContainsKey(key))
            {
                return (_urlStore[key], true);
            }

            return (null, false);
        }

        public bool DeleteShortID(string shortID)
        {
            if (int.TryParse(shortID, out int key) && _urlStore.ContainsKey(key))
            {
                _urlStore.Remove(key);
                return true;
            }

            return false;
        }
    }
}