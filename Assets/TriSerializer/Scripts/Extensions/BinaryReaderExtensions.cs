using System;
using System.IO;
using UnityEngine;

namespace TriSerializer
{
    /// <summary>
    /// Provides extension methods for <see cref="BinaryReader"/> to read various data types and Unity-specific structures.
    /// </summary>
    public static class BinaryReaderExtensions
    {
        /// <summary>
        /// Reads a boolean value from the binary reader.
        /// </summary>
        /// <param name="binaryReader">The binary reader to read from.</param>
        /// <param name="value">The default boolean value (ignored).</param>
        /// <returns>The boolean value read from the stream.</returns>
        public static bool Read(this BinaryReader binaryReader, bool value)
        {
            return binaryReader.ReadBoolean();
        }

        /// <summary>
        /// Reads a 32-bit signed integer from the binary reader.
        /// </summary>
        /// <param name="binaryReader">The binary reader to read from.</param>
        /// <param name="value">The default integer value (ignored).</param>
        /// <returns>The 32-bit signed integer read from the stream.</returns>
        public static int Read(this BinaryReader binaryReader, int value)
        {
            return binaryReader.ReadInt32();
        }

        /// <summary>
        /// Reads a 32-bit unsigned integer from the binary reader.
        /// </summary>
        /// <param name="binaryReader">The binary reader to read from.</param>
        /// <param name="value">The default unsigned integer value (ignored).</param>
        /// <returns>The 32-bit unsigned integer read from the stream.</returns>
        public static uint Read(this BinaryReader binaryReader, uint value)
        {
            return binaryReader.ReadUInt32();
        }

        /// <summary>
        /// Reads a single-precision floating-point number from the binary reader.
        /// </summary>
        /// <param name="binaryReader">The binary reader to read from.</param>
        /// <param name="value">The default float value (ignored).</param>
        /// <returns>The single-precision float read from the stream.</returns>
        public static float Read(this BinaryReader binaryReader, float value)
        {
            return binaryReader.ReadSingle();
        }

        /// <summary>
        /// Reads a 64-bit unsigned integer from the binary reader.
        /// </summary>
        /// <param name="binaryReader">The binary reader to read from.</param>
        /// <param name="value">The default unsigned long value (ignored).</param>
        /// <returns>The 64-bit unsigned integer read from the stream.</returns>
        public static ulong Read(this BinaryReader binaryReader, ulong value)
        {
            return binaryReader.ReadUInt64();
        }

        /// <summary>
        /// Reads a string from the binary reader.
        /// </summary>
        /// <param name="binaryReader">The binary reader to read from.</param>
        /// <param name="value">The default string value (ignored).</param>
        /// <returns>The string read from the stream.</returns>
        public static string Read(this BinaryReader binaryReader, string value)
        {
            return binaryReader.ReadString();
        }

        /// <summary>
        /// Reads a Vector2 from the binary reader.
        /// </summary>
        /// <param name="binaryReader">The binary reader to read from.</param>
        /// <param name="value">The default Vector2 value (ignored).</param>
        /// <returns>The Vector2 read from the stream, with x and y components.</returns>
        public static Vector2 Read(this BinaryReader binaryReader, Vector2 value)
        {
            var vector = new Vector2();
            vector.x = Read(binaryReader, vector.x);
            vector.y = Read(binaryReader, vector.y);
            return vector;
        }

        /// <summary>
        /// Reads a Vector3 from the binary reader.
        /// </summary>
        /// <param name="binaryReader">The binary reader to read from.</param>
        /// <param name="value">The default Vector3 value (ignored).</param>
        /// <returns>The Vector3 read from the stream, with x, y, and z components.</returns>
        public static Vector3 Read(this BinaryReader binaryReader, Vector3 value)
        {
            var vector = new Vector3();
            vector.x = Read(binaryReader, vector.x);
            vector.y = Read(binaryReader, vector.y);
            vector.z = Read(binaryReader, vector.z);
            return vector;
        }

        /// <summary>
        /// Reads a Vector4 from the binary reader.
        /// </summary>
        /// <param name="binaryReader">The binary reader to read from.</param>
        /// <param name="value">The default Vector4 value (ignored).</param>
        /// <returns>The Vector4 read from the stream, with x, y, z, and w components.</returns>
        public static Vector4 Read(this BinaryReader binaryReader, Vector4 value)
        {
            var vector = new Vector4();
            vector.x = Read(binaryReader, vector.x);
            vector.y = Read(binaryReader, vector.y);
            vector.z = Read(binaryReader, vector.z);
            vector.w = Read(binaryReader, vector.w);
            return vector;
        }

        /// <summary>
        /// Reads a Quaternion from the binary reader.
        /// </summary>
        /// <param name="binaryReader">The binary reader to read from.</param>
        /// <param name="value">The default Quaternion value (ignored).</param>
        /// <returns>The Quaternion read from the stream, with x, y, z, and w components.</returns>
        public static Quaternion Read(this BinaryReader binaryReader, Quaternion value)
        {
            var quaternion = new Quaternion();
            quaternion.x = Read(binaryReader, quaternion.x);
            quaternion.y = Read(binaryReader, quaternion.y);
            quaternion.z = Read(binaryReader, quaternion.z);
            quaternion.w = Read(binaryReader, quaternion.w);
            return quaternion;
        }

        /// <summary>
        /// Reads a Matrix4x4 from the binary reader.
        /// </summary>
        /// <param name="binaryReader">The binary reader to read from.</param>
        /// <param name="value">The default Matrix4x4 value (ignored).</param>
        /// <returns>The Matrix4x4 read from the stream, with 16 float components.</returns>
        public static Matrix4x4 Read(this BinaryReader binaryReader, Matrix4x4 value)
        {
            var matrix = new Matrix4x4();
            for (var i = 0; i < 16; i++)
            {
                matrix[i] = Read(binaryReader, matrix[i]);
            }
            return matrix;
        }

        /// <summary>
        /// Reads a BoneWeight from the binary reader.
        /// </summary>
        /// <param name="binaryReader">The binary reader to read from.</param>
        /// <param name="value">The default BoneWeight value (ignored).</param>
        /// <returns>The BoneWeight read from the stream, with weights and bone indices.</returns>
        public static BoneWeight Read(this BinaryReader binaryReader, BoneWeight value)
        {
            var boneWeight = new BoneWeight();
            boneWeight.weight0 = Read(binaryReader, boneWeight.weight0);
            boneWeight.weight1 = Read(binaryReader, boneWeight.weight1);
            boneWeight.weight2 = Read(binaryReader, boneWeight.weight2);
            boneWeight.weight3 = Read(binaryReader, boneWeight.weight3);
            boneWeight.boneIndex0 = Read(binaryReader, boneWeight.boneIndex0);
            boneWeight.boneIndex1 = Read(binaryReader, boneWeight.boneIndex1);
            boneWeight.boneIndex2 = Read(binaryReader, boneWeight.boneIndex2);
            boneWeight.boneIndex3 = Read(binaryReader, boneWeight.boneIndex3);
            return boneWeight;
        }

        /// <summary>
        /// Reads a Bounds from the binary reader.
        /// </summary>
        /// <param name="binaryReader">The binary reader to read from.</param>
        /// <param name="value">The default Bounds value (ignored).</param>
        /// <returns>The Bounds read from the stream, with min and max Vector3 components.</returns>
        public static Bounds Read(this BinaryReader binaryReader, Bounds value)
        {
            var bounds = new Bounds();
            bounds.min = Read(binaryReader, bounds.min);
            bounds.max = Read(binaryReader, bounds.max);
            return bounds;
        }

        /// <summary>
        /// Reads and asserts the components of an ObjectIdentifier from the binary reader.
        /// </summary>
        /// <param name="binaryReader">The binary reader to read from.</param>
        /// <param name="source">The ObjectIdentifier containing expected values.</param>
        public static void ReadAndAssert(this BinaryReader binaryReader, ObjectIdentifier source)
        {
            ReadAndAssert(binaryReader, source.A);
            ReadAndAssert(binaryReader, source.B);
            ReadAndAssert(binaryReader, source.C);
        }

        /// <summary>
        /// Reads a character from the binary reader and asserts it matches the expected value.
        /// </summary>
        /// <param name="binaryReader">The binary reader to read from.</param>
        /// <param name="source">The expected character value.</param>
        public static void ReadAndAssert(this BinaryReader binaryReader, char source)
        {
            var value = binaryReader.ReadChar();
            Debug.Assert(value == source);
        }

        /// <summary>
        /// Reads a 32-bit signed integer from the binary reader and asserts it matches the expected value.
        /// </summary>
        /// <param name="binaryReader">The binary reader to read from.</param>
        /// <param name="source">The expected integer value.</param>
        /// <returns>The 32-bit signed integer read from the stream.</returns>
        public static int ReadAndAssert(this BinaryReader binaryReader, int source)
        {
            var value = binaryReader.ReadInt32();
            Debug.Assert(value == source);
            return value;
        }

        /// <summary>
        /// Reads an enumeration value from the binary reader.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="binaryReader">The binary reader to read from.</param>
        /// <param name="value">The default enum value (ignored).</param>
        /// <returns>The enumeration value read from the stream.</returns>
        public static T ReadEnum<T>(this BinaryReader binaryReader, T value) where T : Enum
        {
            return (T)Enum.ToObject(typeof(T), binaryReader.ReadInt32());
        }

        /// <summary>
        /// Reads an ObjectIdentifier from the binary reader.
        /// </summary>
        /// <param name="binaryReader">The binary reader to read from.</param>
        /// <returns>The ObjectIdentifier constructed from three characters read from the stream.</returns>
        public static ObjectIdentifier ReadIdentifier(this BinaryReader binaryReader)
        {
            var a = binaryReader.ReadChar();
            var b = binaryReader.ReadChar();
            var c = binaryReader.ReadChar();
            var objectIdentifier = new ObjectIdentifier(a, b, c);
            return objectIdentifier;
        }
    }
}