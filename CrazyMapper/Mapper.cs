using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrazyMapper
{
    public delegate void AfterInstance<TTarget>(TTarget target, object parent);

    public static class Mapper
    {
        static Dictionary<Type, Type> DefaultBindedSystem = new Dictionary<Type, Type>();
        static Dictionary<Type, Action<object, object>> DefaultAfterInstanceSystem = new Dictionary<Type, Action<object, object>>();

        public static T Map<T>(object source)
        {
            using (var map = new MapInfo(DefaultBindedSystem, DefaultAfterInstanceSystem))
            {
                var result = map.MapObject(typeof(T), source);
                return (T)result;
            }
        }

        public static void Bind<TSource, TTarget>()
        {
            DefaultBindedSystem[typeof(TSource)] = typeof(TTarget);
        }

        public static void AfterInstance<TTarget>(AfterInstance<TTarget> action)
        {
            if (action == null)
                throw new Exception("cannot set nul value of action!");
            DefaultAfterInstanceSystem[typeof(TTarget)] = (obj, parent) =>
            {
                action((TTarget)obj, parent);
            };
        }

        internal static object CreateInstance(MapInfo mapInfo, Type type)
        {
            Type instanceType = null;
            if (mapInfo.BindedSystem.ContainsKey(type))
                instanceType = mapInfo.BindedSystem[type];
            else
                instanceType = type;
            try
            {
                var instance = Activator.CreateInstance(instanceType);
                return instance;
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot create instance of Type {instanceType.ToString()} because : {ex.Message}");
            }
        }
    }
}
