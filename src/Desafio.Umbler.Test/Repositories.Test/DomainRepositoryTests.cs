using System;
using System.Threading.Tasks;
using Desafio.Umbler.Models;
using Desafio.Umbler.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Desafio.Umbler.Test.Repositories.Test
{
    [TestClass]
    public class DomainRepositoryTests
    {
        private DatabaseContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new DatabaseContext(options);
        }

        [TestMethod]
        public async Task AddAsync_Adds_Domain_To_Db()
        {
            var db = GetInMemoryDbContext();
            var repo = new DomainRepository(db);
            var domain = new Domain { Name = "add.com", Ip = "1.1.1.1", UpdatedAt = DateTime.Now, HostedAt = "host", Ttl = 60, WhoIs = "whois" };
            await repo.AddAsync(domain);
            await repo.SaveChangesAsync();
            var exists = await db.Domains.AnyAsync(d => d.Name == "add.com");
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public async Task GetByNameAsync_Returns_Domain_If_Exists()
        {
            var db = GetInMemoryDbContext();
            db.Domains.Add(new Domain { Name = "get.com", Ip = "2.2.2.2", UpdatedAt = DateTime.Now, HostedAt = "host", Ttl = 60, WhoIs = "whois" });
            db.SaveChanges();
            var repo = new DomainRepository(db);
            var found = await repo.GetByNameAsync("get.com");
            Assert.IsNotNull(found);
            Assert.AreEqual("get.com", found.Name);
        }

        [TestMethod]
        public async Task GetByNameAsync_Returns_Null_If_Not_Exists()
        {
            var db = GetInMemoryDbContext();
            var repo = new DomainRepository(db);
            var found = await repo.GetByNameAsync("notfound.com");
            Assert.IsNull(found);
        }

        [TestMethod]
        public async Task Update_Updates_Domain()
        {
            var db = GetInMemoryDbContext();
            db.Domains.Add(new Domain { Name = "update.com", Ip = "3.3.3.3", UpdatedAt = DateTime.Now, HostedAt = "host", Ttl = 60, WhoIs = "whois" });
            db.SaveChanges();
            var repo = new DomainRepository(db);
            var domain = await db.Domains.SingleAsync(d => d.Name == "update.com");
            domain.Ip = "9.9.9.9";
            repo.Update(domain);
            await repo.SaveChangesAsync();
            var found = await db.Domains.SingleOrDefaultAsync(d => d.Name == "update.com");
            Assert.AreEqual("9.9.9.9", found.Ip);
        }

        [TestMethod]
        public async Task SaveChangesAsync_Persists_Changes()
        {
            var db = GetInMemoryDbContext();
            var repo = new DomainRepository(db);
            var domain = new Domain { Name = "save.com", Ip = "4.4.4.4", UpdatedAt = DateTime.Now, HostedAt = "host", Ttl = 60, WhoIs = "whois" };
            await repo.AddAsync(domain);
            // Not saved yet
            Assert.AreEqual(0, await db.Domains.CountAsync());
            await repo.SaveChangesAsync();
            Assert.AreEqual(1, await db.Domains.CountAsync());
        }
    }
}
