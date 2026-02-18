using System.Collections.Generic;

namespace PixelWarriors
{
    public class BattleCharacter
    {
        public CharacterData Data { get; private set; }
        public TeamSide Side { get; private set; }
        public GridRow Row { get; set; }
        public GridColumn Column { get; set; }

        public CharacterStats EffectiveStats { get; private set; }
        public int MaxHP { get; private set; }
        public int CurrentHP { get; set; }
        public int MaxEnergy { get; private set; }
        public int CurrentEnergy { get; set; }
        public int MaxMana { get; private set; }
        public int CurrentMana { get; set; }

        public int LongActionsRemaining { get; set; }
        public int ShortActionsRemaining { get; set; }

        public Priority Priority { get; set; }
        public bool IsAlive => CurrentHP > 0;

        public List<StatusEffectInstance> StatusEffects { get; private set; } = new();
        public Element LastSpellElement { get; set; } = Element.Arcane;

        public BattleCharacter(CharacterData data, TeamSide side, GridRow row, GridColumn column)
        {
            Data = data;
            Side = side;
            Row = row;
            Column = column;
            Priority = Priority.Normal;

            RecalculateStats();
            CurrentHP = MaxHP;
            CurrentEnergy = MaxEnergy;
            CurrentMana = MaxMana;
        }

        public bool HasEffect(StatusEffect type)
        {
            for (int i = 0; i < StatusEffects.Count; i++)
                if (StatusEffects[i].Type == type) return true;
            return false;
        }

        public StatusEffectInstance GetEffect(StatusEffect type)
        {
            for (int i = 0; i < StatusEffects.Count; i++)
                if (StatusEffects[i].Type == type) return StatusEffects[i];
            return null;
        }

        public void AddEffect(StatusEffectInstance effect)
        {
            // Replace existing effect of same type
            for (int i = 0; i < StatusEffects.Count; i++)
            {
                if (StatusEffects[i].Type == effect.Type)
                {
                    StatusEffects[i] = effect;
                    return;
                }
            }
            StatusEffects.Add(effect);
        }

        public void RemoveEffect(StatusEffect type)
        {
            for (int i = StatusEffects.Count - 1; i >= 0; i--)
            {
                if (StatusEffects[i].Type == type)
                {
                    StatusEffects.RemoveAt(i);
                    return;
                }
            }
        }

        public void RecalculateStats()
        {
            EffectiveStats = Data.GetTotalStats();
            MaxHP = StatCalculator.CalculateMaxHP(EffectiveStats);
            MaxEnergy = StatCalculator.CalculateMaxEnergy(EffectiveStats);
            MaxMana = StatCalculator.CalculateMaxMana(EffectiveStats);
        }

        public void StartTurn()
        {
            LongActionsRemaining = GameplayConfig.LongActionPoints;
            ShortActionsRemaining = GameplayConfig.ShortActionPoints;

            int energyRegen = StatCalculator.CalculateEnergyRegen(MaxEnergy);
            CurrentEnergy = UnityEngine.Mathf.Min(CurrentEnergy + energyRegen, MaxEnergy);

            int manaRegen = StatCalculator.CalculateManaRegen(MaxMana);
            CurrentMana = UnityEngine.Mathf.Min(CurrentMana + manaRegen, MaxMana);
        }

        public bool CanUseAbility(AbilityData ability)
        {
            if (ability.ActionCost == ActionPointType.Long && LongActionsRemaining < ability.LongPointCost)
                return false;

            if (ability.ActionCost == ActionPointType.Short)
            {
                int availableShort = ShortActionsRemaining + LongActionsRemaining;
                if (availableShort < ability.ShortPointCost)
                    return false;
            }

            if (ability.EnergyCost > CurrentEnergy)
                return false;

            if (ability.ManaCost > CurrentMana)
                return false;

            if (ability.HPCost > 0 && ability.HPCost >= CurrentHP)
                return false;

            return true;
        }

        public void ConsumeAbilityCost(AbilityData ability)
        {
            if (ability.ActionCost == ActionPointType.Long)
            {
                LongActionsRemaining -= ability.LongPointCost;
            }
            else
            {
                if (ShortActionsRemaining >= ability.ShortPointCost)
                {
                    ShortActionsRemaining -= ability.ShortPointCost;
                }
                else
                {
                    int remaining = ability.ShortPointCost - ShortActionsRemaining;
                    ShortActionsRemaining = 0;
                    LongActionsRemaining -= remaining;
                }
            }

            CurrentEnergy -= ability.EnergyCost;
            CurrentMana -= ability.ManaCost;
            CurrentHP -= ability.HPCost;
        }

        public bool HasActionsRemaining()
        {
            return LongActionsRemaining > 0 || ShortActionsRemaining > 0;
        }
    }
}
