using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace TriSerializer
{
    /// <summary>
    /// A serializer for Unity <see cref="Texture2D"/> objects, handling serialization and deserialization of texture data and properties.
    /// Inherits from <see cref="ObjectSerializer{Texture2D}"/>.
    /// </summary>
    public class TextureSerializer2D : ObjectSerializer<Texture2D>
    {
        /// <summary>
        /// Gets the unique identifier for the Texture2D serializer.
        /// </summary>
        /// <value>
        /// An <see cref="ObjectIdentifier"/> with the value "T2D".
        /// </value>
        public override ObjectIdentifier Identifier => "T2D";

        /// <summary>
        /// Deserializes a <see cref="Texture2D"/> from the provided serialization context, reconstructing its properties and raw texture data.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary reader and object references for deserialization.</param>
        /// <returns>An enumerable sequence of yielded items during deserialization.</returns>
        public override IEnumerable Deserialize(SerializationContext serializationContext)
        {
            foreach (var item in base.Deserialize(serializationContext))
            {
                yield return item;
            }
            var isReadable = serializationContext.BinaryReader.ReadBoolean();
            if (!isReadable)
            {
                yield break;
            }
            var width = serializationContext.BinaryReader.ReadInt32();
            var height = serializationContext.BinaryReader.ReadInt32();
            GraphicsFormat format = default;
            format = serializationContext.BinaryReader.ReadEnum(format);
            TextureCreationFlags textureCreationFlags = default;
            textureCreationFlags = serializationContext.BinaryReader.ReadEnum(textureCreationFlags);
            var texture2D = new Texture2D(width, height, format, textureCreationFlags);
            texture2D.name = serializationContext.BinaryReader.Read(texture2D.name);
            texture2D.anisoLevel = serializationContext.BinaryReader.Read(texture2D.anisoLevel);
            texture2D.filterMode = serializationContext.BinaryReader.ReadEnum(texture2D.filterMode);
            texture2D.ignoreMipmapLimit = serializationContext.BinaryReader.Read(texture2D.ignoreMipmapLimit);
            texture2D.minimumMipmapLevel = serializationContext.BinaryReader.Read(texture2D.minimumMipmapLevel);
            texture2D.mipMapBias = serializationContext.BinaryReader.Read(texture2D.mipMapBias);
            texture2D.requestedMipmapLevel = serializationContext.BinaryReader.Read(texture2D.requestedMipmapLevel);
            texture2D.wrapMode = serializationContext.BinaryReader.ReadEnum(texture2D.wrapMode);
            texture2D.wrapModeU = serializationContext.BinaryReader.ReadEnum(texture2D.wrapModeU);
            texture2D.wrapModeV = serializationContext.BinaryReader.ReadEnum(texture2D.wrapModeV);
            texture2D.wrapModeW = serializationContext.BinaryReader.ReadEnum(texture2D.wrapModeW);
            var dataLength = serializationContext.BinaryReader.ReadInt32();
            var nativeArray = serializationContext.GetNewNativeArray<byte>(dataLength);
            serializationContext.BinaryReader.Read(nativeArray);
            texture2D.LoadRawTextureData(nativeArray);
            texture2D.Apply(false, false);
            nativeArray.Dispose();
            serializationContext.AddObject(serializationContext.InstanceId, texture2D);
            DeserializationCallback(serializationContext, texture2D);
            yield break;
        }

        /// <summary>
        /// Serializes a <see cref="Texture2D"/> to the provided serialization context, including its properties and raw texture data.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary writer for serialization.</param>
        /// <param name="source">The <see cref="Texture2D"/> to serialize.</param>
        /// <returns>An enumerable sequence of yielded items during serialization.</returns>
        public override IEnumerable Serialize(SerializationContext serializationContext, Texture2D source)
        {
            foreach (var item in base.Serialize(serializationContext, source))
            {
                yield return item;
            }
            if (!source.isReadable)
            {
                Debug.LogError($"Texture '{source}' can't be serialized. It is not readable.");
                serializationContext.BinaryWriter.Write(false);
                yield break;
            }
            else
            {
                serializationContext.BinaryWriter.Write(true);
            }
            serializationContext.BinaryWriter.Write(source.width);
            serializationContext.BinaryWriter.Write(source.height);
            serializationContext.BinaryWriter.Write((int)source.graphicsFormat);
            serializationContext.BinaryWriter.Write((int)GetTextureCreationFlags(source));
            serializationContext.BinaryWriter.Write(source.name);
            serializationContext.BinaryWriter.Write(source.anisoLevel);
            serializationContext.BinaryWriter.Write((int)source.filterMode);
            serializationContext.BinaryWriter.Write(source.ignoreMipmapLimit);
            serializationContext.BinaryWriter.Write(source.minimumMipmapLevel);
            serializationContext.BinaryWriter.Write(source.mipMapBias);
            serializationContext.BinaryWriter.Write(source.requestedMipmapLevel);
            serializationContext.BinaryWriter.Write((int)source.wrapMode);
            serializationContext.BinaryWriter.Write((int)source.wrapModeU);
            serializationContext.BinaryWriter.Write((int)source.wrapModeV);
            serializationContext.BinaryWriter.Write((int)source.wrapModeW);
            var textureData = source.GetRawTextureData<byte>();
            serializationContext.BinaryWriter.Write(textureData.Length);
            serializationContext.BinaryWriter.Write(textureData);
            SerializationCallback(serializationContext, source);
            yield break;
        }

        /// <summary>
        /// Determines the <see cref="TextureCreationFlags"/> for the given <see cref="Texture2D"/> based on its properties.
        /// </summary>
        /// <param name="texture">The <see cref="Texture2D"/> to analyze.</param>
        /// <returns>The computed <see cref="TextureCreationFlags"/> for the texture.</returns>
        private static TextureCreationFlags GetTextureCreationFlags(Texture2D texture)
        {
            TextureCreationFlags flags = 0;
            if (texture.mipmapCount > 1)
            {
                flags |= TextureCreationFlags.MipChain;
            }
            if (GraphicsFormatUtility.IsCrunchFormat(texture.format))
            {
                flags |= TextureCreationFlags.Crunch;
            }
            return flags;
        }
    }
}