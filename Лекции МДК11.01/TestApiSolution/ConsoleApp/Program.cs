using ServiceLayer;
using System.Net.Http.Json;

HttpClient client = new() { BaseAddress = new Uri("http://localhost:5018/api/v1") };

var films = client.GetFromJsonAsync<List<Film>>("films");
foreach (var item in films)
{
    Console.WriteLine($"{item.FilmId} {item.Name}");
}

int id = 1;
var film = await client.GetFromJsonAsync<Film>($"films/{id}");
Console.WriteLine();
Console.WriteLine($"{film.FilmId} {film.Name}");

id = 42;
await client.DeleteAsync($"films/{id}");