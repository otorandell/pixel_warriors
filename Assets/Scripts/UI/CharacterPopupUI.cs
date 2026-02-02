using System.Collections.Generic;
using UnityEngine;

namespace PixelWarriors
{
    public class CharacterPopupUI : PopupBase
    {
        public void ShowCharacterPopup(BattleCharacter character)
        {
            ClearContent();

            CharacterData data = character.Data;
            CharacterStats stats = character.EffectiveStats;

            // Header: "Name - Lv1 Warrior"
            string header = $"{data.Name} - Lv{data.Level} {data.Class}";
            Color classColor = UIFormatUtil.GetClassColor(data.Class);
            AddText(header, UIStyleConfig.FontSizeSmall, classColor, 0.88f, 1f);

            // Resources
            AddText($"HP: {character.CurrentHP}/{character.MaxHP}", UIStyleConfig.FontSizeTiny,
                UIStyleConfig.AccentRed, 0.78f, 0.86f);
            AddText($"EN: {character.CurrentEnergy}/{character.MaxEnergy}", UIStyleConfig.FontSizeTiny,
                UIStyleConfig.AccentYellow, 0.70f, 0.78f);
            AddText($"MP: {character.CurrentMana}/{character.MaxMana}", UIStyleConfig.FontSizeTiny,
                UIStyleConfig.AccentCyan, 0.62f, 0.70f);

            // Stats grid (3 rows)
            AddText($"END:{stats.Endurance}  STA:{stats.Stamina}  INT:{stats.Intellect}",
                UIStyleConfig.FontSizeTiny, UIStyleConfig.TextPrimary, 0.52f, 0.60f);
            AddText($"STR:{stats.Strength}  DEX:{stats.Dexterity}  WIL:{stats.Willpower}",
                UIStyleConfig.FontSizeTiny, UIStyleConfig.TextPrimary, 0.44f, 0.52f);
            AddText($"ARM:{stats.Armor}  MRS:{stats.MagicResist}  INI:{stats.Initiative}",
                UIStyleConfig.FontSizeTiny, UIStyleConfig.TextPrimary, 0.36f, 0.44f);

            // Position + actions
            string posText = $"{character.Row}-{character.Column}  Actions: {character.LongActionsRemaining}L {character.ShortActionsRemaining}S";
            AddText(posText, UIStyleConfig.FontSizeTiny, UIStyleConfig.TextDimmed, 0.28f, 0.36f);

            // Abilities list
            AddText("-- Abilities --", UIStyleConfig.FontSizeTiny, UIStyleConfig.AccentMagenta, 0.20f, 0.28f);

            List<AbilityData> abilities = data.Abilities.FindAll(a => !a.IsPassive);
            float yTop = 0.20f;
            float lineHeight = 0.06f;
            int maxAbilities = Mathf.Min(abilities.Count, 4);

            for (int i = 0; i < maxAbilities; i++)
            {
                AbilityData ability = abilities[i];
                string cost = UIFormatUtil.FormatAbilityCost(ability);
                string line = $"{ability.Name} {cost}";
                float yMax = yTop - i * lineHeight;
                float yMinLine = yMax - lineHeight;
                AddText(line, UIStyleConfig.FontSizeTiny, UIStyleConfig.TextDimmed, yMinLine, yMax);
            }

            Show();
        }
    }
}
