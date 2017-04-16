using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.ModuleResources
{
    public abstract class ResourceLoader<TLoadedResource>
    {
        private Dictionary<string, TLoadedResource> loadedResources = new Dictionary<string, TLoadedResource>();

        public TLoadedResource LoadResourceAndCache(ResourceLocation resourceLocation)
        {
            TLoadedResource resource;
            if (this.loadedResources.TryGetValue(resourceLocation.ToResourceLocationString(), out resource))
            {
                return resource;
            }
            else
            {
                //resourceLocation.Verify(); - all ResourceLocations should already be verified
                resource = LoadResource(resourceLocation);
                this.loadedResources.Add(resourceLocation.ToResourceLocationString(), resource);
                return resource;
            }
        }

        protected abstract TLoadedResource LoadResource(ResourceLocation resourceLocation);

        public List<TLoadedResource> GetAllResources()
        {
            return this.loadedResources.Values.ToList();
        }
    }
}
