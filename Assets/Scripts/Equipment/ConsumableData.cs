namespace PixelWarriors
{
    public class ConsumableData
    {
        public string Id;
        public string Name;
        public string Description;
        public ConsumableCategory Category;
        public int BuyPrice;
        public int MinAct;
        public bool UsableInBattle;
        public bool UsableOutOfBattle;

        // For scrolls: class + ability name to look up and execute
        public CharacterClass? ScrollAbilityClass;
        public string ScrollAbilityName;

        // For books: class + ability name to teach permanently
        public CharacterClass? BookAbilityClass;
        public string BookAbilityName;
    }
}
