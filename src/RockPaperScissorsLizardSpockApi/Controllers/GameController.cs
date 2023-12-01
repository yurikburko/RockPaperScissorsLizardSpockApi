using Microsoft.AspNetCore.Mvc;
using RockPaperScissorsLizardSpockApi.Models;
using RockPaperScissorsLizardSpockApi.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RockPaperScissorsLizardSpockApi.Controllers
{
    [Route("/")]
    [ApiController]
    public class GameController : ControllerBase
    {   
        private readonly IGameService _gameService;
        //TODO. Add logging if needed
        private readonly ILogger<GameController> _logger;

        public GameController(IGameService gameService, ILogger<GameController> logger) 
        {
            _gameService = gameService;
            _logger = logger;
        }

        // GET: /choices
        /// <summary>
        /// Returns all possible choices.
        /// </summary>
        [HttpGet]
        [Route("choices")]
        public IEnumerable<Choice> GetChoices()
        {
            return _gameService.GetChoices();
        }

        // GET /choice
        /// <summary>
        /// Returns a randomly generated choice.
        /// </summary>
        [HttpGet]
        [Route("choice")]
        public Task<Choice> GetChoice()
        {
            return _gameService.GetRandomChoice();
        }

        // POST /play
        /// <summary>
        /// Play a round against a bot.
        /// </summary>
        [HttpPost]
        [Route("play")]
        public Task<RoundResult> Post([FromBody] PlayerChoiceModel model)
        {
            return _gameService.PlayRoundAgainstBot(model.Player);
        }
    }
}
