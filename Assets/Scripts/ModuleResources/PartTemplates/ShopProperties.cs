namespace Assets.Scripts.ModuleResources.PartTemplates
{
    public class ShopProperties
    {
        public int Cost { get; private set; }
        public int RequiredLevel { get; private set; }
        public string Category { get; private set; }

        public ShopProperties(int cost, int requiredLevel, string category)
        {
            this.Cost = cost;
            this.RequiredLevel = requiredLevel;
            this.Category = category;
        }
    }
}