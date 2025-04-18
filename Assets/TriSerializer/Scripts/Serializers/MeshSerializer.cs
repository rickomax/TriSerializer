using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace TriSerializer
{
    /// <summary>
    /// A serializer for Unity <see cref="Mesh"/> objects, handling serialization and deserialization of mesh data including vertices, normals, and other attributes.
    /// Inherits from <see cref="ObjectSerializer{Mesh}"/>.
    /// </summary>
    public class MeshSerializer : ObjectSerializer<Mesh>
    {
        /// <summary>
        /// Gets the unique identifier for the Mesh serializer.
        /// </summary>
        /// <value>
        /// An <see cref="ObjectIdentifier"/> with the value "MSH".
        /// </value>
        public override ObjectIdentifier Identifier => "MSH";

        /// <summary>
        /// Deserializes a <see cref="Mesh"/> from the provided serialization context, reconstructing its attributes and topology.
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
            var mesh = new Mesh();
            mesh.name = serializationContext.BinaryReader.Read(mesh.name);
            var vertexCount = serializationContext.BinaryReader.ReadInt32();
            var positionCount = serializationContext.BinaryReader.ReadBoolean();
            if (positionCount)
            {
                var nativeArray = serializationContext.GetNewNativeArray<Vector3>(vertexCount);
                for (var i = 0; i < vertexCount; i++)
                {
                    nativeArray[i] = serializationContext.BinaryReader.Read(nativeArray[i]);
                }
                mesh.SetVertices(nativeArray);
                nativeArray.Dispose();
            }

            var normalCount = serializationContext.BinaryReader.ReadBoolean();
            if (normalCount)
            {
                var nativeArray = serializationContext.GetNewNativeArray<Vector3>(vertexCount);
                for (var i = 0; i < vertexCount; i++)
                {
                    nativeArray[i] = serializationContext.BinaryReader.Read(nativeArray[i]);
                }
                mesh.SetNormals(nativeArray);
                nativeArray.Dispose();
            }

            var tangentCount = serializationContext.BinaryReader.ReadBoolean();
            if (tangentCount)
            {
                var nativeArray = serializationContext.GetNewNativeArray<Vector4>(vertexCount);
                for (var i = 0; i < vertexCount; i++)
                {
                    nativeArray[i] = serializationContext.BinaryReader.Read(nativeArray[i]);
                }
                mesh.SetTangents(nativeArray);
                nativeArray.Dispose();
            }

            var colorCount = serializationContext.BinaryReader.ReadBoolean();
            if (colorCount)
            {
                var nativeArray = serializationContext.GetNewNativeArray<Color>(vertexCount);
                for (var i = 0; i < vertexCount; i++)
                {
                    nativeArray[i] = serializationContext.BinaryReader.Read(nativeArray[i]);
                }
                mesh.SetColors(nativeArray);
                nativeArray.Dispose();
            }

            var textCoordCount0 = serializationContext.BinaryReader.ReadBoolean();
            if (textCoordCount0)
            {
                var nativeArray = serializationContext.GetNewNativeArray<Vector2>(vertexCount);
                for (var i = 0; i < vertexCount; i++)
                {
                    nativeArray[i] = serializationContext.BinaryReader.Read(nativeArray[i]);
                }
                mesh.SetUVs(0, nativeArray);
                nativeArray.Dispose();
            }

            var textCoordCount1 = serializationContext.BinaryReader.ReadBoolean();
            if (textCoordCount1)
            {
                var nativeArray = serializationContext.GetNewNativeArray<Vector2>(vertexCount);
                for (var i = 0; i < vertexCount; i++)
                {
                    nativeArray[i] = serializationContext.BinaryReader.Read(nativeArray[i]);
                }
                mesh.SetUVs(1, nativeArray);
                nativeArray.Dispose();
            }

            var textCoordCount2 = serializationContext.BinaryReader.ReadBoolean();
            if (textCoordCount2)
            {
                var nativeArray = serializationContext.GetNewNativeArray<Vector2>(vertexCount);
                for (var i = 0; i < vertexCount; i++)
                {
                    nativeArray[i] = serializationContext.BinaryReader.Read(nativeArray[i]);
                }
                mesh.SetUVs(2, nativeArray);
                nativeArray.Dispose();
            }

            var textCoordCount3 = serializationContext.BinaryReader.ReadBoolean();
            if (textCoordCount3)
            {
                var nativeArray = serializationContext.GetNewNativeArray<Vector2>(vertexCount);
                for (var i = 0; i < vertexCount; i++)
                {
                    nativeArray[i] = serializationContext.BinaryReader.Read(nativeArray[i]);
                }
                mesh.SetUVs(0, nativeArray);
                nativeArray.Dispose();
            }

            var boneWeightCount = serializationContext.BinaryReader.ReadBoolean();
            if (boneWeightCount)
            {
                var array = new BoneWeight[vertexCount];
                for (var i = 0; i < vertexCount; i++)
                {
                    array[i] = serializationContext.BinaryReader.Read(array[i]);
                }
                mesh.boneWeights = array;
            }

            mesh.indexFormat = serializationContext.BinaryReader.ReadEnum(mesh.indexFormat);
            mesh.subMeshCount = serializationContext.BinaryReader.Read(mesh.subMeshCount);
            for (var i = 0; i < mesh.subMeshCount; i++)
            {
                var topology = (MeshTopology)serializationContext.BinaryReader.ReadInt32();
                var indexCount = serializationContext.BinaryReader.ReadInt32();
                var indices = serializationContext.GetNewNativeArray<int>(indexCount);
                for (var j = 0; j < indexCount; j++)
                {
                    indices[j] = serializationContext.BinaryReader.Read(indices[j]);
                }
                mesh.SetIndices(indices, topology, i);
                indices.Dispose();
            }

            // Bind Poses
            var bindPoseCount = serializationContext.BinaryReader.ReadInt32();
            if (bindPoseCount > 0)
            {
                var bindPoses = new Matrix4x4[bindPoseCount];
                for (var i = 0; i < bindPoseCount; i++)
                {
                    bindPoses[i] = serializationContext.BinaryReader.Read(bindPoses[i]);
                }
                mesh.bindposes = bindPoses;
            }

            // Blend Shapes
            var blendShapeCount = serializationContext.BinaryReader.ReadInt32();
            for (var i = 0; i < blendShapeCount; i++)
            {
                var blendShapeName = serializationContext.BinaryReader.ReadString();
                var frameCount = serializationContext.BinaryReader.ReadInt32();
                for (var j = 0; j < frameCount; j++)
                {
                    var blendShapeFrameWeight = serializationContext.BinaryReader.ReadSingle();
                    Vector3[] deltaVertices;
                    var deltaVerticesCount = serializationContext.BinaryReader.ReadInt32();
                    if (deltaVerticesCount > 0)
                    {
                        deltaVertices = new Vector3[deltaVerticesCount];
                        for (var k = 0; k < deltaVerticesCount; k++)
                        {
                            deltaVertices[k] = serializationContext.BinaryReader.Read(deltaVertices[k]);
                        }
                    }
                    else
                    {
                        deltaVertices = null;
                    }
                    Vector3[] deltaNormals;
                    var deltaNormalsCount = serializationContext.BinaryReader.ReadInt32();
                    if (deltaNormalsCount > 0)
                    {
                        deltaNormals = new Vector3[deltaNormalsCount];
                        for (var k = 0; k < deltaNormalsCount; k++)
                        {
                            deltaNormals[k] = serializationContext.BinaryReader.Read(deltaNormals[k]);
                        }
                    }
                    else
                    {
                        deltaNormals = null;
                    }
                    Vector3[] deltaTangents;
                    var deltaTangentsCount = serializationContext.BinaryReader.ReadInt32();
                    if (deltaTangentsCount > 0)
                    {
                        deltaTangents = new Vector3[deltaTangentsCount];
                        for (var k = 0; k < deltaTangentsCount; k++)
                        {
                            deltaTangents[k] = serializationContext.BinaryReader.Read(deltaTangents[k]);
                        }
                    }
                    else
                    {
                        deltaTangents = null;
                    }
                    mesh.AddBlendShapeFrame(blendShapeName, blendShapeFrameWeight, deltaVertices, deltaNormals, deltaTangents);
                }
            }

            // Bounds
            mesh.bounds = serializationContext.BinaryReader.Read(mesh.bounds);
            // Name
            mesh.name = serializationContext.BinaryReader.Read(mesh.name);
            mesh.UploadMeshData(false);
            serializationContext.AddObject(serializationContext.InstanceId, mesh);
            DeserializationCallback(serializationContext, mesh);
            yield break;
        }

        /// <summary>
        /// Serializes a <see cref="Mesh"/> to the provided serialization context, including its attributes and topology.
        /// </summary>
        /// <param name="serializationContext">The context containing the binary writer for serialization.</param>
        /// <param name="source">The <see cref="Mesh"/> to serialize.</param>
        /// <returns>An enumerable sequence of yielded items during serialization.</returns>
        public override IEnumerable Serialize(SerializationContext serializationContext, Mesh source)
        {
            foreach (var item in base.Serialize(serializationContext, source))
            {
                yield return item;
            }
            if (!source.isReadable)
            {
                Debug.LogError($"Mesh '{source}' can't be serialized. It is not readable.");
                serializationContext.BinaryWriter.Write(false);
                yield break;
            }
            else
            {
                serializationContext.BinaryWriter.Write(true);
            }
            serializationContext.BinaryWriter.Write(source.name);
            serializationContext.BinaryWriter.Write(source.vertexCount);
            if (source.HasVertexAttribute(VertexAttribute.Position))
            {
                var list = serializationContext.GetTemporaryList<Vector3>();
                source.GetVertices(list);
                serializationContext.BinaryWriter.Write(true);
                foreach (var item in list)
                {
                    serializationContext.BinaryWriter.Write(item);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(false);
            }

            if (source.HasVertexAttribute(VertexAttribute.Normal))
            {
                var list = serializationContext.GetTemporaryList<Vector3>();
                source.GetNormals(list);
                serializationContext.BinaryWriter.Write(true);
                foreach (var item in list)
                {
                    serializationContext.BinaryWriter.Write(item);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(false);
            }

            if (source.HasVertexAttribute(VertexAttribute.Tangent))
            {
                var list = serializationContext.GetTemporaryList<Vector4>();
                source.GetTangents(list);
                serializationContext.BinaryWriter.Write(true);
                foreach (var item in list)
                {
                    serializationContext.BinaryWriter.Write(item);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(false);
            }

            if (source.HasVertexAttribute(VertexAttribute.Color))
            {
                var list = serializationContext.GetTemporaryList<Color>();
                source.GetColors(list);
                serializationContext.BinaryWriter.Write(true);
                foreach (var item in list)
                {
                    serializationContext.BinaryWriter.Write(item);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(false);
            }

            if (source.HasVertexAttribute(VertexAttribute.TexCoord0))
            {
                var list = serializationContext.GetTemporaryList<Vector2>();
                source.GetUVs(0, list);
                serializationContext.BinaryWriter.Write(true);
                foreach (var item in list)
                {
                    serializationContext.BinaryWriter.Write(item);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(false);
            }

            if (source.HasVertexAttribute(VertexAttribute.TexCoord1))
            {
                var list = serializationContext.GetTemporaryList<Vector2>();
                source.GetUVs(1, list);
                serializationContext.BinaryWriter.Write(true);
                foreach (var item in list)
                {
                    serializationContext.BinaryWriter.Write(item);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(false);
            }

            if (source.HasVertexAttribute(VertexAttribute.TexCoord2))
            {
                var list = serializationContext.GetTemporaryList<Vector2>();
                source.GetUVs(2, list);
                serializationContext.BinaryWriter.Write(true);
                foreach (var item in list)
                {
                    serializationContext.BinaryWriter.Write(item);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(false);
            }

            if (source.HasVertexAttribute(VertexAttribute.TexCoord3))
            {
                var list = serializationContext.GetTemporaryList<Vector2>();
                source.GetUVs(3, list);
                serializationContext.BinaryWriter.Write(true);
                foreach (var item in list)
                {
                    serializationContext.BinaryWriter.Write(item);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(false);
            }

            if (source.HasVertexAttribute(VertexAttribute.BlendIndices))
            {
                var list = serializationContext.GetTemporaryList<BoneWeight>();
                source.GetBoneWeights(list);
                serializationContext.BinaryWriter.Write(true);
                foreach (var item in list)
                {
                    serializationContext.BinaryWriter.Write(item);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(false);
            }

            serializationContext.BinaryWriter.Write((int)source.indexFormat);
            serializationContext.BinaryWriter.Write(source.subMeshCount);
            for (var i = 0; i < source.subMeshCount; i++)
            {
                var topology = source.GetTopology(i);
                serializationContext.BinaryWriter.Write((int)topology);
                var indices = source.GetTriangles(i);
                serializationContext.BinaryWriter.Write(indices.Length);
                for (var j = 0; j < indices.Length; j++)
                {
                    serializationContext.BinaryWriter.Write(indices[j]);
                }
            }

            // Bind Poses
            var bindPoses = source.GetBindposes();
            serializationContext.BinaryWriter.Write(bindPoses.Length);
            for (var i = 0; i < bindPoses.Length; i++)
            {
                var bindPose = bindPoses[i];
                serializationContext.BinaryWriter.Write(bindPose);
            }

            // Blend Shapes
            serializationContext.BinaryWriter.Write(source.blendShapeCount);
            for (var i = 0; i < source.blendShapeCount; i++)
            {
                serializationContext.BinaryWriter.Write(source.GetBlendShapeName(i));
                var frameCount = source.GetBlendShapeFrameCount(i);
                serializationContext.BinaryWriter.Write(frameCount);
                for (var j = 0; j < frameCount; j++)
                {
                    serializationContext.BinaryWriter.Write(source.GetBlendShapeFrameWeight(i, j));
                    var deltaVertices = new Vector3[source.vertexCount];
                    var deltaNormals = new Vector3[source.vertexCount];
                    var deltaTangents = new Vector3[source.vertexCount];
                    source.GetBlendShapeFrameVertices(i, j, deltaVertices, deltaNormals, deltaTangents);
                    serializationContext.BinaryWriter.Write(deltaVertices.Length);
                    foreach (var item in deltaVertices)
                    {
                        serializationContext.BinaryWriter.Write(item);
                    }
                    serializationContext.BinaryWriter.Write(deltaNormals.Length);
                    foreach (var item in deltaNormals)
                    {
                        serializationContext.BinaryWriter.Write(item);
                    }
                    serializationContext.BinaryWriter.Write(deltaTangents.Length);
                    foreach (var item in deltaTangents)
                    {
                        serializationContext.BinaryWriter.Write(item);
                    }
                }
            }

            // Bounds
            serializationContext.BinaryWriter.Write(source.bounds);
            // Name
            serializationContext.BinaryWriter.Write(source.name);
            SerializationCallback(serializationContext, source);
            yield break;
        }
    }
}