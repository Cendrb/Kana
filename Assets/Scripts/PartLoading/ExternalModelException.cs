using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.PartLoading
{
    class ExternalModelException : Exception
    {
        public string ReferencePath { get; private set; }
        public bool IsReferenceFormatValid { get; private set; }

        public ExternalModelException(string referencePath, bool isStringFormatValid)
        {
            ReferencePath = referencePath;
            IsReferenceFormatValid = isStringFormatValid;
        }

        public override string ToString()
        {
            if (IsReferenceFormatValid)
                return string.Format("Failed to find a submodel with reference path {0}", ReferencePath);
            else
                return string.Format("Reference path {0} is not valid. Use module.model_name syntax.", ReferencePath);
        }
    }
}
