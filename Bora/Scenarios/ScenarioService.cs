using Bora.Entities;
using Repository.AzureTables;
using System.ComponentModel.DataAnnotations;

namespace Bora.Scenarios
{
	public class ScenarioService : IScenarioService
    {
        private readonly IAzureTablesRepository _boraRepository;

        public ScenarioService(IAzureTablesRepository boraRepository)
        {
            _boraRepository = boraRepository;
        }
        public async Task UpdateAsync(int scenarioId, ScenarioInput scenarioInput)
        {
            var scenario = _boraRepository.FirstOrDefault<Scenario>(e=>e.Id == scenarioId);
            if (scenario == null)
            {
                throw new ValidationException("Não existe um cenário com esse id.");
            }
            else
            {
                if (scenarioInput.Title != null)
                    scenario.Title = scenarioInput.Title!;
                if (scenarioInput.Enabled.HasValue)
                    scenario.Enabled = scenarioInput.Enabled.Value;

                _boraRepository.Update(scenario);
                await _boraRepository.CommitAsync();
            }
        }
    }
}
