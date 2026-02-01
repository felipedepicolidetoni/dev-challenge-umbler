using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Desafio.Umbler.Interfaces;
using Desafio.Umbler.Validators;
using FluentValidation.Results;

namespace Desafio.Umbler.Controllers
{
    [Route("api")]
    public class DomainController : Controller
    {
        private readonly IDomainService _domainService;

        public DomainController(IDomainService domainService)
        {
            _domainService = domainService;
        }

        [HttpGet, Route("domain/{domainName}")]
        public async Task<IActionResult> Get(string domainName)
        {
            var request = new DomainRequest { DomainName = domainName };
            var validator = new DomainRequestValidator();
            ValidationResult result = validator.Validate(request);
            
            if (!result.IsValid)
            {
                var errorMsg = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errorMsg);
            }

            var dto = await _domainService.GetDomainInfoAsync(domainName);
            if (dto == null)
                return NotFound(new { error = "Domain not found or could not retrieve information." });

            return Ok(dto);
        }
    }
}
