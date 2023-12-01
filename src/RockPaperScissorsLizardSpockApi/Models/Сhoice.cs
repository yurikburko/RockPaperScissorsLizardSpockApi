namespace RockPaperScissorsLizardSpockApi.Models
{
    public record Choice
    {
        public int Id { get; set; }
        public ChoiceType Name { get; set; }
    }

    public enum ChoiceType
    {
        Rock = 1,
        Paper = 2,
        Scissors = 3,
        Lizard = 4,
        Spock = 5
    }
}
