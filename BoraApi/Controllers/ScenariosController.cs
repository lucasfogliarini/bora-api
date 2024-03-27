using Bora.Scenarios;
using Microsoft.AspNetCore.Mvc;
using Bora.Entities;

namespace Bora.Api.Controllers
{
	[ApiController]
    [Route("[controller]")]
    public class ScenariosController(IRepository boraRepository, IScenarioService scenarioService) : ODataController<Scenario>(boraRepository)
    {
		[HttpPatch("{scenarioId}")]
        public async Task<IActionResult> UpdateAsync(int scenarioId, ScenarioInput scenarioInput)
        {
            await scenarioService.UpdateAsync(scenarioId, scenarioInput);
            return Ok();
        }
    }
}
