using System.Text.Json.Serialization;

namespace RockPaperScissorsLizardSpockApi.Models
{
    public record RoundResult
    {
        public PlayerRoundResult Results { get; set; }
        public int Player { get; set; }
        public int Bot { get; set; }
    }
}
