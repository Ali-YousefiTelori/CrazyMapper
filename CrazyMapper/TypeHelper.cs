﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrazyMapper
{
    /// <summary>
    /// object types of enumerable
    /// </summary>
    internal enum SerializeObjectType
    {
        /// <summary>
        /// uknown file type or type is null
        /// </summary>
        None = 0,
        Object = 1,
        Char = 2,
        CharNullable = 3,
        Boolean = 4,
        BooleanNullable = 5,
        Byte = 6,
        ByteNullable = 7,
        SByte = 8,
        SByteNullable = 9,
        Int16 = 10,
        Int16Nullable = 11,
        UInt16 = 12,
        UInt16Nullable = 13,
        Int32 = 14,
        Int32Nullable = 15,
        UInt32 = 16,
        UInt32Nullable = 17,
        Int64 = 18,
        Int64Nullable = 19,
        UInt64 = 20,
        UInt64Nullable = 21,
        Single = 22,
        SingleNullable = 23,
        Double = 24,
        DoubleNullable = 25,
        DateTime = 26,
        DateTimeNullable = 27,
        DateTimeOffset = 28,
        DateTimeOffsetNullable = 29,
        Decimal = 30,
        DecimalNullable = 31,
        Guid = 32,
        GuidNullable = 33,
        TimeSpan = 34,
        TimeSpanNullable = 35,
        BigInteger = 36,
        BigIntegerNullable = 37,
        Uri = 38,
        String = 39,
        Bytes = 40,
        DBNull = 41,
        Enum = 42,
        Void = 43,
        IntPtr = 44
    }

    internal static class TypeHelper
    {
        private static readonly Dictionary<Type, SerializeObjectType> TypeCodeMap = new Dictionary<Type, SerializeObjectType>
        {
            { typeof(char), SerializeObjectType.Char },
            { typeof(char?), SerializeObjectType.CharNullable },
            { typeof(bool), SerializeObjectType.Boolean },
            { typeof(bool?), SerializeObjectType.BooleanNullable },
            { typeof(byte), SerializeObjectType.Byte },
            { typeof(byte?), SerializeObjectType.ByteNullable },
            { typeof(sbyte), SerializeObjectType.SByte },
            { typeof(sbyte?), SerializeObjectType.SByteNullable },
            { typeof(short), SerializeObjectType.Int16 },
            { typeof(short?), SerializeObjectType.Int16Nullable },
            { typeof(ushort), SerializeObjectType.UInt16 },
            { typeof(ushort?), SerializeObjectType.UInt16Nullable },
            { typeof(int), SerializeObjectType.Int32 },
            { typeof(int?), SerializeObjectType.Int32Nullable },
            { typeof(uint), SerializeObjectType.UInt32 },
            { typeof(uint?), SerializeObjectType.UInt32Nullable },
            { typeof(long), SerializeObjectType.Int64 },
            { typeof(long?), SerializeObjectType.Int64Nullable },
            { typeof(ulong), SerializeObjectType.UInt64 },
            { typeof(ulong?), SerializeObjectType.UInt64Nullable },
            { typeof(float), SerializeObjectType.Single },
            { typeof(float?), SerializeObjectType.SingleNullable },
            { typeof(double), SerializeObjectType.Double },
            { typeof(double?), SerializeObjectType.DoubleNullable },
            { typeof(DateTime), SerializeObjectType.DateTime },
            { typeof(DateTime?), SerializeObjectType.DateTimeNullable },
            { typeof(DateTimeOffset), SerializeObjectType.DateTimeOffset },
            { typeof(DateTimeOffset?), SerializeObjectType.DateTimeOffsetNullable },
            { typeof(decimal), SerializeObjectType.Decimal },
            { typeof(decimal?), SerializeObjectType.DecimalNullable },
            { typeof(Guid), SerializeObjectType.Guid },
            { typeof(Guid?), SerializeObjectType.GuidNullable },
            { typeof(TimeSpan), SerializeObjectType.TimeSpan },
            { typeof(TimeSpan?), SerializeObjectType.TimeSpanNullable },
            { typeof(string), SerializeObjectType.String },
            { typeof(IntPtr), SerializeObjectType.IntPtr },
            { typeof(void), SerializeObjectType.Void },
#if (!NETSTANDARD1_6 && !NETCOREAPP1_1 && !PORTABLE)
            { typeof(DBNull), SerializeObjectType.DBNull }
#endif
        };

        /// <summary>
        /// get type code of type
        /// </summary>
        /// <param name="type">your type</param>
        /// <returns>type code</returns>
        public static SerializeObjectType GetTypeCodeOfObject(Type type)
        {
            if (type == null)
                return SerializeObjectType.None;
            else if (TypeCodeMap.ContainsKey(type))
                return TypeCodeMap[type];
            else if (type.IsEnum)
                return SerializeObjectType.Enum;
            return SerializeObjectType.Object;
        }

        public static bool IsDictionary(Type type)
        {
            return typeof(System.Collections.IDictionary).IsAssignableFrom(type);
        }

        public static bool IsArray(Type type, object value)
        {
            return typeof(System.Collections.IEnumerable).IsAssignableFrom(type) && !(value is string);
        }
    }
}
