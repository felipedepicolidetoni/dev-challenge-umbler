using System;
using System.Threading.Tasks;
using Desafio.Umbler.Models;

namespace Desafio.Umbler.Interfaces
{
    public interface IDomainRepository
    {
        Task<Domain> GetByNameAsync(string domainName);
        Task AddAsync(Domain domain);
        void Update(Domain domain);
        Task SaveChangesAsync();
    }
}
