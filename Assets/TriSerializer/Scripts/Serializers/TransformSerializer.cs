using System.Collections;
using UnityEngine;

namespace TriSerializer
{
    /// <summary>
    /// A serializer for Unity <see cref="Transform"/> components, handling serialization and deserialization of transform properties.
    /// Inherits from <see cref="ComponentSerializer{Transform}"/>.
    /// </summary>
    public class TransformSerializer : ComponentSerializer<Transform>
    {
        /// <summary>
        /// Gets the unique identifier for the Transform serializer.
        /// </summary>
        /// <value>
        /// An <see cref="ObjectIdentifier"/> with the value "TRF".
        /// </value>
        public override ObjectIdentifier Identifier => "TRF";

        /// <summary>
        /// Deserializes a <see cref="Transform"/> component from the provided serialization context, including its parent and properties.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary reader and object references for deserialization.</param>
        /// <returns>An enumerable sequence of yielded items during deserialization.</returns>
        public override IEnumerable Deserialize(SerializationContext serializationContext)
        {
            foreach (var item in base.Deserialize(serializationContext))
            {
                yield return item;
            }
            var transform = serializationContext.GameObject.transform;
            serializationContext.SetupDestination(transform);
            var parent = serializationContext.ReadReference<Transform>();
            transform.SetParent(parent, false);
            transform.hasChanged = serializationContext.BinaryReader.Read(transform.hasChanged);
            transform.hierarchyCapacity = serializationContext.BinaryReader.Read(transform.hierarchyCapacity);
            transform.localPosition = serializationContext.BinaryReader.Read(transform.localPosition);
            transform.localRotation = serializationContext.BinaryReader.Read(transform.localRotation);
            transform.localScale = serializationContext.BinaryReader.Read(transform.localScale);
            DeserializationCallback(serializationContext, transform);
            yield break;
        }

        /// <summary>
        /// Serializes a <see cref="Transform"/> component to the provided serialization context, including its parent and properties.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary writer for serialization.</param>
        /// <param name="source">The <see cref="Transform"/> to serialize.</param>
        /// <returns>An enumerable sequence of yielded items during serialization.</returns>
        public override IEnumerable Serialize(SerializationContext serializationContext, Transform source)
        {
            foreach (var item in base.Serialize(serializationContext, source))
            {
                yield return item;
            }
            serializationContext.WriteReference(source.parent);
            serializationContext.BinaryWriter.Write(source.hasChanged);
            serializationContext.BinaryWriter.Write(source.hierarchyCapacity);
            serializationContext.BinaryWriter.Write(source.localPosition);
            serializationContext.BinaryWriter.Write(source.localRotation);
            serializationContext.BinaryWriter.Write(source.localScale);
            SerializationCallback(serializationContext, source);
            yield break;
        }
    }
}