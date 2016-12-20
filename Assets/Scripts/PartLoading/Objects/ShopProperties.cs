namespace Assets.Scripts.PartLoading.Objects
{
    public class ShopProperties
    {
        public int Cost { get; private set; }
        public int RequiredLevel { get; private set; }

        public ShopProperties(int cost, int requiredLevel)
        {
            Cost = cost;
            RequiredLevel = requiredLevel;
        }
    }
}