using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace ServiceLayer
{
    public class FilmService
    {
        private readonly HttpClient _client;
        private readonly string _baseAddress = "http://localhost:5018/api/v1";

        public FilmService()
        {
            _client = new() { BaseAddress = new Uri(_baseAddress) };
        }

        public async Task<IEnumerable<Film>?> GetFilmsAsync()
        {
            return await _client.GetFromJsonAsync<IEnumerable<Film>>("films");
        }

        public async Task<Film?> GetFilmAsync()
        {
            return await _client.GetFromJsonAsync<Film>($"films/{id}");
        }
    }
}
