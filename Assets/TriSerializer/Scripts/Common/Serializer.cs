using ICSharpCode.SharpZipLib.GZip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TriSerializer
{
    /// <summary>
    /// The main class that serializes/deserializes GameObjects to/from files and Streams.
    /// </summary>
    public static partial class Serializer
    {
        /// <summary>
        /// The list of registered object/resource serializers.
        /// </summary>
        public static readonly Dictionary<Type, IObjectSerializer> ObjectSerializers = new()
            {
            { typeof(GameObject), new GameObjectSerializer() },
            { typeof(Transform), new TransformSerializer()},
            { typeof(MeshFilter), new MeshFilterSerializer() },
            { typeof(MeshRenderer), new MeshRendererSerializer() },
            { typeof(SkinnedMeshRenderer), new SkinnedMeshRendererSerializer() },
            { typeof(Mesh), new MeshSerializer() },
            { typeof(Material), new MaterialSerializer() },
            { typeof(Texture2D), new TextureSerializer2D() }
        };

        /// <summary>
        /// The list of registered object/resource deserializers (copied from the ObjectSerializers list).
        /// </summary>
        public static Dictionary<ObjectIdentifier, IObjectSerializer> ObjectDeserializers;

        /// <summary>
        /// Initializes the object deserializers.
        /// </summary>
        private static void Initialize()
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

        /// <summary>
        /// Deserializes the data from the given filename into the given GameObject.
        /// </summary>
        /// <param name="filename">The filename where to load the data from.</param>
        /// <param name="nonLock">Whether the deserializer should release the main thread when the deserialization is taking too long.</param>
        /// <param name="userData">User data to pass to the callbacks.</param>
        /// <param name="onFinish">The method to execute when the deserialization finishes.</param>
        /// <param name="deserializationCallback">Callback to execute for each deserialized object.</param>
        public static void DeserializeToGameObject(string filename,
        bool nonLock = false,
        object userData = null,
        Action<SerializationContext> onFinish = null,
        Action<SerializationContext, UnityEngine.Object> deserializationCallback = null
        )
        {
            Debug.Assert(File.Exists(filename));
            var stream = File.OpenRead(filename);
            DeserializeToStream(stream: stream,
            nonLock: nonLock,
            userData: userData,
            onFinish: onFinish,
            deserializationCallback: deserializationCallback);
        }

        /// <summary>
        /// Deserializes the data from the given Stream into the given GameObject.
        /// </summary>
        /// <param name="stream">The Stream containing the GameObject data.</param>
        /// <param name="nonLock">Whether the deserializer should release the main thread when the deserialization is taking too long.</param>
        /// <param name="userData">User data to pass to the callbacks.</param>
        /// <param name="onFinish">The method to execute when the deserialization finishes.</param>
        /// <param name="deserializationCallback">Callback to execute for each deserialized object.</param>
        public static void DeserializeToStream( Stream stream,
                                                bool nonLock = false,
                                                object userData = null,
                                                Action<SerializationContext> onFinish = null,
                                                Action<SerializationContext, UnityEngine.Object> deserializationCallback = null
        )
        {
            Initialize();
            var enumerator = DeserializeInternal(stream: stream,
            onFinish: onFinish,
            userData: userData,
            nonLock: nonLock,
            deserializationCallback: deserializationCallback
            );
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

        /// <summary>
        /// Serializes the given GameObject as the given filename.
        /// </summary>
        /// <param name="filename">The filename where to save the serialized GameObject.</param>
        /// <param name="gameObject">The GameObject to serialize.</param>
        /// <param name="nonLock">Whether the serializer should release the main thread when the serialization is taking too long.</param>
        /// <param name="onHash">Pass a callback here to receive the SHA-256 hash generated from the serialized contents.</param>
        /// <param name="userData">User data to pass to the callbacks.</param>
        /// <param name="onFinish">The method to execute when the serialization finishes.</param>
        /// <param name="serializationCallback">Use this callback to store custom information into the serialized objects data.</param>
        /// <param name="compress">Whether to compress the output file.</param>
        public static void SerializeToFile( string filename,
                                            GameObject gameObject,
                                            bool nonLock = false,
                                            object userData = null,
                                            Action<SerializationContext> onFinish = null,
                                            Action<SerializationContext, string> onHash = null,
                                            Action<SerializationContext, UnityEngine.Object> serializationCallback = null,
                                            bool compress = false
        )
        {
            Debug.Assert(FileSystemHelper.IsPathValid(filename));
            var stream = File.Create(filename);
            SerializeToStream(stream: stream,
            gameObject: gameObject,
            nonLock: nonLock,
            userData: userData,
            onFinish: onFinish,
            onHash: onHash,
            serializationCallback: serializationCallback,
            compress: compress);
        }

        /// <summary>
        /// Serializes the given GameObject into the given Stream.
        /// </summary>
        /// <param name="stream">The Stream to serialize the GameObject into.</param>
        /// <param name="gameObject">The GameObject to serialize.</param>
        /// <param name="nonLock">Whether the serializer should release the main thread when the serialization is taking too long.</param>
        /// <param name="onHash">Pass a callback here to receive the SHA-256 hash generated from the serialized contents.</param>
        /// <param name="userData">User data to pass to the callbacks.</param>
        /// <param name="onFinish">The method to execute when the serialization finishes.</param>
        /// <param name="serializationCallback">Use this callback to store custom information into the serialized objects data.</param>
        /// <param name="compress">Whether to compress the output Stream.</param>
        public static void SerializeToStream(   Stream stream,
                                                GameObject gameObject,
                                                bool nonLock = false,
                                                object userData = null,
                                                Action<SerializationContext> onFinish = null,
                                                Action<SerializationContext, string> onHash = null,
                                                Action<SerializationContext, UnityEngine.Object> serializationCallback = null,
                                                bool compress = false
        )
        {
            Initialize();
            IEnumerator enumerator;
            if (compress)
            {
                var binaryWriter = new BinaryWriter(stream, System.Text.Encoding.Default, true);
                binaryWriter.Write(Shared.CompressedIdentifier);
                binaryWriter.Close();
                var zipStream = new GZipOutputStream(stream);
                enumerator = SerializeInternal(stream: zipStream,
                gameObject: gameObject,
                userData: userData,
                onFinish: onFinish,
                onHash: onHash,
                serializationCallback: serializationCallback,
                nonLock: nonLock,
                compress: compress);
            }
            else
            {
                var binaryWriter = new BinaryWriter(stream, System.Text.Encoding.Default, true);
                binaryWriter.Write(Shared.Identifier);
                binaryWriter.Close();
                enumerator = SerializeInternal(stream: stream,
                gameObject: gameObject,
                userData: userData,
                onFinish: onFinish,
                onHash: onHash,
                serializationCallback: serializationCallback,
                nonLock: nonLock);
            }
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

        /// <summary>
        /// Collects resources associated with the specified Unity object for serialization.
        /// </summary>
        /// <param name="serializationContext">The context for serialization operations.</param>
        /// <param name="source">The Unity object to collect resources from.</param>
        /// <returns>An enumerable sequence of yielded items during resource collection, including pauses for non-locking behavior.</returns>
        private static IEnumerable CollectResources(SerializationContext serializationContext, UnityEngine.Object source)
        {
            if (ObjectSerializers.TryGetValue(source.GetType(), out var serializer))
            {
                foreach (var item in serializer.CollectResources(serializationContext, source))
                {
                    yield return item;
                }
                foreach (var pause in serializationContext.ReleaseMainThread())
                {
                    yield return pause;
                }
            }
        }

        /// <summary>
        /// Internal method to deserialize data from a stream into a GameObject.
        /// </summary>
        /// <param name="stream">The stream containing the serialized data.</param>
        /// <param name="onFinish">Callback to execute when deserialization completes.</param>
        /// <param name="userData">User data to pass to the callbacks.</param>
        /// <param name="nonLock">Whether to release the main thread during deserialization.</param>
        /// <param name="deserializationCallback">Callback to execute for each deserialized object.</param>
        /// <returns>An enumerator for the deserialization process, yielding control for coroutines or synchronous execution.</returns>
        private static IEnumerator DeserializeInternal( Stream stream,
                                                        Action<SerializationContext> onFinish,
                                                        object userData = null,
                                                        bool nonLock = false,
                                                        Action<SerializationContext, UnityEngine.Object> deserializationCallback = null
        )
        {
            var binaryReader = new BinaryReader(stream, System.Text.Encoding.Default, false);
            var serializationContext = new SerializationContext(nonLock);
            serializationContext.UserData = userData;
            serializationContext.DeserializationCallback = deserializationCallback;
            var identifier = binaryReader.ReadIdentifier();
            if (identifier == Shared.CompressedIdentifier)
            {
                var zipStream = new GZipInputStream(stream);
                binaryReader = new BinaryReader(zipStream, System.Text.Encoding.Default, false);
            }
            else
            {
                Debug.Assert(identifier == Shared.Identifier);
            }
            serializationContext.BinaryReader = binaryReader;
            serializationContext.Version = binaryReader.ReadAndAssert(Shared.Version1);
            while (binaryReader.BaseStream.CanRead)
            {
                try
                {
                    identifier = binaryReader.ReadIdentifier();
                }
                catch (Exception exception)
                {
                    if (exception is EndOfStreamException)
                    {
                        goto Finish;
                    }
                    throw exception;
                }
                if (ObjectDeserializers.TryGetValue(identifier, out var deserializer))
                {
                    foreach (var item in deserializer.Deserialize(serializationContext))
                    {
                        yield return item;
                    }
                    foreach (var pause in serializationContext.ReleaseMainThread())
                    {
                        yield return pause;
                    }
                }
                else
                {
                    Debug.LogError("Unknown serializer:" + deserializer.Identifier);
                }
            }
            Finish:
            binaryReader.Close();
            if (onFinish != null)
            {
                onFinish(serializationContext);
            }
            yield break;
        }

        /// <summary>
        /// Serializes a single Unity object using its registered serializer.
        /// </summary>
        /// <param name="serializationContext">The context for serialization operations.</param>
        /// <param name="source">The Unity object to serialize.</param>
        /// <returns>An enumerable sequence of yielded items during serialization.</returns>
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

        /// <summary>
        /// Internal method to serialize a GameObject and its components into a stream.
        /// </summary>
        /// <param name="stream">The stream to write the serialized data to.</param>
        /// <param name="gameObject">The GameObject to serialize.</param>
        /// <param name="userData">User data to pass to the callbacks.</param>
        /// <param name="onFinish">Callback to execute when serialization completes.</param>
        /// <param name="onHash">Callback to receive the SHA-256 hash of the serialized data.</param>
        /// <param name="serializationCallback">Callback to execute for each serialized object.</param>
        /// <param name="nonLock">Whether to release the main thread during serialization.</param>
        /// <param name="compress">Whether the output stream is compressed.</param>
        /// <returns>An enumerator for the serialization process, yielding control for coroutines or synchronous execution.</returns>
        private static IEnumerator SerializeInternal(   Stream stream,
                                                        GameObject gameObject,
                                                        object userData = null,
                                                        Action<SerializationContext> onFinish = null,
                                                        Action<SerializationContext, string> onHash = null,
                                                        Action<SerializationContext, UnityEngine.Object> serializationCallback = null,
                                                        bool nonLock = false,
                                                        bool compress = false
        )
        {
            Debug.Assert(gameObject != null);
            var binaryWriter = new BinaryWriter(stream, System.Text.Encoding.Default, false);
            var serializationContext = new SerializationContext(nonLock);
            serializationContext.UserData = userData;
            serializationContext.SerializationCallback = serializationCallback;
            binaryWriter.Write(Shared.Version1);
            serializationContext.BinaryWriter = binaryWriter;
            serializationContext.Version = Shared.Version1;
            var components = gameObject.GetComponentsInChildren<Component>();
            foreach (var component in components)
            {
                if (component is Transform transform)
                {
                    foreach (var item in SerializeObject(serializationContext, transform.gameObject))
                    {
                        yield return item;
                    }
                    foreach (var pause in serializationContext.ReleaseMainThread())
                    {
                        yield return pause;
                    }

                    foreach (var item in SerializeObject(serializationContext, transform))
                    {
                        yield return item;
                    }
                    foreach (var pause in serializationContext.ReleaseMainThread())
                    {
                        yield return pause;
                    }
                }
            }
            foreach (var component in components)
            {
                foreach (var item in CollectResources(serializationContext, component))
                {
                    yield return item;
                }
                foreach (var pause in serializationContext.ReleaseMainThread())
                {
                    yield return pause;
                }
            }
            foreach (var resource in serializationContext.Resources)
            {
                foreach (var item in SerializeObject(serializationContext, resource))
                {
                    yield return item;
                }
                foreach (var pause in serializationContext.ReleaseMainThread())
                {
                    yield return pause;
                }
            }
            foreach (var component in components)
            {
                if (component is Transform)
                {
                    continue;
                }
                foreach (var item in SerializeObject(serializationContext, component))
                {
                    yield return item;
                }
                foreach (var pause in serializationContext.ReleaseMainThread())
                {
                    yield return pause;
                }
            }
            if (onHash != null)
            {
                if (!compress)
                {
                    var hash = FileSystemHelper.CalculateSHA256(binaryWriter.BaseStream);
                    onHash(serializationContext, hash);
                }
                else
                {
                    Debug.LogWarning("Can't create hash for compressed stream.");
                }
            }
            binaryWriter.Close();
            if (onFinish != null)
            {
                onFinish(serializationContext);
            }
        }
    }
}