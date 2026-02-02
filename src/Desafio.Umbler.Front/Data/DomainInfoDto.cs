namespace Desafio.Umbler.Front.Data
{
    public record DomainInfoDto(
        string Domain,
        string[] NameServers,
        string IpAddress,
        string HostingCompany
    );
}