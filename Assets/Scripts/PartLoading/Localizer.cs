using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.PartLoading
{
    class Localizer
    {
        private static readonly string TAG = "LangTranslator";

        private List<Localization> localizations = new List<Localization>();

        public Localizer()
        {

        }

        public void AddLang(string path)
        {
            Log.Info(TAG, "Loading " + path + " lang file...");
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
            Log.Info(TAG, "Successfully loaded " + successfullyLoaded + " translations");
        }

        public string GetLocalizedName(string module, string name)
        {
            Localization localization =
                localizations.Find(l => l.Module == module && l.Name == name);
            if (localization != null)
                return localization.LocalizedName;
            else
            {
                Log.Warning(TAG, "No localization found for " + name);
                return name;
            }
        }

        private void logLineError(int lineNumber, string filePath, string reason)
        {
            Log.Warning(TAG, string.Format("Unable to read line {0} of {1} langfile: {2}", lineNumber, Path.GetFileName(filePath), reason));
        }
    }
}
