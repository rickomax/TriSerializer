using System.Collections;
using UnityEngine;

namespace TriSerializer
{
    /// <summary>
    /// A serializer for Unity <see cref="SkinnedMeshRenderer"/> components, handling serialization and deserialization of renderer properties, bones, and mesh references.
    /// Inherits from <see cref="RendererSerializer{SkinnedMeshRenderer}"/>.
    /// </summary>
    public class SkinnedMeshRendererSerializer : RendererSerializer<SkinnedMeshRenderer>
    {
        /// <summary>
        /// Gets the unique identifier for the SkinnedMeshRenderer serializer.
        /// </summary>
        /// <value>
        /// An <see cref="ObjectIdentifier"/> with the value "SMR".
        /// </value>
        public override ObjectIdentifier Identifier => "SMR";

        /// <summary>
        /// Collects resources (shared mesh) associated with the SkinnedMeshRenderer for serialization.
        /// </summary>
        /// <param name="serializationContext">The context for serialization operations.</param>
        /// <param name="source">The <see cref="SkinnedMeshRenderer"/> to collect resources from.</param>
        /// <returns>An enumerable sequence of yielded items during resource collection.</returns>
        public override IEnumerable CollectResources(SerializationContext serializationContext, SkinnedMeshRenderer source)
        {
            foreach (var item in base.CollectResources(serializationContext, source))
            {
                yield return item;
            }
            serializationContext.AddResource(source.sharedMesh);
            yield break;
        }

        /// <summary>
        /// Deserializes a <see cref="SkinnedMeshRenderer"/> component from the provided serialization context, including its renderer properties, bones, and mesh.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary reader and object references for deserialization.</param>
        /// <returns>An enumerable sequence of yielded items during deserialization.</returns>
        public override IEnumerable Deserialize(SerializationContext serializationContext)
        {
            foreach (var item in base.Deserialize(serializationContext))
            {
                yield return item;
            }
            var skinnedMeshRenderer = serializationContext.GameObject.AddComponent<SkinnedMeshRenderer>();
            serializationContext.SetupDestination(skinnedMeshRenderer);
            foreach (var item in DeserializeRenderer(serializationContext, skinnedMeshRenderer))
            {
                yield return item;
            }
            var bonesCount = serializationContext.BinaryReader.ReadInt32();
            if (bonesCount > 0)
            {
                var bones = new Transform[bonesCount];
                for (var i = 0; i < bonesCount; i++)
                {
                    bones[i] = serializationContext.ReadReference(bones[i]);
                }
                skinnedMeshRenderer.bones = bones;
            }
            skinnedMeshRenderer.sharedMesh = serializationContext.ReadReference(skinnedMeshRenderer.sharedMesh);
            skinnedMeshRenderer.forceMatrixRecalculationPerRender = serializationContext.BinaryReader.Read(skinnedMeshRenderer.forceMatrixRecalculationPerRender);
            skinnedMeshRenderer.rootBone = serializationContext.ReadReference(skinnedMeshRenderer.rootBone);
            skinnedMeshRenderer.skinnedMotionVectors = serializationContext.BinaryReader.Read(skinnedMeshRenderer.skinnedMotionVectors);
            skinnedMeshRenderer.updateWhenOffscreen = serializationContext.BinaryReader.Read(skinnedMeshRenderer.updateWhenOffscreen);
            DeserializationCallback(serializationContext, skinnedMeshRenderer);
            yield break;
        }

        /// <summary>
        /// Serializes a <see cref="SkinnedMeshRenderer"/> component to the provided serialization context, including its renderer properties, bones, and mesh.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary writer for serialization.</param>
        /// <param name="source">The <see cref="SkinnedMeshRenderer"/> to serialize.</param>
        /// <returns>An enumerable sequence of yielded items during serialization.</returns>
        public override IEnumerable Serialize(SerializationContext serializationContext, SkinnedMeshRenderer source)
        {
            foreach (var item in base.Serialize(serializationContext, source))
            {
                yield return item;
            }
            foreach (var item in SerializeRenderer(serializationContext, source))
            {
                yield return item;
            }
            var bones = source.bones;
            if (bones != null)
            {
                serializationContext.BinaryWriter.Write(bones.Length);
                for (var i = 0; i < bones.Length; i++)
                {
                    serializationContext.WriteReference(bones[i]);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(0);
            }
            serializationContext.WriteReference(source.sharedMesh);
            serializationContext.BinaryWriter.Write(source.forceMatrixRecalculationPerRender);
            serializationContext.WriteReference(source.rootBone);
            serializationContext.BinaryWriter.Write(source.skinnedMotionVectors);
            serializationContext.BinaryWriter.Write(source.updateWhenOffscreen);
            SerializationCallback(serializationContext, source);
            yield break;
        }
    }
}