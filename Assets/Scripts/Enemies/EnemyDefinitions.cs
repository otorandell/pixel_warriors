namespace PixelWarriors
{
    public static class EnemyDefinitions
    {
        public static CharacterData CreateEnemy(EnemyType enemyType)
        {
            return enemyType switch
            {
                // === Act 1 ===
                EnemyType.Ratman => Act1Enemies.CreateRatman(),
                EnemyType.Skeleton => Act1Enemies.CreateSkeleton(),
                EnemyType.ZombieShambler => Act1Enemies.CreateZombieShambler(),
                EnemyType.FungusCreeper => Act1Enemies.CreateFungusCreeper(),
                EnemyType.GoblinArcher => Act1Enemies.CreateGoblinArcher(),
                EnemyType.SwarmBat => Act1Enemies.CreateSwarmBat(),
                EnemyType.TunnelRat => Act1Enemies.CreateTunnelRat(),
                EnemyType.Minotaur => Act1Enemies.CreateMinotaur(),
                EnemyType.GiantSpider => Act1Enemies.CreateGiantSpider(),
                EnemyType.BoneLord => Act1Enemies.CreateBoneLord(),
                EnemyType.GoblinKing => Act1Enemies.CreateGoblinKing(),
                EnemyType.CatacombGuardian => Act1Enemies.CreateCatacombGuardian(),

                // === Act 2 ===
                EnemyType.Spider => Act2Enemies.CreateSpider(),
                EnemyType.Bandit => Act2Enemies.CreateBandit(),
                EnemyType.OrcWarrior => Act2Enemies.CreateOrcWarrior(),
                EnemyType.StoneSentinel => Act2Enemies.CreateStoneSentinel(),
                EnemyType.BerserkerCultist => Act2Enemies.CreateBerserkerCultist(),
                EnemyType.DarkMage => Act2Enemies.CreateDarkMage(),
                EnemyType.HerbalistShaman => Act2Enemies.CreateHerbalistShaman(),
                EnemyType.CrossbowBandit => Act2Enemies.CreateCrossbowBandit(),
                EnemyType.FireImp => Act2Enemies.CreateFireImp(),
                EnemyType.OrcBrute => Act2Enemies.CreateOrcBrute(),
                EnemyType.WyvernKnight => Act2Enemies.CreateWyvernKnight(),
                EnemyType.NecromancerAdept => Act2Enemies.CreateNecromancerAdept(),
                EnemyType.BladeDancer => Act2Enemies.CreateBladeDancer(),
                EnemyType.MinotaurLord => Act2Enemies.CreateMinotaurLord(),
                EnemyType.BanditWarlord => Act2Enemies.CreateBanditWarlord(),

                // === Act 3 ===
                EnemyType.DarkKnight => Act3Enemies.CreateDarkKnight(),
                EnemyType.AbyssalGolem => Act3Enemies.CreateAbyssalGolem(),
                EnemyType.PlagueBringer => Act3Enemies.CreatePlagueBringer(),
                EnemyType.DeathCultist => Act3Enemies.CreateDeathCultist(),
                EnemyType.ChainDevil => Act3Enemies.CreateChainDevil(),
                EnemyType.ShadowAssassin => Act3Enemies.CreateShadowAssassin(),
                EnemyType.LichAcolyte => Act3Enemies.CreateLichAcolyte(),
                EnemyType.BloodMage => Act3Enemies.CreateBloodMage(),
                EnemyType.VoidSpeaker => Act3Enemies.CreateVoidSpeaker(),
                EnemyType.VampireLord => Act3Enemies.CreateVampireLord(),
                EnemyType.TwinWraith => Act3Enemies.CreateTwinWraith(),
                EnemyType.DemonChampion => Act3Enemies.CreateDemonChampion(),
                EnemyType.Lich => Act3Enemies.CreateLich(),
                EnemyType.ArchDemon => Act3Enemies.CreateArchDemon(),

                _ => Act1Enemies.CreateRatman()
            };
        }
    }
}
