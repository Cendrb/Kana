using System;
using Newtonsoft.Json.Linq;

namespace Assets.Scripts.ModuleResources.Exceptions
{
    class UnsupportedTokenException : Exception
    {
        public JTokenType Type { get; private set; }

        public UnsupportedTokenException(JTokenType type)
        {
            Type = type;
        }

        public override string ToString()
        {
            return string.Format("{0} is not a supported type for properties parsing");
        }
    }
}
