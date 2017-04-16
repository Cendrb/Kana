using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Util;

namespace Assets.Scripts.ModuleResources.Localization
{
    public class LocalizationsLoader : ResourceLoader<List<Localization>>
    {
        private static readonly string TAG = "LocalizationsLoader";

        protected override List<Localization> LoadResource(ResourceLocation resourceLocation)
        {
            List<Localization> localizations = new List<Localization>();
            string path = resourceLocation.GetPath();
            Log.Info(TAG, "Loading " + resourceLocation.ToResourceLocationString() + " lang file...");
            int successfullyLoaded = 0;
            try
            {
                string[] data = File.ReadAllLines(path);
                int lineNumber = 0;
                foreach (string langLine in data)
                {
                    lineNumber++;
                    string[] nameValuePair = langLine.Split('=');
                    if (nameValuePair.Length == 2)
                    {
                        string[] moduleNamePair = nameValuePair[0].Split('.');
                        if (moduleNamePair.Length == 2)
                        {
                            localizations.Add(new Localization(moduleNamePair[0], moduleNamePair[1], nameValuePair[1]));
                            successfullyLoaded++;
                        }
                        else
                        {
                            logLineError(lineNumber, path, "Exactly one '.' is required (module.name=Localization)");
                            break;
                        }
                    }
                    else
                    {
                        logLineError(lineNumber, path, "Exactly one '=' is required (module.name=Localization)");
                        break;
                    }
                }
            }
            catch (IOException e)
            {
                Log.Exception(TAG, "Failed to read langfile at " + path, e);
            }
            Log.Info(TAG, "Successfully loaded " + successfullyLoaded + " translations from " + resourceLocation.ToResourceLocationString());
            return localizations;
        }


        private void logLineError(int lineNumber, string filePath, string reason)
        {
            Log.Warning(TAG, string.Format("Unable to read line {0} of {1} langfile: {2}", lineNumber, Path.GetFileName(filePath), reason));
        }
    }
}
