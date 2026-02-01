namespace Desafio.Umbler.Dtos
{
    public record DomainInfoDto(
        string Domain,
        string[] NameServers,
        string IpAddress,
        string HostingCompany
    );
}