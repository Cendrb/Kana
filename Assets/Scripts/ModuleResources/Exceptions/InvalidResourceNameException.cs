using System;

namespace Assets.Scripts.ModuleResources.Exceptions
{
    public class InvalidResourceNameException : Exception
    {
        public ResourceLocation ResourceLocation { get; private set; }
        public string ReadName { get; private set; }

        public InvalidResourceNameException(ResourceLocation resourceLocation, string readName)
        {
            this.ResourceLocation = resourceLocation;
            this.ReadName = readName;
        }

        public override string ToString()
        {
            return string.Format(
                "The 'name' property needs to be the same as the filename. ({0} file != {1} read name)",
                this.ResourceLocation.Name, this.ReadName);
        }
    }
}