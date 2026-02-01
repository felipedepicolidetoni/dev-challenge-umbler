using System;
using System.Threading.Tasks;
using Desafio.Umbler.Dtos;
using Desafio.Umbler.Interfaces;
using Desafio.Umbler.Models;
using Desafio.Umbler.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Desafio.Umbler.Test.Services.Test
{
    [TestClass]
    public class DomainServiceTests
    {
        [TestMethod]
        public async Task GetDomainInfoAsync_Returns_Dto_When_Found_And_Not_Expired()
        {
            var repo = new Mock<IDomainRepository>();
            var whois = new Mock<IWhoisClient>();
            var now = DateTime.Now;
            var domain = new Domain { Name = "test.com", Ip = "1.2.3.4", UpdatedAt = now, HostedAt = "host", Ttl = 60, WhoIs = "whois" };
            repo.Setup(r => r.GetByNameAsync("test.com")).ReturnsAsync(domain);
            repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            whois.Setup(w => w.QueryAsync(It.IsAny<string>())).ReturnsAsync(new Whois.NET.WhoisResponse());
            var service = new DomainService(repo.Object, whois.Object);
            var result = await service.GetDomainInfoAsync("test.com");
            Assert.IsNotNull(result);
            Assert.AreEqual("test.com", result.Domain);
        }

        [TestMethod]
        public async Task GetDomainInfoAsync_Creates_New_Domain_When_Not_Exists()
        {
            var repo = new Mock<IDomainRepository>();
            var whois = new Mock<IWhoisClient>();
            repo.Setup(r => r.GetByNameAsync("new.com")).ReturnsAsync((Domain)null);
            repo.Setup(r => r.AddAsync(It.IsAny<Domain>())).Returns(Task.CompletedTask);
            repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            whois.Setup(w => w.QueryAsync(It.IsAny<string>())).ReturnsAsync(new Whois.NET.WhoisResponse());
            var service = new DomainService(repo.Object, whois.Object);
            var result = await service.GetDomainInfoAsync("new.com");
            Assert.IsNotNull(result);
            Assert.AreEqual("new.com", result.Domain);
            repo.Verify(r => r.AddAsync(It.IsAny<Domain>()), Times.Once);
        }

        [TestMethod]
        public async Task GetDomainInfoAsync_Updates_Domain_When_Expired()
        {
            var repo = new Mock<IDomainRepository>();
            var whois = new Mock<IWhoisClient>();
            var expired = DateTime.Now.AddMinutes(-120);
            var domain = new Domain { Name = "expired.com", Ip = "1.2.3.4", UpdatedAt = expired, HostedAt = "host", Ttl = 60, WhoIs = "whois" };
            repo.Setup(r => r.GetByNameAsync("expired.com")).ReturnsAsync(domain);
            repo.Setup(r => r.Update(It.IsAny<Domain>()));
            repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            whois.Setup(w => w.QueryAsync(It.IsAny<string>())).ReturnsAsync(new Whois.NET.WhoisResponse());
            var service = new DomainService(repo.Object, whois.Object);
            var result = await service.GetDomainInfoAsync("expired.com");
            Assert.IsNotNull(result);
            Assert.AreEqual("expired.com", result.Domain);
            repo.Verify(r => r.Update(It.IsAny<Domain>()), Times.Once);
        }

        [TestMethod]
        public async Task GetDomainInfoAsync_Throws_On_NullOrEmpty()
        {
            var repo = new Mock<IDomainRepository>();
            var whois = new Mock<IWhoisClient>();
            var service = new DomainService(repo.Object, whois.Object);
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
            {
                await service.GetDomainInfoAsync("");
            });
        }

        [TestMethod]
        public async Task GetDomainInfoAsync_Throws_On_Repository_Error()
        {
            var repo = new Mock<IDomainRepository>();
            var whois = new Mock<IWhoisClient>();
            repo.Setup(r => r.GetByNameAsync(It.IsAny<string>())).ThrowsAsync(new Exception("DB error"));
            var service = new DomainService(repo.Object, whois.Object);
            await Assert.ThrowsExceptionAsync<Exception>(async () =>
            {
                await service.GetDomainInfoAsync("fail.com");
            });
        }

        [TestMethod]
        public async Task Domain_Moking_WhoisClient()
        {
            var repo = new Mock<IDomainRepository>();
            var whoisClient = new Mock<IWhoisClient>();
            var domainName = "test.com";
            whoisClient.Setup(w => w.QueryAsync(domainName)).ReturnsAsync(new Whois.NET.WhoisResponse());
            repo.Setup(r => r.GetByNameAsync(domainName)).ReturnsAsync((Domain)null);
            repo.Setup(r => r.AddAsync(It.IsAny<Domain>())).Returns(Task.CompletedTask);
            repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            var service = new DomainService(repo.Object, whoisClient.Object);
            var result = await service.GetDomainInfoAsync(domainName);
            Assert.IsNotNull(result);
            Assert.AreEqual(domainName, result.Domain);
            whoisClient.Verify(w => w.QueryAsync(domainName), Times.AtLeastOnce);
        }
    }
}
