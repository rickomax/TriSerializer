using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace TriSerializer
{
    /// <summary>
    /// A serializer for Unity <see cref="Material"/> objects, handling serialization and deserialization of material properties and resources.
    /// Inherits from <see cref="ObjectSerializer{Material}"/>.
    /// </summary>
    public class MaterialSerializer : ObjectSerializer<Material>
    {
        /// <summary>
        /// Gets the unique identifier for the Material serializer.
        /// </summary>
        /// <value>
        /// An <see cref="ObjectIdentifier"/> with the value "MAT".
        /// </value>
        public override ObjectIdentifier Identifier => "MAT";

        /// <summary>
        /// Collects resources (textures) associated with the material for serialization.
        /// </summary>
        /// <param name="serializationContext">The context for serialization operations.</param>
        /// <param name="source">The <see cref="Material"/> to collect resources from.</param>
        /// <returns>An enumerable sequence of yielded items during resource collection.</returns>
        public override IEnumerable CollectResources(SerializationContext serializationContext, Material source)
        {
            var textureProperties = source.GetPropertyNames(MaterialPropertyType.Texture);
            for (var i = 0; i < textureProperties.Length; i++)
            {
                var property = Shader.PropertyToID(textureProperties[i]);
                var texture = source.GetTexture(property);
                serializationContext.AddResource(texture);
            }
            yield break;
        }

        /// <summary>
        /// Deserializes a <see cref="Material"/> from the provided serialization context, including its properties and resources.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary reader and object references for deserialization.</param>
        /// <returns>An enumerable sequence of yielded items during deserialization.</returns>
        public override IEnumerable Deserialize(SerializationContext serializationContext)
        {
            foreach (var item in base.Deserialize(serializationContext))
            {
                yield return item;
            }
            var shaderName = serializationContext.BinaryReader.ReadString();
            var shader = Shader.Find(shaderName);
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }
            var material = new Material(shader);
            material.name = serializationContext.BinaryReader.Read(material.name);

            material.doubleSidedGI = serializationContext.BinaryReader.Read(material.doubleSidedGI);
            var enabledKeywordsLength = serializationContext.BinaryReader.ReadInt32();
            var enabledkeyWords = new LocalKeyword[enabledKeywordsLength];
            for (var i = 0; i < enabledKeywordsLength; i++)
            {
                var keyword = serializationContext.BinaryReader.ReadString();
                enabledkeyWords[i] = new LocalKeyword(shader, keyword);
            }

            material.enabledKeywords = enabledkeyWords;
            material.enableInstancing = serializationContext.BinaryReader.Read(material.enableInstancing);
            material.globalIlluminationFlags = serializationContext.BinaryReader.ReadEnum(material.globalIlluminationFlags);
            material.renderQueue = serializationContext.BinaryReader.Read(material.renderQueue);

            var floatPropertiesCount = serializationContext.BinaryReader.ReadInt32();
            for (var i = 0; i < floatPropertiesCount; i++)
            {
                var property = serializationContext.BinaryReader.ReadString();
                float value = default;
                value = serializationContext.BinaryReader.Read(value);
                material.SetFloat(property, value);
            }

            var intPropertiesCount = serializationContext.BinaryReader.ReadInt32();
            for (var i = 0; i < intPropertiesCount; i++)
            {
                var property = serializationContext.BinaryReader.ReadString();
                int value = default;
                value = serializationContext.BinaryReader.Read(value);
                material.SetInteger(property, value);
            }

            var vectorPropertiesCount = serializationContext.BinaryReader.ReadInt32();
            for (var i = 0; i < vectorPropertiesCount; i++)
            {
                var property = serializationContext.BinaryReader.ReadString();
                Vector4 value = default;
                value = serializationContext.BinaryReader.Read(value);
                material.SetVector(property, value);
            }

            var matrixPropertiesCount = serializationContext.BinaryReader.ReadInt32();
            for (var i = 0; i < matrixPropertiesCount; i++)
            {
                var property = serializationContext.BinaryReader.ReadString();
                Matrix4x4 value = default;
                value = serializationContext.BinaryReader.Read(value);
                material.SetMatrix(property, value);
            }

            var texturePropertiesCount = serializationContext.BinaryReader.ReadInt32();
            for (var i = 0; i < texturePropertiesCount; i++)
            {
                var property = serializationContext.BinaryReader.ReadString();
                Texture value = default;
                value = serializationContext.ReadReference(value);
                material.SetTexture(property, value);
            }
            serializationContext.AddObject(serializationContext.InstanceId, material);
            DeserializationCallback(serializationContext, material);
            yield break;
        }

        /// <summary>
        /// Serializes a <see cref="Material"/> to the provided serialization context, including its properties and resources.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary writer for serialization.</param>
        /// <param name="source">The <see cref="Material"/> to serialize.</param>
        /// <returns>An enumerable sequence of yielded items during serialization.</returns>
        public override IEnumerable Serialize(SerializationContext serializationContext, Material source)
        {
            foreach (var item in base.Serialize(serializationContext, source))
            {
                yield return item;
            }
            var shader = source.shader;
            Debug.Assert(shader != null);
            serializationContext.BinaryWriter.Write(shader.name);
            serializationContext.BinaryWriter.Write(source.name);

            serializationContext.BinaryWriter.Write(source.doubleSidedGI);
            var enabledKeywords = source.enabledKeywords;
            serializationContext.BinaryWriter.Write(enabledKeywords.Length);
            for (var i = 0; i < enabledKeywords.Length; i++)
            {
                serializationContext.BinaryWriter.Write(enabledKeywords[i].name);
            }

            serializationContext.BinaryWriter.Write(source.enableInstancing);
            serializationContext.BinaryWriter.Write((int)source.globalIlluminationFlags);
            serializationContext.BinaryWriter.Write(source.renderQueue);

            var floatProperties = source.GetPropertyNames(MaterialPropertyType.Float);
            serializationContext.BinaryWriter.Write(floatProperties.Length);
            for (var i = 0; i < floatProperties.Length; i++)
            {
                var property = floatProperties[i];
                serializationContext.BinaryWriter.Write(property);
                serializationContext.BinaryWriter.Write(source.GetFloat(property));
            }

            var intProperties = source.GetPropertyNames(MaterialPropertyType.Int);
            serializationContext.BinaryWriter.Write(intProperties.Length);
            for (var i = 0; i < intProperties.Length; i++)
            {
                var property = intProperties[i];
                serializationContext.BinaryWriter.Write(property);
                serializationContext.BinaryWriter.Write(source.GetInteger(property));
            }

            var vectorProperties = source.GetPropertyNames(MaterialPropertyType.Vector);
            serializationContext.BinaryWriter.Write(vectorProperties.Length);
            for (var i = 0; i < vectorProperties.Length; i++)
            {
                var property = vectorProperties[i];
                serializationContext.BinaryWriter.Write(property);
                serializationContext.BinaryWriter.Write(source.GetVector(property));
            }

            var matrixProperties = source.GetPropertyNames(MaterialPropertyType.Matrix);
            serializationContext.BinaryWriter.Write(matrixProperties.Length);
            for (var i = 0; i < matrixProperties.Length; i++)
            {
                var property = matrixProperties[i];
                serializationContext.BinaryWriter.Write(property);
                serializationContext.BinaryWriter.Write(source.GetMatrix(property));
            }

            var textureProperties = source.GetPropertyNames(MaterialPropertyType.Texture);
            serializationContext.BinaryWriter.Write(textureProperties.Length);
            for (var i = 0; i < textureProperties.Length; i++)
            {
                var property = textureProperties[i];
                serializationContext.BinaryWriter.Write(property);
                serializationContext.WriteReference(source.GetTexture(property));
            }
            SerializationCallback(serializationContext, source);

            yield break;
        }
    }
}