using System.Collections;
using UnityEngine;

namespace TriSerializer
{
    /// <summary>
    /// A serializer for Unity <see cref="Renderer"/> components of type <typeparamref name="T"/>, handling serialization and deserialization of renderer properties and materials.
    /// Inherits from <see cref="ComponentSerializer{T}"/> and constrains <typeparamref name="T"/> to be a <see cref="Renderer"/>.
    /// </summary>
    /// <typeparam name="T">The type of the renderer to serialize, which must derive from <see cref="Renderer"/>.</typeparam>
    public class RendererSerializer<T> : ComponentSerializer<T> where T : Renderer
    {
        /// <summary>
        /// Collects resources (materials) associated with the renderer for serialization.
        /// </summary>
        /// <param name="serializationContext">The context for serialization operations.</param>
        /// <param name="source">The <see cref="Renderer"/> to collect resources from.</param>
        /// <returns>An enumerable sequence of yielded items during resource collection.</returns>
        public override IEnumerable CollectResources(SerializationContext serializationContext, T source)
        {
            var materialSerializer = Serializer.ObjectSerializers[typeof(Material)];
#if USE_SHARED
var sharedMaterials = source.sharedMaterials;
if (sharedMaterials != null)
{
foreach (var sharedMaterial in sharedMaterials)
{
foreach (var item in materialSerializer.CollectResources(serializationContext, sharedMaterial))
{
yield return item;
}
serializationContext.AddResource(sharedMaterial);
}
}
#endif
            var materials = source.materials;
            if (materials != null)
            {
                foreach (var material in materials)
                {
                    foreach (var item in materialSerializer.CollectResources(serializationContext, material))
                    {
                        yield return item;
                    }
                    serializationContext.AddResource(material);
                }
            }
            yield break;
        }

        /// <summary>
        /// Deserializes renderer-specific properties and materials for the specified <see cref="Renderer"/> destination.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary reader and object references for deserialization.</param>
        /// <param name="destination">The <see cref="Renderer"/> to deserialize properties into.</param>
        /// <returns>An enumerable sequence of yielded items during deserialization.</returns>
        protected IEnumerable DeserializeRenderer(SerializationContext serializationContext, Renderer destination)
        {
            destination.localBounds = serializationContext.BinaryReader.Read(destination.localBounds);
            destination.allowOcclusionWhenDynamic = serializationContext.BinaryReader.Read(destination.allowOcclusionWhenDynamic);
            destination.enabled = serializationContext.BinaryReader.Read(destination.enabled);
            destination.forceRenderingOff = serializationContext.BinaryReader.Read(destination.forceRenderingOff);
            destination.lightProbeProxyVolumeOverride = serializationContext.ReadReference(destination.lightProbeProxyVolumeOverride);
            destination.lightmapIndex = serializationContext.BinaryReader.Read(destination.lightmapIndex);
            destination.lightmapScaleOffset = serializationContext.BinaryReader.Read(destination.lightmapScaleOffset);
            destination.probeAnchor = serializationContext.ReadReference(destination.probeAnchor);
            destination.realtimeLightmapIndex = serializationContext.BinaryReader.Read(destination.realtimeLightmapIndex);
            destination.realtimeLightmapScaleOffset = serializationContext.BinaryReader.Read(destination.realtimeLightmapScaleOffset);
            destination.rendererPriority = serializationContext.BinaryReader.Read(destination.rendererPriority);
            destination.renderingLayerMask = serializationContext.BinaryReader.Read(destination.renderingLayerMask);
            destination.sortingLayerID = serializationContext.BinaryReader.Read(destination.sortingLayerID);
            destination.sortingLayerName = serializationContext.BinaryReader.Read(destination.sortingLayerName);
            destination.lightProbeUsage = serializationContext.BinaryReader.ReadEnum(destination.lightProbeUsage);
            destination.shadowCastingMode = serializationContext.BinaryReader.ReadEnum(destination.shadowCastingMode);
#if USE_SHARED
var sharedMaterialsCount = serializationContext.BinaryReader.ReadInt32();
if (sharedMaterialsCount > 0)
{
var sharedMaterials = new Material[sharedMaterialsCount];
for (var i = 0; i < sharedMaterialsCount; i++)
{
sharedMaterials[i] = serializationContext.ReadReference(sharedMaterials[i]);
}
destination.sharedMaterials = sharedMaterials;
}
#endif
            var materialsCount = serializationContext.BinaryReader.ReadInt32();
            if (materialsCount > 0)
            {
                var materials = new Material[materialsCount];
                for (var i = 0; i < materialsCount; i++)
                {
                    materials[i] = serializationContext.ReadReference(materials[i]);
                }
                destination.materials = materials;
            }
            yield break;
        }

        /// <summary>
        /// Serializes renderer-specific properties and materials for the specified <see cref="Renderer"/> source.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary writer for serialization.</param>
        /// <param name="source">The <see cref="Renderer"/> to serialize.</param>
        /// <returns>An enumerable sequence of yielded items during serialization.</returns>
        protected IEnumerable SerializeRenderer(SerializationContext serializationContext, T source)
        {
            serializationContext.BinaryWriter.Write(source.localBounds);
            serializationContext.BinaryWriter.Write(source.allowOcclusionWhenDynamic);
            serializationContext.BinaryWriter.Write(source.enabled);
            serializationContext.BinaryWriter.Write(source.forceRenderingOff);
            serializationContext.WriteReference(source.lightProbeProxyVolumeOverride);
            serializationContext.BinaryWriter.Write(source.lightmapIndex);
            serializationContext.BinaryWriter.Write(source.lightmapScaleOffset);
            serializationContext.WriteReference(source.probeAnchor);
            serializationContext.BinaryWriter.Write(source.realtimeLightmapIndex);
            serializationContext.BinaryWriter.Write(source.realtimeLightmapScaleOffset);
            serializationContext.BinaryWriter.Write(source.rendererPriority);
            serializationContext.BinaryWriter.Write(source.renderingLayerMask);
            serializationContext.BinaryWriter.Write(source.sortingLayerID);
            serializationContext.BinaryWriter.Write(source.sortingLayerName);
            serializationContext.BinaryWriter.Write((int)source.lightProbeUsage);
            serializationContext.BinaryWriter.Write((int)source.shadowCastingMode);
#if USE_SHARED
var sharedMaterials = source.sharedMaterials;
if (sharedMaterials != null)
{
serializationContext.BinaryWriter.Write(sharedMaterials.Length);
for (var i = 0; i < sharedMaterials.Length; i++)
{
serializationContext.WriteReference(sharedMaterials[i]);
}
}
else
{
serializationContext.BinaryWriter.Write(0);
}
#endif
            var materials = source.materials;
            if (materials != null)
            {
                serializationContext.BinaryWriter.Write(materials.Length);
                for (var i = 0; i < materials.Length; i++)
                {
                    serializationContext.WriteReference(materials[i]);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(0);
            }
            yield break;
        }
    }
}