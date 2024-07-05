using Bora.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Bora.Api.Controllers
{
	[ApiController]
    [Route("[controller]")]
    public class ResponsibilitiesController(IRepository boraRepository) : ODataController<Responsibility>(boraRepository)
    {
	}
}
