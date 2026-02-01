using System;
using System.Linq;
using System.Threading.Tasks;
using Desafio.Umbler.Dtos;
using Desafio.Umbler.Interfaces;
using Desafio.Umbler.Models;
using DnsClient;

namespace Desafio.Umbler.Services
{
    public class DomainService : IDomainService
    {
        private readonly IDomainRepository _repository;
        private readonly IWhoisClient _whoisClient;

        public DomainService(IDomainRepository repository, IWhoisClient whoisClient)
        {
            _repository = repository;
            _whoisClient = whoisClient;
        }

        public async Task<DomainInfoDto> GetDomainInfoAsync(string domainName)
        {
            if (string.IsNullOrWhiteSpace(domainName))
                throw new ArgumentNullException(nameof(domainName), "Domain name cannot be null or empty.");

            var domain = await _repository.GetByNameAsync(domainName);
            bool isNewDomain = domain == null;
            bool isExpired = !isNewDomain && DateTime.Now.Subtract(domain.UpdatedAt).TotalMinutes > domain.Ttl;

            if (isNewDomain)
            {
                domain = await BuildDomainEntityAsync(null, domainName);
                await _repository.AddAsync(domain);
                await _repository.SaveChangesAsync();
            }
            else if (isExpired)
            {
                domain = await BuildDomainEntityAsync(domain, domainName);
                _repository.Update(domain);
                await _repository.SaveChangesAsync();
            }

            var nameServers = await GetNameServersAsync(domainName);
            return new DomainInfoDto(
                domain.Name,
                nameServers,
                domain.Ip,
                domain.HostedAt
            );
        }

        private async Task<Domain> BuildDomainEntityAsync(Domain domain, string domainName)
        {
            var response = await _whoisClient.QueryAsync(domainName);
            var lookup = new LookupClient();
            var result = await lookup.QueryAsync(domainName, QueryType.ANY);
            var record = result.Answers.ARecords().FirstOrDefault();
            var address = record?.Address;
            var ip = address?.ToString();
            var hostResponse = !string.IsNullOrEmpty(ip) ? await _whoisClient.QueryAsync(ip) : null;

            domain ??= new Domain();

            domain.Name = domainName;
            domain.Ip = ip;
            domain.UpdatedAt = DateTime.Now;
            domain.WhoIs = response.Raw;
            domain.Ttl = record?.TimeToLive ?? 0;
            domain.HostedAt = hostResponse?.OrganizationName;
            return domain;
        }

        private static async Task<string[]> GetNameServersAsync(string domainName)
        {
            var lookup = new LookupClient();
            var result = await lookup.QueryAsync(domainName, QueryType.NS);
            return result.Answers.NsRecords().Select(ns => ns.NSDName.Value).ToArray();
        }
    }
}