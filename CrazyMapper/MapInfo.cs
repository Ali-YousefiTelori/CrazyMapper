using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrazyMapper
{
    public class MapInfo : IDisposable
    {
        internal Dictionary<Type, Type> BindedSystem { get; set; } = new Dictionary<Type, Type>();
        internal Dictionary<Type, Action<object, object>> DefaultAfterInstanceSystem = new Dictionary<Type, Action<object, object>>();

        internal Dictionary<object, object> MappedItems = new Dictionary<object, object>();

        public MapInfo()
        {

        }


        public MapInfo(Dictionary<Type, Type> bindedSystem, Dictionary<Type, Action<object, object>> afterInstanceSystem)
        {
            BindedSystem = bindedSystem;
            DefaultAfterInstanceSystem = afterInstanceSystem;
        }

        public T MapObject<T>(T targetType, object source, object parent = null)
        {
            return (T)MapObject(typeof(T), source, parent);
        }

        public object MapObject(Type targetType, object source, object parent = null)
        {
            if (source == null)
                return null;
            var typeOfSource = source.GetType();
            if (typeOfSource == targetType)
                return source;
            if (MappedItems.ContainsKey(source))
            {
                return MappedItems[source];
            }
            if (TypeHelper.IsDictionary(targetType))
            {
                if (TypeHelper.IsDictionary(typeOfSource))
                {
                    return MapDictionary(targetType, source, parent);
                }
                else
                {
                    return null;
                }
            }
            else if (TypeHelper.IsArray(targetType, source))
            {
                if (TypeHelper.IsArray(typeOfSource, source))
                {
                    return MapArray(targetType, source, parent);
                }
                else
                {
                    return null;
                }
            }

            var target = Mapper.CreateInstance(this, targetType);
            if (DefaultAfterInstanceSystem.ContainsKey(target.GetType()))
                DefaultAfterInstanceSystem[target.GetType()].Invoke(target, parent);
            MappedItems[source] = target;
            foreach (var targetProperty in target.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
            {
                if (!targetProperty.CanRead)
                    continue;
                var sourceProperty = typeOfSource.GetProperty(targetProperty.Name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                if (sourceProperty == null || !targetProperty.CanWrite)
                    continue;
                if (sourceProperty.PropertyType == targetProperty.PropertyType)
                {
                    targetProperty.SetValue(target, sourceProperty.GetValue(source, null), null);
                }
                else
                {
                    var valueOfSource = sourceProperty.GetValue(source, null);
                    if (valueOfSource == null)
                    {
                        targetProperty.SetValue(target, null, null);
                        continue;
                    }
                    else
                    {
                        var value = ConvertType(valueOfSource.GetType(), targetProperty.PropertyType, valueOfSource, target);
                        targetProperty.SetValue(target, value, null);
                    }
                }
            }
            
            return target;
        }

        internal object MapDictionary(Type toType, object fromValue, object parent = null)
        {
            if (MappedItems.ContainsKey(fromValue))
                return MappedItems[fromValue];

            var instance = Mapper.CreateInstance(this, toType);
            if (!(instance is System.Collections.IDictionary))
                return null;
            if (DefaultAfterInstanceSystem.ContainsKey(instance.GetType()))
                DefaultAfterInstanceSystem[instance.GetType()].Invoke(instance, parent);
            MappedItems[fromValue] = instance;
            var target = (System.Collections.IDictionary)instance;
            var typing = toType.GetGenericArguments();
            Type keyType = typing[0];
            Type valueType = typing[1];

            foreach (System.Collections.DictionaryEntry item in (System.Collections.IDictionary)fromValue)
            {
                target.Add(MapObject(keyType, item.Key, parent), MapObject(valueType, item.Value, parent));
            }

            return target;
        }

        internal object MapArray(Type toType, object fromValue, object parent = null)
        {
            if (MappedItems.ContainsKey(fromValue))
                return MappedItems[fromValue];
            var instance = Mapper.CreateInstance(this, toType);
            if (!(instance is System.Collections.IEnumerable))
                return null;

            if (DefaultAfterInstanceSystem.ContainsKey(instance.GetType()))
                DefaultAfterInstanceSystem[instance.GetType()].Invoke(instance, parent);
            MappedItems[fromValue] = instance;
            var genericType = instance.GetType().GetGenericArguments().FirstOrDefault();
            var target = (System.Collections.IEnumerable)instance;
            var addMethod = target.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).FirstOrDefault(x => x.Name == "Add" && x.GetParameters().Count() == 1);
            foreach (var item in (System.Collections.IEnumerable)fromValue)
            {
                if (item == null)
                {
                    addMethod.Invoke(target, null);
                }
                else
                {
                    if (genericType != null)
                    {
                        addMethod.Invoke(target, new object[] { MapObject(genericType, item, parent) });
                    }
                    else
                    {
                        addMethod.Invoke(target, new object[] { MapObject(item.GetType(), item, parent) });
                    }
                }
            }
            return instance;
        }

        internal object ConvertType(Type fromType, Type toType, object fromValue, object parent = null)
        {
            //var sourcePropertyValueType = TypeHelper.GetTypeCodeOfObject(valueOfSource.GetType());
            if (TypeHelper.IsDictionary(toType))
            {
                if (TypeHelper.IsDictionary(fromType))
                {
                    return MapDictionary(toType, fromValue, parent);
                }
                else
                {
                    return null;
                }
            }
            else if (TypeHelper.IsArray(toType, fromValue))
            {
                if (TypeHelper.IsArray(fromType, fromValue))
                {
                    return MapArray(toType, fromValue, parent);
                }
                else
                {
                    return null;
                }
            }
            var targetPropertyType = TypeHelper.GetTypeCodeOfObject(toType);
            if (targetPropertyType == SerializeObjectType.Boolean || targetPropertyType == SerializeObjectType.BooleanNullable)
            {
                return Convert.ToBoolean(fromValue);
            }
            else if (targetPropertyType == SerializeObjectType.Byte || targetPropertyType == SerializeObjectType.ByteNullable)
            {
                return Convert.ToByte(fromValue);
            }
            else if (targetPropertyType == SerializeObjectType.Char || targetPropertyType == SerializeObjectType.CharNullable)
            {
                return Convert.ToChar(fromValue);
            }
            else if (targetPropertyType == SerializeObjectType.DateTime || targetPropertyType == SerializeObjectType.DateTimeNullable)
            {
                return Convert.ToDateTime(fromValue);
            }
            else if (targetPropertyType == SerializeObjectType.DateTimeOffset || targetPropertyType == SerializeObjectType.DateTimeOffsetNullable)
            {
                return new DateTimeOffset(Convert.ToDateTime(fromValue));
            }
            else if (targetPropertyType == SerializeObjectType.Decimal || targetPropertyType == SerializeObjectType.DecimalNullable)
            {
                return Convert.ToDecimal(fromValue);
            }
            else if (targetPropertyType == SerializeObjectType.Double || targetPropertyType == SerializeObjectType.DoubleNullable)
            {
                return Convert.ToDouble(fromValue);
            }
            else if (targetPropertyType == SerializeObjectType.Enum)
            {
                return Enum.Parse(toType, fromValue.ToString());
            }
            else if (targetPropertyType == SerializeObjectType.Guid)
            {
                return Guid.Parse(fromValue.ToString());
            }
            else if (targetPropertyType == SerializeObjectType.Int16 || targetPropertyType == SerializeObjectType.Int16Nullable)
            {
                return Convert.ToInt16(fromValue);
            }
            else if (targetPropertyType == SerializeObjectType.Int32 || targetPropertyType == SerializeObjectType.Int32Nullable)
            {
                return Convert.ToInt32(fromValue);
            }
            else if (targetPropertyType == SerializeObjectType.Int64 || targetPropertyType == SerializeObjectType.Int64Nullable)
            {
                return Convert.ToInt64(fromValue);
            }
            else if (targetPropertyType == SerializeObjectType.UInt16 || targetPropertyType == SerializeObjectType.UInt16Nullable)
            {
                return Convert.ToUInt16(fromValue);
            }
            else if (targetPropertyType == SerializeObjectType.UInt32 || targetPropertyType == SerializeObjectType.UInt32Nullable)
            {
                return Convert.ToUInt32(fromValue);
            }
            else if (targetPropertyType == SerializeObjectType.UInt64 || targetPropertyType == SerializeObjectType.UInt64Nullable)
            {
                return Convert.ToUInt64(fromValue);
            }
            else if (targetPropertyType == SerializeObjectType.SByte || targetPropertyType == SerializeObjectType.SByteNullable)
            {
                return Convert.ToSByte(fromValue);
            }
            else if (targetPropertyType == SerializeObjectType.Single || targetPropertyType == SerializeObjectType.SingleNullable)
            {
                return Convert.ToSingle(fromValue);
            }
            else if (targetPropertyType == SerializeObjectType.String)
            {
                return fromValue.ToString();
            }
            else if (targetPropertyType == SerializeObjectType.TimeSpan || targetPropertyType == SerializeObjectType.TimeSpanNullable)
            {
                return TimeSpan.Parse(fromValue.ToString());
            }
            else if (targetPropertyType == SerializeObjectType.Uri)
            {
                return new Uri(fromValue.ToString());
            }
            else
            {
                return MapObject(toType, fromValue, parent);
            }
        }

        public void Dispose()
        {

        }
    }
}
