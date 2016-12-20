using System;

namespace Assets.Scripts.PartLoading.Exceptions
{
    class ScriptNotFoundException : Exception
    {
        public string Module { get; private set; }
        public string ScriptName { get; private set; }

        public ScriptNotFoundException(string module, string scriptName)
        {
            Module = module;
            ScriptName = scriptName;
        }

        public override string ToString()
        {
            return string.Format("Script class {0} was not found in module {1} (needs to extend Part)", ScriptName, Module);
        }
    }
}
