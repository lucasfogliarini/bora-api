using Bora.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Bora.Api.Controllers
{
	[ApiController]
    [Route("[controller]")]
    public class LocationsController(IRepository boraRepository) : ODataController<Location>(boraRepository)
    {
	}
}
