using System;
using System.Collections;
using Object = UnityEngine.Object;

namespace TriSerializer
{
    /// <summary>
    /// An abstract base class for serializing and deserializing Unity objects of type <typeparamref name="T"/>.
    /// Implements <see cref="IObjectSerializer"/> and constrains <typeparamref name="T"/> to be a <see cref="UnityEngine.Object"/>.
    /// </summary>
    /// <typeparam name="T">The type of the Unity object to serialize, which must derive from <see cref="UnityEngine.Object"/>.</typeparam>
    public abstract class ObjectSerializer<T> : IObjectSerializer where T : Object
    {
        /// <summary>
        /// Gets the unique identifier for the serializer.
        /// </summary>
        public virtual ObjectIdentifier Identifier { get; }

        /// <summary>
        /// Collects resources associated with the source object for serialization.
        /// </summary>
        /// <param name="serializationContext">The context for serialization operations.</param>
        /// <param name="source">The object to collect resources from.</param>
        /// <returns>An enumerable sequence of yielded items during resource collection.</returns>
        public virtual IEnumerable CollectResources(SerializationContext serializationContext, T source)
        {
            yield break;
        }

        /// <summary>
        /// Collects resources for the specified object, implementing the <see cref="IObjectSerializer"/> interface.
        /// </summary>
        /// <param name="serializationContext">The context for serialization operations.</param>
        /// <param name="source">The object to collect resources from.</param>
        /// <returns>An enumerable sequence of yielded items during resource collection.</returns>
        IEnumerable IObjectSerializer.CollectResources(SerializationContext serializationContext, Object source)
        {
            var enumerable = CollectResources(serializationContext, (T)source);
            var enumerator = enumerable.GetEnumerator();
            using var unknown = enumerator as IDisposable;
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        /// <summary>
        /// Deserializes an object from the provided serialization context, reading instance ID and hide flags.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary reader and object references for deserialization.</param>
        /// <returns>An enumerable sequence of yielded items during deserialization.</returns>
        public virtual IEnumerable Deserialize(SerializationContext serializationContext)
        {
            serializationContext.InstanceId = serializationContext.BinaryReader.Read(serializationContext.InstanceId);
            serializationContext.HideFlags = serializationContext.BinaryReader.ReadEnum(serializationContext.HideFlags);
            yield break;
        }

        /// <summary>
        /// Deserializes an object, implementing the <see cref="IObjectSerializer"/> interface.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary reader and object references for deserialization.</param>
        /// <returns>An enumerable sequence of yielded items during deserialization.</returns>
        IEnumerable IObjectSerializer.Deserialize(SerializationContext serializationContext)
        {
            var enumerable = Deserialize(serializationContext);
            foreach (var item in enumerable)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Invokes the serialization callback if provided in the context.
        /// </summary>
        /// <param name="serializationContext">The serialization context containing the callback.</param>
        /// <param name="source">The object being serialized.</param>
        protected void SerializationCallback(SerializationContext serializationContext, Object source)
        {
            if (serializationContext.SerializationCallback != null)
            {
                serializationContext.SerializationCallback(serializationContext, source);
            }
        }

        /// <summary>
        /// Invokes the deserialization callback if provided in the context.
        /// </summary>
        /// <param name="serializationContext">The serialization context containing the callback.</param>
        /// <param name="source">The object being deserialized.</param>
        protected void DeserializationCallback(SerializationContext serializationContext, Object source)
        {
            if (serializationContext.DeserializationCallback != null)
            {
                serializationContext.DeserializationCallback(serializationContext, source);
            }
        }

        /// <summary>
        /// Serializes an object to the provided serialization context, writing its identifier, instance ID, and hide flags.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary writer for serialization.</param>
        /// <param name="source">The object to serialize.</param>
        /// <returns>An enumerable sequence of yielded items during serialization.</returns>
        public virtual IEnumerable Serialize(SerializationContext serializationContext, T source)
        {
            var instanceID = source.GetInstanceID();
            serializationContext.BinaryWriter.Write(Identifier);
            serializationContext.BinaryWriter.Write(instanceID);
            serializationContext.BinaryWriter.Write((int)source.hideFlags);
            yield break;
        }

        /// <summary>
        /// Serializes an object, implementing the <see cref="IObjectSerializer"/> interface.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary writer for serialization.</param>
        /// <param name="source">The object to serialize.</param>
        /// <returns>An enumerable sequence of yielded items during serialization.</returns>
        IEnumerable IObjectSerializer.Serialize(SerializationContext serializationContext, Object source)
        {
            var enumerable = Serialize(serializationContext, (T)source);
            foreach (var item in enumerable)
            {
                yield return item;
            }
        }
    }
}