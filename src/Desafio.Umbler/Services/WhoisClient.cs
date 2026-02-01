using System.Threading.Tasks;
using Whois.NET;
using Desafio.Umbler.Interfaces;

namespace Desafio.Umbler.Services
{
    public class WhoisClient : IWhoisClient
    {
        public async Task<WhoisResponse> QueryAsync(string domain)
        {
            return await Whois.NET.WhoisClient.QueryAsync(domain);
        }
    }
}
