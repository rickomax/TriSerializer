using System.Collections;
using UnityEngine;

namespace TriSerializer
{
    /// <summary>
    /// A serializer for Unity <see cref="GameObject"/> objects, handling serialization and deserialization of their properties.
    /// Inherits from <see cref="ObjectSerializer{GameObject}"/>.
    /// </summary>
    public class GameObjectSerializer : ObjectSerializer<GameObject>
    {
        /// <summary>
        /// Gets the unique identifier for the GameObject serializer.
        /// </summary>
        /// <value>
        /// An <see cref="ObjectIdentifier"/> with the value "GOB".
        /// </value>
        public override ObjectIdentifier Identifier => "GOB";

        /// <summary>
        /// Deserializes a <see cref="GameObject"/> from the provided serialization context, including its properties.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary reader and object references for deserialization.</param>
        /// <returns>An enumerable sequence of yielded items during deserialization.</returns>
        public override IEnumerable Deserialize(SerializationContext serializationContext)
        {
            foreach (var item in base.Deserialize(serializationContext))
            {
                yield return item;
            }
            var gameObject = new GameObject();
            serializationContext.SetupDestination(gameObject);
            gameObject.SetActive(serializationContext.BinaryReader.ReadBoolean());
            gameObject.isStatic = serializationContext.BinaryReader.Read(gameObject.isStatic);
            gameObject.layer = serializationContext.BinaryReader.Read(gameObject.layer);
            gameObject.tag = serializationContext.BinaryReader.Read(gameObject.tag);
            gameObject.name = serializationContext.BinaryReader.Read(gameObject.name);
            DeserializationCallback(serializationContext, gameObject);
            yield break;
        }

        /// <summary>
        /// Serializes a <see cref="GameObject"/> to the provided serialization context, including its properties.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary writer for serialization.</param>
        /// <param name="source">The <see cref="GameObject"/> to serialize.</param>
        /// <returns>An enumerable sequence of yielded items during serialization.</returns>
        public override IEnumerable Serialize(SerializationContext serializationContext, GameObject source)
        {
            foreach (var item in base.Serialize(serializationContext, source))
            {
                yield return item;
            }
            serializationContext.AddObject(source);
            serializationContext.BinaryWriter.Write(source.activeSelf);
            serializationContext.BinaryWriter.Write(source.isStatic);
            serializationContext.BinaryWriter.Write(source.layer);
            serializationContext.BinaryWriter.Write(source.tag);
            serializationContext.BinaryWriter.Write(source.name);
            SerializationCallback(serializationContext, source);
            yield break;
        }
    }
}