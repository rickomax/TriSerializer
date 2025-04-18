using System.Collections;
using Object = UnityEngine.Object;

namespace TriSerializer
{
    /// <summary>
    /// Defines an interface for serializing and deserializing Unity objects.
    /// </summary>
    public interface IObjectSerializer
    {
        /// <summary>
        /// Gets the unique identifier for the serializer.
        /// </summary>
        ObjectIdentifier Identifier { get; }

        /// <summary>
        /// Collects resources associated with the source object for serialization.
        /// </summary>
        /// <param name="serializationContext">The context for serialization operations.</param>
        /// <param name="source">The object to collect resources from.</param>
        /// <returns>An enumerable sequence of yielded items during resource collection.</returns>
        IEnumerable CollectResources(SerializationContext serializationContext, Object source);

        /// <summary>
        /// Deserializes an object from the provided serialization context.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary reader and object references for deserialization.</param>
        /// <returns>An enumerable sequence of yielded items during deserialization.</returns>
        IEnumerable Deserialize(SerializationContext serializationContext);

        /// <summary>
        /// Serializes an object to the provided serialization context.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary writer for serialization.</param>
        /// <param name="source">The object to serialize.</param>
        /// <returns>An enumerable sequence of yielded items during serialization.</returns>
        IEnumerable Serialize(SerializationContext serializationContext, Object source);
    }
}