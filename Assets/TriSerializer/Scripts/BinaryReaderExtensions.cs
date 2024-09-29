﻿using System;
using System.IO;
using UnityEngine;

namespace TriSerializer
{
    public static class BinaryReaderExtensions
    {
        public static bool Read(this BinaryReader binaryReader, bool value)
        {
           return binaryReader.ReadBoolean();
        }

        public static int Read(this BinaryReader binaryReader, int value)
        {
            return binaryReader.ReadInt32();
        }

        public static uint Read(this BinaryReader binaryReader, uint value)
        {
            return binaryReader.ReadUInt32();
        }

        public static float Read(this BinaryReader binaryReader, float value)
        {
            return binaryReader.ReadSingle();
        }

        public static ulong Read(this BinaryReader binaryReader, ulong value)
        {
            return binaryReader.ReadUInt64();
        }

        public static T ReadEnum<T>(this BinaryReader binaryReader, T value) where T : Enum
        {
            return (T)Enum.ToObject(typeof(T), binaryReader.ReadInt32());
        }

        public static string Read(this BinaryReader binaryReader, string value)
        {
            return binaryReader.ReadString();
        }

        public static Vector3 Read(this BinaryReader binaryReader, Vector3 value)
        {
            var vector = new Vector3();
            vector.x = Read(binaryReader, vector.x);
            vector.y = Read(binaryReader, vector.y);
            vector.z = Read(binaryReader, vector.z);
            return vector;
        }
        public static Quaternion Read(this BinaryReader binaryReader, Quaternion value)
        {
            var quaternion = new Quaternion();
            quaternion.x = Read(binaryReader, quaternion.x);
            quaternion.y = Read(binaryReader, quaternion.y);
            quaternion.z = Read(binaryReader, quaternion.z);
            quaternion.w = Read(binaryReader, quaternion.w);
            return quaternion;
        }

        public static Bounds Read(this BinaryReader binaryReader, Bounds value)
        {
            var bounds = new Bounds();
            bounds.min = Read(binaryReader, bounds.min);
            bounds.max = Read(binaryReader, bounds.max);
            return bounds;
        }
        public static void ReadAndAssert(this BinaryReader binaryReader, ObjectIdentifier source)
        {
            ReadAndAssert(binaryReader, source.A);
            ReadAndAssert(binaryReader, source.B);
            ReadAndAssert(binaryReader, source.C);
        }

        public static void ReadAndAssert(this BinaryReader binaryReader, char source)
        {
            var value = binaryReader.ReadChar();
            Debug.Assert(value == source);
        }

        public static void ReadAndAssert(this BinaryReader binaryReader, int source)
        {
            var value = binaryReader.ReadInt32();
            Debug.Assert(value == source);
        }

        public static ObjectIdentifier ReadIdentifier(this BinaryReader binaryReader)
        {
            var a = binaryReader.ReadChar();
            var b = binaryReader.ReadChar();
            var c = binaryReader.ReadChar();
            var objectIdentifier = new ObjectIdentifier(a,b,c);
            return objectIdentifier;
        }
    }
}