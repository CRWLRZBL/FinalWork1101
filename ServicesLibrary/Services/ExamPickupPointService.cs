using ServicesLibrary.Models;
using System.Net.Http.Json;

namespace ServicesLibrary.Services
{

    public class ExamPickupPointService
    {
        private readonly HttpClient _client;
        private readonly string _baseAddress = "http://localhost:5007/api/";

        public ExamPickupPointService()
        {
            _client = new() { BaseAddress = new Uri(_baseAddress) };
        }

        public async Task<List<ExamPickupPoint>?> GetPickupPointsAsync()
        {
            try
            {
                return await _client.GetFromJsonAsync<List<ExamPickupPoint>>("ExamPickupPoints");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}