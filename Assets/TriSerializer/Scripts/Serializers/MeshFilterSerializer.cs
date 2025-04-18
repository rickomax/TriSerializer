using System.Collections;
using UnityEngine;

namespace TriSerializer
{
    /// <summary>
    /// A serializer for Unity <see cref="MeshFilter"/> components, handling serialization and deserialization of mesh references.
    /// Inherits from <see cref="ComponentSerializer{MeshFilter}"/>.
    /// </summary>
    public class MeshFilterSerializer : ComponentSerializer<MeshFilter>
    {
        /// <summary>
        /// Gets the unique identifier for the MeshFilter serializer.
        /// </summary>
        /// <value>
        /// An <see cref="ObjectIdentifier"/> with the value "MFL".
        /// </value>
        public override ObjectIdentifier Identifier => "MFL";

        /// <summary>
        /// Collects resources (meshes) associated with the MeshFilter for serialization.
        /// </summary>
        /// <param name="serializationContext">The context for serialization operations.</param>
        /// <param name="source">The <see cref="MeshFilter"/> to collect resources from.</param>
        /// <returns>An enumerable sequence of yielded items during resource collection.</returns>
        public override IEnumerable CollectResources(SerializationContext serializationContext, MeshFilter source)
        {
            foreach (var item in base.CollectResources(serializationContext, source))
            {
                yield return item;
            }
#if USE_SHARED
serializationContext.AddResource(source.sharedMesh);
#endif
            serializationContext.AddResource(source.mesh);
            yield break;
        }

        /// <summary>
        /// Deserializes a <see cref="MeshFilter"/> component from the provided serialization context, including its mesh references.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary reader and object references for deserialization.</param>
        /// <returns>An enumerable sequence of yielded items during deserialization.</returns>
        public override IEnumerable Deserialize(SerializationContext serializationContext)
        {
            foreach (var item in base.Deserialize(serializationContext))
            {
                yield return item;
            }
            var meshFilter = serializationContext.GameObject.AddComponent<MeshFilter>();
            serializationContext.SetupDestination(meshFilter);
#if USE_SHARED
meshFilter.sharedMesh = serializationContext.ReadReference(meshFilter.sharedMesh);
#endif
            meshFilter.mesh = serializationContext.ReadReference(meshFilter.mesh);
            DeserializationCallback(serializationContext, meshFilter);
            yield break;
        }

        /// <summary>
        /// Serializes a <see cref="MeshFilter"/> component to the provided serialization context, including its mesh references.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary writer for serialization.</param>
        /// <param name="source">The <see cref="MeshFilter"/> to serialize.</param>
        /// <returns>An enumerable sequence of yielded items during serialization.</returns>
        public override IEnumerable Serialize(SerializationContext serializationContext, MeshFilter source)
        {
            foreach (var item in base.Serialize(serializationContext, source))
            {
                yield return item;
            }
#if USE_SHARED
serializationContext.WriteReference(source.sharedMesh);
#endif
            serializationContext.WriteReference(source.mesh);
            SerializationCallback(serializationContext, source);
            yield break;
        }
    }
}