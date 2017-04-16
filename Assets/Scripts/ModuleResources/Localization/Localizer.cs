using System.Collections.Generic;
using Assets.Scripts.Util;

namespace Assets.Scripts.ModuleResources.Localization
{
    public class Localizer
    {
        private static readonly string TAG = "Localizer";

        private readonly List<Localization> localizations = new List<Localization>();

        public Localizer()
        {

        }

        public void AddLang(IEnumerable<Localization> addLocalizations)
        {
            this.localizations.AddRange(addLocalizations);
        }

        public string GetLocalizedName(string module, string name)
        {
            Localization localization =
                this.localizations.Find(l => l.Module == module && l.Name == name);
            if (localization != null)
            {
                return localization.LocalizedName;
            }
            else
            {
                Log.Warning(TAG, "No localization found for " + name);
                return name;
            }
        }
    }
}
