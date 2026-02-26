using System.Collections.Generic;

namespace PixelWarriors
{
    public class EventData
    {
        public string Id;
        public int MinAct;          // 0 = any act, 1+ = gated
        public bool IsUnique;       // Only shown once per run (tracked in RunData.SeenEvents)

        public string Title;
        public string Narrative;    // Multi-line flavour text

        public List<EventChoice> Choices;
    }
}
