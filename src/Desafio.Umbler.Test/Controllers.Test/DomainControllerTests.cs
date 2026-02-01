using Desafio.Umbler.Controllers;
using Desafio.Umbler.Dtos;
using Desafio.Umbler.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Desafio.Umbler.Test.Controllers.Test
{
	[TestClass]
	public class DomainControllerTests
	{
		[TestMethod]
		public async Task Get_Returns_Ok_When_Domain_Found()
		{
			var mockService = new Mock<IDomainService>();
			var dto = new DomainInfoDto("test.com", new[] { "ns1.test.com" }, "1.2.3.4", "TestHost");
			mockService.Setup(s => s.GetDomainInfoAsync("test.com")).ReturnsAsync(dto);
			var controller = new DomainController(mockService.Object);
			var result = await controller.Get("test.com");
			var okResult = result as OkObjectResult;
			Assert.IsNotNull(okResult);
			Assert.IsInstanceOfType(okResult.Value, typeof(DomainInfoDto));
			var returnedDto = okResult.Value as DomainInfoDto;
			Assert.AreEqual(dto.Domain, returnedDto.Domain);
		}

		[TestMethod]
		public async Task Get_Returns_NotFound_When_Domain_NotFound()
		{
			var mockService = new Mock<IDomainService>();
			mockService.Setup(s => s.GetDomainInfoAsync("notfound.com")).ReturnsAsync((DomainInfoDto)null);
			var controller = new DomainController(mockService.Object);
			var result = await controller.Get("notfound.com");
			Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
		}

		[TestMethod]
		public async Task Get_Throws_ArgumentException_On_Invalid_Domain()
		{
			var mockService = new Mock<IDomainService>();
			var controller = new DomainController(mockService.Object);
			await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
			{
				await controller.Get("");
			});
		}
	}
}