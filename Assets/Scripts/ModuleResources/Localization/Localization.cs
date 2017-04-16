namespace Assets.Scripts.ModuleResources.Localization
{
    public class Localization
    {
        public string Module { get; private set; }
        public string Name { get; private set; }
        public string LocalizedName { get; private set; }

        public Localization(string module, string name, string localizedName)
        {
            this.Module = module;
            this.Name = name;
            this.LocalizedName = localizedName;
        }
    }
}
