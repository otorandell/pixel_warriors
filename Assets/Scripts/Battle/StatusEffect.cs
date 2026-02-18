namespace PixelWarriors
{
    public class StatusEffectInstance
    {
        public StatusEffect Type { get; private set; }
        public int RemainingTurns { get; set; }
        public int Value { get; set; }
        public BattleCharacter Source { get; private set; }

        public bool IsExpired => RemainingTurns == 0;

        public StatusEffectInstance(StatusEffect type, int duration, int value,
            BattleCharacter source = null)
        {
            Type = type;
            RemainingTurns = duration;
            Value = value;
            Source = source;
        }
    }
}
