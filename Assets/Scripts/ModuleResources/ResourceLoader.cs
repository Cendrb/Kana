using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.ModuleResources
{
    public abstract class ResourceLoader<TLoadedResource>
    {
        private Dictionary<string, TLoadedResource> loadedResources = new Dictionary<string, TLoadedResource>();

        public TLoadedResource LoadResource(ResourceLocation resourceLocation)
        {
            TLoadedResource resource;
            if (loadedResources.TryGetValue(resourceLocation.ToResourceLocationString(), out resource))
            {
                return resource;
            }
            else
            {
                //resourceLocation.Verify(); - all ResourceLocations should already be verified
                resource = loadResource(resourceLocation);
                loadedResources.Add(resourceLocation.ToResourceLocationString(), resource);
                return resource;
            }
        }

        protected abstract TLoadedResource loadResource(ResourceLocation resourceLocation);

        public List<TLoadedResource> GetAllResources()
        {
            return loadedResources.Values.ToList();
        }
    }
}
