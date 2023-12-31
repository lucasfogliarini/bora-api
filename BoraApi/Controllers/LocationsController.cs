using Bora.Entities;
using Microsoft.AspNetCore.Mvc;
using Repository.AzureTables;

namespace Bora.Api.Controllers
{
	[ApiController]
    [Route("[controller]")]
    public class LocationsController : ODataController<Location>
    {
        public LocationsController(IAzureTablesRepository boraRepository) : base(boraRepository)
        {
        }
    }
}
