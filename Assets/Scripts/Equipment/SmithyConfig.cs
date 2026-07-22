namespace PixelWarriors
{
    public static class SmithyConfig
    {
        // Upgrade costs per level (multiplied by act)
        public const int BaseUpgradeMetalCost = 4;
        public const int BaseUpgradeLeatherCost = 2;
        public const int MaxUpgradeLevel = 3;

        // Stat bonus per upgrade level
        public const int UpgradeStatBonus = 1;       // +1 to primary stats per upgrade
        public const int UpgradeDamageBonus = 1;      // +1 weapon damage per upgrade
        public const float UpgradeBlockBonus = 0.03f;  // +3% block per upgrade

        // Dismantle yields
        public const int DismantleMetalBase = 2;   // Weapons, heavy armor
        public const int DismantleLeatherBase = 2;  // Light armor, trinkets
        public const float DismantleActBonus = 0.5f; // +50% per act level

        // Craft costs
        public const int CraftMetalCost = 5;
        public const int CraftLeatherCost = 3;
        public const int CraftOptions = 3; // Number of craft options offered

        // Gem imbue cost (gold)
        public const int ImbueGoldCost = 20; // Per act multiplier applied

        // Material drop chances from battle
        public const float MaterialDropChance = 0.45f;
        public const int MaterialDropMin = 2;
        public const int MaterialDropMax = 3;
        public const float GemDropChance = 0.08f; // From elite/boss only

        // Gem names
        public static string GetGemName(GemType gem)
        {
            return gem switch
            {
                GemType.Ruby => "Ruby",
                GemType.Sapphire => "Sapphire",
                GemType.Diamond => "Diamond",
                GemType.Emerald => "Emerald",
                GemType.Onyx => "Onyx",
                _ => "Gem"
            };
        }

        // Gem weapon effects (description)
        public static string GetGemWeaponEffect(GemType gem)
        {
            return gem switch
            {
                GemType.Ruby => "+Burn on hit (3 turns)",
                GemType.Sapphire => "+2 Mana regen/turn",
                GemType.Diamond => "+5% crit chance",
                GemType.Emerald => "+Poison on hit",
                GemType.Onyx => "+3 bonus damage",
                _ => ""
            };
        }

        // Gem armor effects (description)
        public static string GetGemArmorEffect(GemType gem)
        {
            return gem switch
            {
                GemType.Ruby => "+3 Armor",
                GemType.Sapphire => "+4 Magic Resist",
                GemType.Diamond => "+3 ARM +2 MRS",
                GemType.Emerald => "+2 Endurance",
                GemType.Onyx => "+2 Initiative",
                _ => ""
            };
        }

        public static bool IsWeaponSlot(EquipmentSlot slot)
        {
            return slot == EquipmentSlot.Hand1;
        }

        public static int GetUpgradeCost(int currentLevel, int act, bool isMetal)
        {
            int baseCost = isMetal ? BaseUpgradeMetalCost : BaseUpgradeLeatherCost;
            return baseCost * (currentLevel + 1) * act;
        }

        public static int GetDismantleYield(EquipmentData item, bool isMetal)
        {
            int baseYield = isMetal ? DismantleMetalBase : DismantleLeatherBase;
            float actBonus = 1f + (item.ActLevel - 1) * DismantleActBonus;
            int upgradeBonus = item.UpgradeLevel;
            return UnityEngine.Mathf.RoundToInt((baseYield + upgradeBonus) * actBonus);
        }

        public static bool YieldsMainlyMetal(EquipmentSlot slot)
        {
            return slot == EquipmentSlot.Hand1 || slot == EquipmentSlot.Offhand
                || slot == EquipmentSlot.Head || slot == EquipmentSlot.Body;
        }
    }
}
