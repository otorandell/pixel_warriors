namespace PixelWarriors
{
    public readonly struct HitResult
    {
        public readonly bool Missed;
        public readonly bool Dodged;
        public readonly bool IsCrit;
        public readonly int Damage;

        public bool IsEffective => !Missed && !Dodged;

        public HitResult(bool missed, bool dodged, bool isCrit, int damage)
        {
            Missed = missed;
            Dodged = dodged;
            IsCrit = isCrit;
            Damage = damage;
        }

        public static HitResult Miss() => new HitResult(missed: true, dodged: false, isCrit: false, damage: 0);
        public static HitResult Dodge() => new HitResult(missed: false, dodged: true, isCrit: false, damage: 0);

        public static HitResult Hit(int damage, bool isCrit) =>
            new HitResult(missed: false, dodged: false, isCrit: isCrit, damage: damage);
    }
}
