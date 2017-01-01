using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Assets.Scripts.Util
{
    interface IJsonSerializable
    {
        void Serialize(JObject targetJObject);

        void Deserialize(JObject sourceJObject);
    }
}
