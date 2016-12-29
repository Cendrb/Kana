﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Scripts.Util.Resources;
using UnityEngine;

namespace Assets.Scripts.PartLoading
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
            localizations.AddRange(addLocalizations);
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
    }
}
