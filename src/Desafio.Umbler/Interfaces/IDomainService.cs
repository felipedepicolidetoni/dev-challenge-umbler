using System.Threading.Tasks;
using Desafio.Umbler.Dtos;

namespace Desafio.Umbler.Interfaces
{
    public interface IDomainService
    {
        Task<DomainInfoDto> GetDomainInfoAsync(string domainName);
    }
}