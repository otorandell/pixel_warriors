namespace PixelWarriors
{
    public class ConsumableStack
    {
        public string ConsumableId;
        public int Quantity;

        public ConsumableStack(string consumableId, int quantity)
        {
            ConsumableId = consumableId;
            Quantity = quantity;
        }
    }
}
