using System.Collections;
using UnityEngine;

namespace TriSerializer
{
    /// <summary>
    /// An abstract base class for serializing and deserializing Unity components of type <typeparamref name="T"/>.
    /// Inherits from <see cref="ObjectSerializer{T}"/> and constrains <typeparamref name="T"/> to be a <see cref="Component"/>.
    /// </summary>
    /// <typeparam name="T">The type of the component to serialize, which must derive from <see cref="Component"/>.</typeparam>
    public abstract class ComponentSerializer<T> : ObjectSerializer<T> where T : Component
    {
        /// <summary>
        /// Deserializes a component from the provided serialization context, including the associated GameObject ID.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary reader and object references for deserialization.</param>
        /// <returns>An enumerable sequence of yielded items during deserialization.</returns>
        public override IEnumerable Deserialize(SerializationContext serializationContext)
        {
            foreach (var item in base.Deserialize(serializationContext))
            {
                yield return item;
            }
            var gameObjectId = serializationContext.BinaryReader.ReadInt32();
            serializationContext.GameObject = serializationContext.GetObject<GameObject>(gameObjectId);
            yield break;
        }

        /// <summary>
        /// Serializes a component to the provided serialization context, including the associated GameObject's instance ID.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary writer for serialization.</param>
        /// <param name="source">The component to serialize.</param>
        /// <returns>An enumerable sequence of yielded items during serialization.</returns>
        public override IEnumerable Serialize(SerializationContext serializationContext, T source)
        {
            foreach (var item in base.Serialize(serializationContext, source))
            {
                yield return item;
            }
            serializationContext.BinaryWriter.Write(source.gameObject.GetInstanceID());
            yield break;
        }
    }
}