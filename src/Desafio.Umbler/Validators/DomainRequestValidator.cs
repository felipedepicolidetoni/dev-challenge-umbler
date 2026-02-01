using FluentValidation;

namespace Desafio.Umbler.Validators
{
    public class DomainRequest
    {
        public string DomainName { get; set; }
    }

    public class DomainRequestValidator : AbstractValidator<DomainRequest>
    {
        public DomainRequestValidator()
        {
            RuleFor(x => x.DomainName)
                .NotEmpty().WithMessage("Domain name is required.")
                .Matches(@"^(?!-)[A-Za-z0-9-]{1,63}(?<!-)(\.[A-Za-z]{2,})+$")                
                .WithMessage("Invalid domain format. Example: umbler.com");
        }
    }
}