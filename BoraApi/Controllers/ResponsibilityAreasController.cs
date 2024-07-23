using Bora.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Bora.Api.Controllers
{
	[ApiController]
    [Route("[controller]")]
    public class ResponsibilityAreasController(IRepository boraRepository) : ODataController<ResponsibilityArea>(boraRepository)
    {
	}
}
