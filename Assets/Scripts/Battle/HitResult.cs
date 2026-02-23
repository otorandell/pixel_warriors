namespace PixelWarriors
{
    public readonly struct HitResult
    {
        public readonly bool Missed;
        public readonly bool Dodged;
        public readonly bool Blocked;
        public readonly bool IsCrit;
        public readonly int Damage;

        public bool IsEffective => !Missed && !Dodged && !Blocked;

        public HitResult(bool missed, bool dodged, bool blocked, bool isCrit, int damage)
        {
            Missed = missed;
            Dodged = dodged;
            Blocked = blocked;
            IsCrit = isCrit;
            Damage = damage;
        }

        public static HitResult Miss() => new HitResult(missed: true, dodged: false, blocked: false, isCrit: false, damage: 0);
        public static HitResult Dodge() => new HitResult(missed: false, dodged: true, blocked: false, isCrit: false, damage: 0);
        public static HitResult Block() => new HitResult(missed: false, dodged: false, blocked: true, isCrit: false, damage: 0);

        public static HitResult Hit(int damage, bool isCrit) =>
            new HitResult(missed: false, dodged: false, blocked: false, isCrit: isCrit, damage: damage);
    }
}
