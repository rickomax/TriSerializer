using System.Collections;
using UnityEngine;

namespace TriSerializer
{
    /// <summary>
    /// A serializer for Unity <see cref="MeshRenderer"/> components, handling serialization and deserialization of renderer properties and resources.
    /// Inherits from <see cref="RendererSerializer{MeshRenderer}"/>.
    /// </summary>
    public class MeshRendererSerializer : RendererSerializer<MeshRenderer>
    {
        /// <summary>
        /// Gets the unique identifier for the MeshRenderer serializer.
        /// </summary>
        /// <value>
        /// An <see cref="ObjectIdentifier"/> with the value "MRD".
        /// </value>
        public override ObjectIdentifier Identifier => "MRD";

        /// <summary>
        /// Collects resources (vertex streams) associated with the MeshRenderer for serialization.
        /// </summary>
        /// <param name="serializationContext">The context for serialization operations.</param>
        /// <param name="source">The <see cref="MeshRenderer"/> to collect resources from.</param>
        /// <returns>An enumerable sequence of yielded items during resource collection.</returns>
        public override IEnumerable CollectResources(SerializationContext serializationContext, MeshRenderer source)
        {
            foreach (var item in base.CollectResources(serializationContext, source))
            {
                yield return item;
            }
            serializationContext.AddResource(source.additionalVertexStreams);
            serializationContext.AddResource(source.enlightenVertexStream);
            yield break;
        }

        /// <summary>
        /// Deserializes a <see cref="MeshRenderer"/> component from the provided serialization context, including its renderer properties and vertex streams.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary reader and object references for deserialization.</param>
        /// <returns>An enumerable sequence of yielded items during deserialization.</returns>
        public override IEnumerable Deserialize(SerializationContext serializationContext)
        {
            foreach (var item in base.Deserialize(serializationContext))
            {
                yield return item;
            }
            var meshRenderer = serializationContext.GameObject.AddComponent<MeshRenderer>();
            serializationContext.SetupDestination(meshRenderer);
            foreach (var item in DeserializeRenderer(serializationContext, meshRenderer))
            {
                yield return item;
            }
            meshRenderer.additionalVertexStreams = serializationContext.ReadReference(meshRenderer.additionalVertexStreams);
            meshRenderer.enlightenVertexStream = serializationContext.ReadReference(meshRenderer.enlightenVertexStream);
            DeserializationCallback(serializationContext, meshRenderer);
            yield break;
        }

        /// <summary>
        /// Serializes a <see cref="MeshRenderer"/> component to the provided serialization context, including its renderer properties and vertex streams.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary writer for serialization.</param>
        /// <param name="source">The <see cref="MeshRenderer"/> to serialize.</param>
        /// <returns>An enumerable sequence of yielded items during serialization.</returns>
        public override IEnumerable Serialize(SerializationContext serializationContext, MeshRenderer source)
        {
            foreach (var item in base.Serialize(serializationContext, source))
            {
                yield return item;
            }
            foreach (var item in SerializeRenderer(serializationContext, source))
            {
                yield return item;
            }
            serializationContext.WriteReference(source.additionalVertexStreams);
            serializationContext.WriteReference(source.enlightenVertexStream);
            SerializationCallback(serializationContext, source);
            yield break;
        }
    }
}