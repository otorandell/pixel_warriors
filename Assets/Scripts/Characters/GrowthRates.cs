namespace PixelWarriors
{
    public static class GrowthRates
    {
        // Per-class growth rates (0-100). Higher = more likely to gain +1 on level up.
        // Order: END, STA, INT, STR, DEX, WIL, ARM, MRS, INI

        public static CharacterStats GetClassGrowth(CharacterClass characterClass)
        {
            return characterClass switch
            {
                //                                    END  STA  INT  STR  DEX  WIL  ARM  MRS  INI
                CharacterClass.Warrior      => new CharacterStats(60,  40,  10,  55,  30,  20,  45,  20,  25),
                CharacterClass.Rogue        => new CharacterStats(25,  55,  20,  40,  60,  20,  20,  25,  50),
                CharacterClass.Ranger       => new CharacterStats(35,  50,  15,  35,  55,  25,  25,  20,  45),
                CharacterClass.Priest       => new CharacterStats(50,  30,  40,  30,  20,  50,  35,  35,  20),
                CharacterClass.Elementalist => new CharacterStats(20,  15,  65,  15,  30,  60,  10,  40,  30),
                CharacterClass.Warlock      => new CharacterStats(35,  25,  55,  20,  20,  55,  15,  45,  25),
                _ => new CharacterStats(30, 30, 30, 30, 30, 30, 30, 30, 30)
            };
        }
    }
}
