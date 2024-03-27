using Bora.Entities;
using System.ComponentModel.DataAnnotations;

namespace Bora.Scenarios
{
	public class ScenarioService : IScenarioService
    {
        private readonly IRepository _boraRepository;

        public ScenarioService(IRepository boraRepository)
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
