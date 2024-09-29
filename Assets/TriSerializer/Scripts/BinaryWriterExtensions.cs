using System.IO;
using UnityEngine;

namespace TriSerializer
{

    public static class BinaryWriterExtensions
    {
        public static void WriteReference(this BinaryWriter binaryWriter, Object value)
        {
            binaryWriter.Write(value == null ? 0 : value.GetInstanceID());
        }

        public static void Write(this BinaryWriter binaryWriter, Vector3 value)
        {
            binaryWriter.Write(value.x);
            binaryWriter.Write(value.y);
            binaryWriter.Write(value.z);
        }

        public static void Write(this BinaryWriter binaryWriter, Quaternion value)
        {
            binaryWriter.Write(value.x);
            binaryWriter.Write(value.y);
            binaryWriter.Write(value.z);
            binaryWriter.Write(value.w);
        }

        public static void Write(this BinaryWriter binaryWriter, Matrix4x4 value)
        {
            for (var i = 0; i < 16; i++)
            {
                binaryWriter.Write(value[i]);
            }
        }

        public static void Write(this BinaryWriter binaryWriter, Bounds bounds)
        {
            Write(binaryWriter, bounds.min);
            Write(binaryWriter, bounds.max);
        }

        public static void Write(this BinaryWriter binaryWriter, Vector3[] vertices)
        {
            if (vertices == null)
            {
                binaryWriter.Write(0);
                return;
            }
            binaryWriter.Write(vertices.Length);
            for (var i = 0; i < vertices.Length; i++)
            {
                Write(binaryWriter, vertices[i]);
            }
        }

        public static void Write(this BinaryWriter binaryWriter, ObjectIdentifier objectIdentifier)
        {
            binaryWriter.Write(objectIdentifier.A);
            binaryWriter.Write(objectIdentifier.B);
            binaryWriter.Write(objectIdentifier.C);
        }
    }
}