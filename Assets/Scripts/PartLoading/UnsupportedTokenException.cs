using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Assets.Scripts.PartLoading
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
