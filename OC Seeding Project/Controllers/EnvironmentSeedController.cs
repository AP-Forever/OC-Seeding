using Microsoft.AspNetCore.Mvc;
using OrderCloud.Catalyst;
using OC_Seeding_Project.API.Commands;
using OC_Seeding_Project.EnvironmentSeed;

namespace OC_Seeding_Project.Controllers
{
    public class EnvironmentSeedController : CatalystController
    {
        private readonly ISeedCommand _command;

        public EnvironmentSeedController(ISeedCommand command)
        {
            _command = command;
        }

        [HttpPost, Route("seed")]
        public async Task<EnvironmentSeedResponse> Seed([FromBody] EnvironmentSeedRequestModel seed)
        {
            return await _command.Seed(seed);
        }

        [HttpPost, Route("test")]
        public string test()
        {
            return "test";
        }
    }
}
