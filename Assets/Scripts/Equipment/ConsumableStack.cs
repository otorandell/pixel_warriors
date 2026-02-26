using System;

namespace PixelWarriors
{
    [Serializable]
    public class ConsumableStack
    {
        public string ConsumableId;
        public int Quantity;

        public ConsumableStack() { }

        public ConsumableStack(string consumableId, int quantity)
        {
            ConsumableId = consumableId;
            Quantity = quantity;
        }
    }
}
