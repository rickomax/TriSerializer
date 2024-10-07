using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TriSerializer
{
    public static partial class Serializer
    {
        private static readonly Dictionary<Type, IObjectSerializer> ObjectSerializers = new()
        {
            { typeof(GameObject), new GameObjectSerializer() },

            { typeof(Transform), new TransformSerializer()},
            { typeof(MeshFilter), new MeshFilterSerializer() },
            { typeof(MeshRenderer), new MeshRendererSerializer() },
            { typeof(SkinnedMeshRenderer), new SkinnedMeshRendererSerializer() },

            { typeof(Mesh), new MeshSerializer() },
            { typeof(Material), new MaterialSerializer() }
        };

        private static Dictionary<ObjectIdentifier, IObjectSerializer> ObjectDeserializers;

        static Serializer()
        {
            if (ObjectDeserializers == null)
            {
                ObjectDeserializers = new Dictionary<ObjectIdentifier, IObjectSerializer>(ObjectSerializers.Count);
                foreach (var kvp in ObjectSerializers)
                {
                    ObjectDeserializers.Add(kvp.Value.Identifier, kvp.Value);
                }
            }
        }

        public static void SerializeToFile(string filename, GameObject gameObject, bool nonLock)
        {
            var enumerator = SerializeToFileInternal(filename, gameObject);
            if (nonLock)
            {
                CoroutineHelper.Instance.StartCoroutine(enumerator);
            }
            else
            {
                while (enumerator.MoveNext())
                {

                }
            }
        }

        public static void DeserializeToGameObject(string filename, GameObject gameObject, bool nonLock)
        {
            var enumerator = DeserializeToGameObjectInternal(filename, gameObject);
            if (nonLock)
            {
                CoroutineHelper.Instance.StartCoroutine(enumerator);
            }
            else
            {
                while (enumerator.MoveNext())
                {

                }
            }
        }

        public static IEnumerator DeserializeToGameObjectInternal(string filename, GameObject gameObject)
        {
            Debug.Assert(File.Exists(filename));
            Debug.Assert(gameObject != null);
            using (var binaryReader = new BinaryReader(File.OpenRead(filename)))
            {
                var serializationContext = new SerializationContext();
                serializationContext.BinaryReader = binaryReader;
                binaryReader.ReadAndAssert(Shared.Identifier);
                binaryReader.ReadAndAssert(Shared.Version);
                while (binaryReader.BaseStream.CanRead)
                {
                    var identifier = binaryReader.ReadIdentifier();
                    if (ObjectDeserializers.TryGetValue(identifier, out var deserializer))
                    {
                        foreach (var item in deserializer.Deserialize(serializationContext))
                        {
                            yield return item;
                        }
                    }
                }
            }
            yield break;
        }

        public static IEnumerator SerializeToFileInternal(string filename, GameObject gameObject)
        {
            Debug.Assert(FileSystemHelper.IsPathValid(filename));
            Debug.Assert(gameObject != null);
            using (var binaryWriter = new BinaryWriter(File.Create(filename)))
            {
                var serializationContext = new SerializationContext();
                serializationContext.BinaryWriter = binaryWriter;
                binaryWriter.Write(Shared.Identifier);
                binaryWriter.Write(Shared.Version);
                var components = gameObject.GetComponents<Component>();
                foreach (var component in components)
                {
                    if (component is Transform transform)
                    {
                        foreach (var item in SerializeObject(serializationContext, transform.gameObject))
                        {
                            yield return item;
                        }
                    }
                }
                foreach (var component in components)
                {
                    foreach (var item in CollectResources(serializationContext, component))
                    {
                        yield return item;
                    }
                }
                foreach (var resource in serializationContext.Resources)
                {
                    foreach (var item in SerializeObject(serializationContext, resource))
                    {
                        yield return item;
                    }
                }
                foreach (var component in components)
                {
                    foreach (var item in SerializeObject(serializationContext, component))
                    {
                        yield return item;
                    }
                }
            }
        }

        private static IEnumerable SerializeObject(SerializationContext serializationContext, UnityEngine.Object source)
        {
            if (ObjectSerializers.TryGetValue(source.GetType(), out var serializer))
            {
                foreach (var item in serializer.Serialize(serializationContext, source))
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable CollectResources(SerializationContext serializationContext, UnityEngine.Object source)
        {
            if (ObjectSerializers.TryGetValue(source.GetType(), out var serializer))
            {
                foreach (var item in serializer.CollectResources(serializationContext, source))
                {
                    yield return item;
                }
            }
        }
    }
}
