using System;

namespace PixelWarriors
{
    public class EventChoice
    {
        public string Label;
        public string Description;          // Effect preview shown under the label, e.g. "Heal 25% HP"
        public string ConditionLabel;       // e.g. "Requires Priest" (null = no condition)

        public Func<RunData, bool> Condition;               // null = always available
        public Func<RunData, string> OutcomeDescription;    // called after Apply
        public Action<RunData, int> Apply;                  // int = selected party index (-1 if no pick)

        public bool NeedsCharacterPick;
        public string CharacterPickPrompt;  // e.g. "Who drinks from the altar?"
    }
}
