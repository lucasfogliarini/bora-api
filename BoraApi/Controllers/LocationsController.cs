using Bora.Database;
using Bora.Database.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Bora.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocationsController : ODataController<Location>
    {
        public LocationsController(IBoraDatabase boraDatabase) : base(boraDatabase)
        {
        }
    }
}
