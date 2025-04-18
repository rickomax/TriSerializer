using System.IO;
using UnityEngine;

namespace TriSerializer
{
    /// <summary>
    /// Provides extension methods for <see cref="BinaryWriter"/> to write Unity-specific data types and structures.
    /// </summary>
    public static class BinaryWriterExtensions
    {
        /// <summary>
        /// Writes a Vector2 to the binary writer.
        /// </summary>
        /// <param name="binaryWriter">The binary writer to write to.</param>
        /// <param name="value">The Vector2 to write.</param>
        public static void Write(this BinaryWriter binaryWriter, Vector2 value)
        {
            binaryWriter.Write(value.x);
            binaryWriter.Write(value.y);
        }

        /// <summary>
        /// Writes a Vector3 to the binary writer.
        /// </summary>
        /// <param name="binaryWriter">The binary writer to write to.</param>
        /// <param name="value">The Vector3 to write.</param>
        public static void Write(this BinaryWriter binaryWriter, Vector3 value)
        {
            binaryWriter.Write(value.x);
            binaryWriter.Write(value.y);
            binaryWriter.Write(value.z);
        }

        /// <summary>
        /// Writes a Vector4 to the binary writer.
        /// </summary>
        /// <param name="binaryWriter">The binary writer to write to.</param>
        /// <param name="value">The Vector4 to write.</param>
        public static void Write(this BinaryWriter binaryWriter, Vector4 value)
        {
            binaryWriter.Write(value.x);
            binaryWriter.Write(value.y);
            binaryWriter.Write(value.z);
            binaryWriter.Write(value.w);
        }

        /// <summary>
        /// Writes a Quaternion to the binary writer.
        /// </summary>
        /// <param name="binaryWriter">The binary writer to write to.</param>
        /// <param name="value">The Quaternion to write.</param>
        public static void Write(this BinaryWriter binaryWriter, Quaternion value)
        {
            binaryWriter.Write(value.x);
            binaryWriter.Write(value.y);
            binaryWriter.Write(value.z);
            binaryWriter.Write(value.w);
        }

        /// <summary>
        /// Writes a Color to the binary writer.
        /// </summary>
        /// <param name="binaryWriter">The binary writer to write to.</param>
        /// <param name="value">The Color to write.</param>
        public static void Write(this BinaryWriter binaryWriter, Color value)
        {
            binaryWriter.Write(value.r);
            binaryWriter.Write(value.g);
            binaryWriter.Write(value.b);
            binaryWriter.Write(value.a);
        }

        /// <summary>
        /// Writes a BoneWeight to the binary writer.
        /// </summary>
        /// <param name="binaryWriter">The binary writer to write to.</param>
        /// <param name="value">The BoneWeight to write.</param>
        public static void Write(this BinaryWriter binaryWriter, BoneWeight value)
        {
            binaryWriter.Write(value.weight0);
            binaryWriter.Write(value.weight1);
            binaryWriter.Write(value.weight2);
            binaryWriter.Write(value.weight3);
            binaryWriter.Write(value.boneIndex0);
            binaryWriter.Write(value.boneIndex1);
            binaryWriter.Write(value.boneIndex2);
            binaryWriter.Write(value.boneIndex3);
        }

        /// <summary>
        /// Writes a Matrix4x4 to the binary writer.
        /// </summary>
        /// <param name="binaryWriter">The binary writer to write to.</param>
        /// <param name="value">The Matrix4x4 to write.</param>
        public static void Write(this BinaryWriter binaryWriter, Matrix4x4 value)
        {
            for (var i = 0; i < 16; i++)
            {
                binaryWriter.Write(value[i]);
            }
        }

        /// <summary>
        /// Writes a Bounds to the binary writer.
        /// </summary>
        /// <param name="binaryWriter">The binary writer to write to.</param>
        /// <param name="bounds">The Bounds to write.</param>
        public static void Write(this BinaryWriter binaryWriter, Bounds bounds)
        {
            Write(binaryWriter, bounds.min);
            Write(binaryWriter, bounds.max);
        }

        /// <summary>
        /// Writes an ObjectIdentifier to the binary writer.
        /// </summary>
        /// <param name="binaryWriter">The binary writer to write to.</param>
        /// <param name="objectIdentifier">The ObjectIdentifier to write.</param>
        public static void Write(this BinaryWriter binaryWriter, ObjectIdentifier objectIdentifier)
        {
            binaryWriter.Write(objectIdentifier.A);
            binaryWriter.Write(objectIdentifier.B);
            binaryWriter.Write(objectIdentifier.C);
        }
    }
}