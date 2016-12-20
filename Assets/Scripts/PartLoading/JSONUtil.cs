using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Assets.Scripts.PartLoading
{
    static class JSONUtil
    {
        public static T ReadProperty<T>(JObject jObject, string propertyName)
        {
            JToken outToken;
            if (jObject.TryGetValue(propertyName, out outToken))
            {
                object parsed = ParseJTokenToCSharpType(outToken);
                if (parsed is T)
                {
                    return (T) parsed;
                }
                else
                {
                    throw new PropertyReadException(propertyName, jObject, outToken.ToString(), typeof(T));
                }
            }
            else
            {
                throw new PropertyReadException(propertyName, jObject, null, typeof(T));
            }
        }

        public static JObject ReadObject(JObject jObject, string propertyName)
        {
            JToken outToken;
            if (jObject.TryGetValue(propertyName, out outToken))
            {
                JObject casted = outToken as JObject;
                if (casted == null)
                {
                    throw new PropertyReadException(propertyName, jObject, outToken.ToString(), typeof(JObject));
                }
                else
                {
                    return casted;
                }
            }
            else
            {
                throw new PropertyReadException(propertyName, jObject, null, typeof(JObject));
            }
        }

        public static JArray ReadArray(JObject jObject, string propertyName)
        {
            JToken outToken;
            if (jObject.TryGetValue(propertyName, out outToken))
            {
                JArray casted = outToken as JArray;
                if (casted == null)
                {
                    throw new PropertyReadException(propertyName, jObject, outToken.ToString(), typeof(JArray));
                }
                else
                {
                    return casted;
                }
            }
            else
            {
                throw new PropertyReadException(propertyName, jObject, null, typeof(JArray));
            }
        }

        public static T[] ReadArray<T>(JObject jObject, string propertyName)
        {
            JArray jArray = ReadArray(jObject, propertyName);
            T[] array = new T[jArray.Count];
            int index = 0;
            foreach (JToken jToken in jArray)
            {
                T casted = Extensions.Value<T>(jToken);
                if (casted == null)
                    throw new PropertyReadException(propertyName, jObject, null, typeof(T[]));
                array[index] = casted;
                index++;
            }
            return array;
        }

        public static T[] ReadArrayWithDefaultValue<T>(JObject jObject, string propertyName, T[] defaultValue)
        {
            JArray jArray;
            try
            {
                jArray = ReadArray(jObject, propertyName);
            }
            catch (PropertyReadException)
            {
                return defaultValue;
            }

            T[] array = new T[jArray.Count];
            int index = 0;
            foreach (JToken jToken in jArray)
            {
                T casted = Extensions.Value<T>(jToken);
                if (casted == null)
                    throw new PropertyReadException(propertyName, jObject, null, typeof(T[]));
                array[index] = casted;
                index++;
            }
            return array;
        }

        public static T ReadWithDefaultValue<T>(JObject jObject, string propertyName, T defaultValue)
        {
            JToken outToken;
            if (jObject.TryGetValue(propertyName, out outToken))
            {
                object parsed = ParseJTokenToCSharpType(outToken);
                if (parsed is T)
                {
                    return (T)parsed;
                }
                else
                {
                    throw new PropertyReadException(propertyName, jObject, outToken.ToString(), typeof(T));
                }
            }
            else
            {
                return defaultValue;
            }
        }

        public static Vector2 ArrayToVector2(float[] array)
        {
            if (array.Length == 2)
                return new Vector2(array[0], array[1]);
            else
                throw new ArgumentException("Array needs to have exactly two members in order to be parsed into a Vector2");
        }

        public static Vector2 ArrayToVector2(int[] array)
        {
            if (array.Length == 2)
                return new Vector2(array[0], array[1]);
            else
                throw new ArgumentException("Array needs to have exactly two members in order to be parsed into a Vector2");
        }

        public static Vector2 ArrayToVector2(JArray array)
        {
            if (array.Count == 2)
                return new Vector2(Extensions.Value<float>(array[0]), Extensions.Value<float>(array[1]));
            else
                throw new ArgumentException("Array needs to have exactly two members in order to be parsed into a Vector2");
        }

        public static object ParseJTokenToCSharpType(JToken jToken)
        {
            switch (jToken.Type)
            {
                case JTokenType.None:
                    throw new UnsupportedTokenException(jToken.Type);
                case JTokenType.Object:
                    throw new UnsupportedTokenException(jToken.Type);
                case JTokenType.Array:
                    // not working as expected
                    //JArray jArray = (JArray)jToken;
                    //var array = Array.CreateInstance(ParseJTokenToCSharpType(jArray.First).GetType(), jArray.Count);
                    //int index = 0;
                    //foreach (JToken token in jArray)
                    //{
                    //    array.SetValue(ParseJTokenToCSharpType(token), index);
                    //    index++;
                    //}
                    //return array;
                    throw new UnsupportedTokenException(jToken.Type);
                case JTokenType.Constructor:
                    throw new UnsupportedTokenException(jToken.Type);
                case JTokenType.Property:
                    throw new UnsupportedTokenException(jToken.Type);
                case JTokenType.Comment:
                    throw new UnsupportedTokenException(jToken.Type);
                case JTokenType.Integer:
                    return Extensions.Value<int>(jToken);
                case JTokenType.Float:
                    return Extensions.Value<float>(jToken);
                case JTokenType.String:
                    return Extensions.Value<string>(jToken);
                case JTokenType.Boolean:
                    return Extensions.Value<bool>(jToken);
                case JTokenType.Null:
                    return null;
                case JTokenType.Undefined:
                    throw new UnsupportedTokenException(jToken.Type);
                case JTokenType.Date:
                    throw new UnsupportedTokenException(jToken.Type);
                case JTokenType.Raw:
                    throw new UnsupportedTokenException(jToken.Type);
                case JTokenType.Bytes:
                    throw new UnsupportedTokenException(jToken.Type);
                case JTokenType.Guid:
                    return Extensions.Value<Guid>(jToken);
                case JTokenType.Uri:
                    throw new UnsupportedTokenException(jToken.Type);
                case JTokenType.TimeSpan:
                    throw new UnsupportedTokenException(jToken.Type);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
