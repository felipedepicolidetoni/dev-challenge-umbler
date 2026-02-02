using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Desafio.Umbler.Front.Data;
using System.Text.RegularExpressions;

namespace Desafio.Umbler.Front.Services
{
    public class DomainService
    {
        private readonly IHttpClientFactory _clientFactory;
        public DomainService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<DomainInfoDto?> SearchDomainAsync(string domain)
        {
            if (!IsDomainValid(domain))
                throw new ArgumentException("Enter a valid domain (e.g., umbler.com)");
            if (domain.Contains("/") || domain.Contains("://"))
                throw new ArgumentException("Only the domain name is allowed (e.g., umbler.com)");

            var apiPath = $"/api/domain/{domain}";
            var http = _clientFactory.CreateClient("API");
            var response = await http.GetAsync(apiPath);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<DomainInfoDto>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                throw new Exception($"Error: {response.StatusCode}");
            }
        }

        public static bool IsDomainValid(string dom)
        {
            if (string.IsNullOrWhiteSpace(dom)) return false;
            return Regex.IsMatch(dom, @"^(?!-)[A-Za-z0-9-]{1,63}(?<!-)(\.[A-Za-z]{2,})+$");
        }
    }
}
