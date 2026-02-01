using System.Threading.Tasks;
using Desafio.Umbler.Interfaces;
using Desafio.Umbler.Models;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Umbler.Repositories
{
    public class DomainRepository : IDomainRepository
    {
        private readonly DatabaseContext _db;
        public DomainRepository(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<Domain> GetByNameAsync(string domainName)
        {
            return await _db.Domains.SingleOrDefaultAsync(d => d.Name == domainName);
        }

        public async Task AddAsync(Domain domain)
        {
            await _db.Domains.AddAsync(domain);
        }

        public void Update(Domain domain)
        {
            _db.Domains.Update(domain);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
