using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.VisualScripting.Member;
using Object = UnityEngine.Object;

namespace TriSerializer
{

    public interface IObjectSerializer
    {
        ObjectIdentifier Identifier { get; }
        IEnumerable Serialize(SerializationContext serializationContext, Object source);
        IEnumerable Deserialize(SerializationContext serializationContext);
        IEnumerable CollectResources(SerializationContext serializationContext, Object source);
    }

    public abstract class ObjectSerializer<T> : IObjectSerializer where T : Object
    {
        public virtual ObjectIdentifier Identifier { get; }

        public virtual IEnumerable Serialize(SerializationContext serializationContext, T source)
        {
            serializationContext.BinaryWriter.Write(Identifier);
            serializationContext.BinaryWriter.Write(source.GetInstanceID());
            serializationContext.BinaryWriter.Write((int)source.hideFlags);
            yield break;
        }

        public virtual IEnumerable Deserialize(SerializationContext serializationContext)
        {
            serializationContext.InstanceId = serializationContext.BinaryReader.ReadInt32();
            serializationContext.HideFlags = (HideFlags)serializationContext.BinaryReader.ReadInt32();
            yield break;
        }

        public virtual IEnumerable CollectResources(SerializationContext serializationContext, T source)
        {
            yield break;
        }

        IEnumerable IObjectSerializer.Serialize(SerializationContext serializationContext, Object source)
        {
            var enumerable = Serialize(serializationContext, (T)source);
            var enumerator = enumerable.GetEnumerator();
            using var unknown = enumerator as IDisposable;
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        IEnumerable IObjectSerializer.Deserialize(SerializationContext serializationContext)
        {
            var enumerable = Deserialize(serializationContext);
            var enumerator = enumerable.GetEnumerator();
            using var unknown = enumerator as IDisposable;
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        IEnumerable IObjectSerializer.CollectResources(SerializationContext serializationContext, Object source)
        {
            var enumerable = CollectResources(serializationContext, (T)source);
            var enumerator = enumerable.GetEnumerator();
            using var unknown = enumerator as IDisposable;
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

    }

    public abstract class ComponentSerializer<T> : ObjectSerializer<T> where T : Component
    {
        public override IEnumerable Serialize(SerializationContext serializationContext, T source)
        {
            foreach (var item in base.Serialize(serializationContext, source))
            {
                yield return item;
            }
            serializationContext.BinaryWriter.Write(source.gameObject.GetInstanceID());
        }

        public override IEnumerable Deserialize(SerializationContext serializationContext)
        {
            foreach (var item in base.Deserialize(serializationContext))
            {
                yield return item;
            }
            var gameObjectId = serializationContext.BinaryReader.ReadInt32();
            serializationContext.GameObject = serializationContext.GetObject<GameObject>(gameObjectId);
        }
    }

    public class MeshFilterSerializer : ComponentSerializer<MeshFilter>
    {
        public override ObjectIdentifier Identifier => "MFL";

        public override IEnumerable CollectResources(SerializationContext serializationContext, MeshFilter source)
        {
            foreach (var item in base.CollectResources(serializationContext, source))
            {
                yield return item;
            }
            serializationContext.AddResource(source.sharedMesh);
            serializationContext.AddResource(source.mesh);
            yield break;
        }

        public override IEnumerable Serialize(SerializationContext serializationContext, MeshFilter source)
        {
            foreach (var item in base.Serialize(serializationContext, source))
            {
                yield return item;
            }
            serializationContext.BinaryWriter.WriteReference(source.sharedMesh);
            serializationContext.BinaryWriter.WriteReference(source.mesh);
        }

        public override IEnumerable Deserialize(SerializationContext serializationContext)
        {
            foreach (var item in base.Deserialize(serializationContext))
            {
                yield return item;
            }
            var destination = serializationContext.GameObject.AddComponent<MeshFilter>();
            serializationContext.SetupDestination(destination);
            destination.sharedMesh = serializationContext.ReadReference(destination.sharedMesh);
            destination.mesh = serializationContext.ReadReference(destination.mesh);
        }
    }

    public class RendererSerializer<T> : ComponentSerializer<T> where T : Renderer
    {
        public override IEnumerable CollectResources(SerializationContext serializationContext, T source)
        {
            var sharedMaterials = source.sharedMaterials;
            if (sharedMaterials != null)
            {
                foreach (var sharedMaterial in sharedMaterials)
                {
                    serializationContext.AddResource(sharedMaterial);
                }
            }
            var materials = source.materials;
            if (materials != null)
            {
                foreach (var material in materials)
                {
                    serializationContext.AddResource(material);
                }
            }
            yield break;
        }

        protected void SerializeRenderer(SerializationContext serializationContext, T source)
        {
            serializationContext.BinaryWriter.Write(source.bounds);
            serializationContext.BinaryWriter.Write(source.localBounds);
            serializationContext.BinaryWriter.Write(source.allowOcclusionWhenDynamic);
            serializationContext.BinaryWriter.Write(source.enabled);
            serializationContext.BinaryWriter.Write(source.forceRenderingOff);
            serializationContext.BinaryWriter.WriteReference(source.lightProbeProxyVolumeOverride);
            serializationContext.BinaryWriter.Write(source.lightmapIndex);
            serializationContext.BinaryWriter.Write(source.lightmapScaleOffset);
            serializationContext.BinaryWriter.WriteReference(source.probeAnchor);
            serializationContext.BinaryWriter.Write(source.realtimeLightmapIndex);
            serializationContext.BinaryWriter.Write(source.realtimeLightmapScaleOffset);
            serializationContext.BinaryWriter.Write(source.rendererPriority);
            serializationContext.BinaryWriter.Write(source.renderingLayerMask);
            serializationContext.BinaryWriter.Write(source.sortingLayerID);
            serializationContext.BinaryWriter.Write(source.sortingLayerName);
            serializationContext.BinaryWriter.Write((int)source.lightProbeUsage);
            serializationContext.BinaryWriter.Write((int)source.shadowCastingMode);
            var sharedMaterials = source.sharedMaterials;
            if (sharedMaterials != null)
            {
                serializationContext.BinaryWriter.Write(sharedMaterials.Length);
                for (var i = 0; i < sharedMaterials.Length; i++)
                {
                    serializationContext.BinaryWriter.WriteReference(sharedMaterials[i]);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(0);
            }
            var materials = source.materials;
            if (materials != null)
            {
                serializationContext.BinaryWriter.Write(materials.Length);
                for (var i = 0; i < materials.Length; i++)
                {
                    serializationContext.BinaryWriter.WriteReference(materials[i]);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(0);
            }
        }

        protected void DeserializeRenderer(SerializationContext serializationContext)
        {
            var destination = (Renderer)serializationContext.Destination;

            destination.bounds = serializationContext.BinaryReader.Read(destination.bounds);
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
            destination.lightProbeUsage = (LightProbeUsage)serializationContext.BinaryReader.Read((int)destination.lightProbeUsage);
            destination.shadowCastingMode = (ShadowCastingMode)serializationContext.BinaryReader.Read((int)destination.shadowCastingMode);
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
        }

    }

    public class MeshRendererSerializer : RendererSerializer<MeshRenderer>
    {
        public override ObjectIdentifier Identifier => "MRD";

        public override IEnumerable Serialize(SerializationContext serializationContext, MeshRenderer source)
        {
            foreach (var item in base.Serialize(serializationContext, source))
            {
                yield return item;
            }

            SerializeRenderer(serializationContext, source);
            serializationContext.BinaryWriter.WriteReference(source.additionalVertexStreams);
            serializationContext.BinaryWriter.WriteReference(source.enlightenVertexStream);
            serializationContext.BinaryWriter.Write((int)source.receiveGI);
            serializationContext.BinaryWriter.Write(source.scaleInLightmap);
            serializationContext.BinaryWriter.Write(source.stitchLightmapSeams);

            yield break;
        }

        public override IEnumerable Deserialize(SerializationContext serializationContext)
        {
            foreach (var item in base.Deserialize(serializationContext))
            {
                yield return item;
            }

            var destination = serializationContext.GameObject.AddComponent<MeshRenderer>();
            serializationContext.SetupDestination(destination);

            DeserializeRenderer(serializationContext);
            destination.additionalVertexStreams = serializationContext.ReadReference(destination.additionalVertexStreams);
            destination.enlightenVertexStream = serializationContext.ReadReference(destination.enlightenVertexStream);
            destination.receiveGI = (ReceiveGI)serializationContext.BinaryReader.Read((int)destination.receiveGI);
            destination.scaleInLightmap = serializationContext.BinaryReader.Read(destination.scaleInLightmap);
            destination.stitchLightmapSeams = serializationContext.BinaryReader.Read(destination.stitchLightmapSeams);
        }
    }

    public class SkinnedMeshRendererSerializer : RendererSerializer<SkinnedMeshRenderer>
    {
        public override ObjectIdentifier Identifier => "SMR";

        public override IEnumerable Serialize(SerializationContext serializationContext, SkinnedMeshRenderer source)
        {
            foreach (var item in base.Serialize(serializationContext, source))
            {
                yield return item;
            }

            serializationContext.AddResource(source.sharedMesh);

            serializationContext.BinaryWriter.WriteReference(source.sharedMesh);
            serializationContext.BinaryWriter.Write(source.forceMatrixRecalculationPerRender);
            serializationContext.BinaryWriter.WriteReference(source.rootBone);
            serializationContext.BinaryWriter.Write(source.skinnedMotionVectors);
            serializationContext.BinaryWriter.Write(source.updateWhenOffscreen);

            yield break;
        }

        public override IEnumerable Deserialize(SerializationContext serializationContext)
        {
            throw new NotImplementedException();
        }
    }

    public class MeshSerializer : ObjectSerializer<Mesh>
    {
        public override ObjectIdentifier Identifier => "MSH";

        public override IEnumerable Serialize(SerializationContext serializationContext, Mesh source)
        {
            foreach (var item in base.Serialize(serializationContext, source))
            {
                yield return item;
            }

            if (!source.isReadable)
            {
                serializationContext.BinaryWriter.Write(false);
                yield break;
            }
            else
            {
                serializationContext.BinaryWriter.Write(true);
            }

            if (source.HasVertexAttribute(VertexAttribute.Position))
            {
                var list = serializationContext.GetTemporaryList<Vector3>();
                source.GetVertices(list);
                serializationContext.BinaryWriter.Write(list.Count);
                foreach (var item in list)
                {
                    serializationContext.BinaryWriter.Write(item);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(0);
            }

            if (source.HasVertexAttribute(VertexAttribute.Normal))
            {
                var list = serializationContext.GetTemporaryList<Vector3>();
                source.GetNormals(list);
                serializationContext.BinaryWriter.Write(list.Count);
                foreach (var item in list)
                {
                    serializationContext.BinaryWriter.Write(item);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(0);
            }

            if (source.HasVertexAttribute(VertexAttribute.Tangent))
            {
                var list = serializationContext.GetTemporaryList<Vector4>();
                source.GetTangents(list);
                serializationContext.BinaryWriter.Write(list.Count);
                foreach (var item in list)
                {
                    serializationContext.BinaryWriter.Write(item);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(0);
            }


            if (source.HasVertexAttribute(VertexAttribute.Color))
            {
                var list = serializationContext.GetTemporaryList<Color>();
                source.GetColors(list);
                serializationContext.BinaryWriter.Write(list.Count);
                foreach (var item in list)
                {
                    serializationContext.BinaryWriter.Write(item);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(0);
            }

            if (source.HasVertexAttribute(VertexAttribute.TexCoord0))
            {
                var list = serializationContext.GetTemporaryList<Vector2>();
                source.GetUVs(0, list);
                serializationContext.BinaryWriter.Write(list.Count);
                foreach (var item in list)
                {
                    serializationContext.BinaryWriter.Write(item);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(0);
            }

            if (source.HasVertexAttribute(VertexAttribute.TexCoord1))
            {
                var list = serializationContext.GetTemporaryList<Vector2>();
                source.GetUVs(1, list);
                serializationContext.BinaryWriter.Write(list.Count);
                foreach (var item in list)
                {
                    serializationContext.BinaryWriter.Write(item);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(0);
            }

            if (source.HasVertexAttribute(VertexAttribute.TexCoord2))
            {
                var list = serializationContext.GetTemporaryList<Vector2>();
                source.GetUVs(2, list);
                serializationContext.BinaryWriter.Write(list.Count);
                foreach (var item in list)
                {
                    serializationContext.BinaryWriter.Write(item);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(0);
            }

            if (source.HasVertexAttribute(VertexAttribute.TexCoord3))
            {
                var list = serializationContext.GetTemporaryList<Vector2>();
                source.GetUVs(3, list);
                serializationContext.BinaryWriter.Write(list.Count);
                foreach (var item in list)
                {
                    serializationContext.BinaryWriter.Write(item);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(0);
            }

            if (source.HasVertexAttribute(VertexAttribute.BlendIndices))
            {
                var list = serializationContext.GetTemporaryList<BoneWeight>();
                source.GetBoneWeights(list);
                serializationContext.BinaryWriter.Write(list.Count);
                foreach (var item in list)
                {
                    serializationContext.BinaryWriter.Write(item);
                }
            }
            else
            {
                serializationContext.BinaryWriter.Write(0);
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
        }

        public override IEnumerable Deserialize(SerializationContext serializationContext)
        {
            foreach (var item in base.Deserialize(serializationContext))
            {
                yield return item;
            }

            var mesh = new Mesh();

            var isReadable = serializationContext.BinaryReader.ReadBoolean();
            if (!isReadable)
            {
                yield break;
            }

            var vertexCount = serializationContext.BinaryReader.ReadInt32();
            if (vertexCount > 0)
            {
                var nativeArray = serializationContext.GetNewNativeArray<Vector3>(vertexCount);
                for (var i = 0; i < vertexCount; i++)
                {
                    nativeArray[i] = serializationContext.BinaryReader.Read(nativeArray[i]);
                }
                mesh.SetVertices(nativeArray);
                nativeArray.Dispose();
            }

            var normalCount = serializationContext.BinaryReader.ReadInt32();
            if (normalCount > 0)
            {
                var nativeArray = serializationContext.GetNewNativeArray<Vector3>(normalCount);
                for (var i = 0; i < vertexCount; i++)
                {
                    nativeArray[i] = serializationContext.BinaryReader.Read(nativeArray[i]);
                }
                mesh.SetNormals(nativeArray);
                nativeArray.Dispose();
            }

            var tangentCount = serializationContext.BinaryReader.ReadInt32();
            if (tangentCount > 0)
            {
                var nativeArray = serializationContext.GetNewNativeArray<Vector4>(tangentCount);
                for (var i = 0; i < vertexCount; i++)
                {
                    nativeArray[i] = serializationContext.BinaryReader.Read(nativeArray[i]);
                }
                mesh.SetTangents(nativeArray);
                nativeArray.Dispose();
            }

            var colorCount = serializationContext.BinaryReader.ReadInt32();
            if (colorCount > 0)
            {
                var nativeArray = serializationContext.GetNewNativeArray<Color>(colorCount);
                for (var i = 0; i < vertexCount; i++)
                {
                    nativeArray[i] = serializationContext.BinaryReader.Read(nativeArray[i]);
                }
                mesh.SetColors(nativeArray);
                nativeArray.Dispose();
            }

            var textCoordCount0 = serializationContext.BinaryReader.ReadInt32();
            if (textCoordCount0 > 0)
            {
                var nativeArray = serializationContext.GetNewNativeArray<Vector2>(textCoordCount0);
                for (var i = 0; i < vertexCount; i++)
                {
                    nativeArray[i] = serializationContext.BinaryReader.Read(nativeArray[i]);
                }
                mesh.SetUVs(0, nativeArray);
                nativeArray.Dispose();
            }

            var textCoordCount1 = serializationContext.BinaryReader.ReadInt32();
            if (textCoordCount1 > 0)
            {
                var nativeArray = serializationContext.GetNewNativeArray<Vector2>(textCoordCount1);
                for (var i = 0; i < vertexCount; i++)
                {
                    nativeArray[i] = serializationContext.BinaryReader.Read(nativeArray[i]);
                }
                mesh.SetUVs(1, nativeArray);
                nativeArray.Dispose();
            }

            var textCoordCount2 = serializationContext.BinaryReader.ReadInt32();
            if (textCoordCount2 > 0)
            {
                var nativeArray = serializationContext.GetNewNativeArray<Vector2>(textCoordCount2);
                for (var i = 0; i < vertexCount; i++)
                {
                    nativeArray[i] = serializationContext.BinaryReader.Read(nativeArray[i]);
                }
                mesh.SetUVs(2, nativeArray);
                nativeArray.Dispose();
            }

            var textCoordCount3 = serializationContext.BinaryReader.ReadInt32();
            if (textCoordCount3 > 0)
            {
                var nativeArray = serializationContext.GetNewNativeArray<Vector2>(textCoordCount3);
                for (var i = 0; i < vertexCount; i++)
                {
                    nativeArray[i] = serializationContext.BinaryReader.Read(nativeArray[i]);
                }
                mesh.SetUVs(0, nativeArray);
                nativeArray.Dispose();
            }

            var boneWeightCount = serializationContext.BinaryReader.ReadInt32();
            if (boneWeightCount > 0)
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
        }
    }


    public class MaterialSerializer : ObjectSerializer<Material>
    {
        public override ObjectIdentifier Identifier => "MAT";

        public override IEnumerable Serialize(SerializationContext serializationContext, Material source)
        {
            foreach (var item in base.Serialize(serializationContext, source))
            {
                yield return item;
            }

            var shader = source.shader;

            serializationContext.BinaryWriter.Write(shader.name);

            var floatProperties = source.GetPropertyNames(MaterialPropertyType.Float);
            serializationContext.BinaryWriter.Write(floatProperties.Length);
            for (var i = 0; i < floatProperties.Length; i++)
            {
                var property = Shader.PropertyToID(floatProperties[i]);
                serializationContext.BinaryWriter.Write(property);
                serializationContext.BinaryWriter.Write(source.GetFloat(property));
            }

            var intProperties = source.GetPropertyNames(MaterialPropertyType.Int);
            serializationContext.BinaryWriter.Write(intProperties.Length);
            for (var i = 0; i < intProperties.Length; i++)
            {
                var property = Shader.PropertyToID(intProperties[i]);
                serializationContext.BinaryWriter.Write(property);
                serializationContext.BinaryWriter.Write(source.GetInt(property));
            }

            var vectorProperties = source.GetPropertyNames(MaterialPropertyType.Vector);
            serializationContext.BinaryWriter.Write(vectorProperties.Length);
            for (var i = 0; i < vectorProperties.Length; i++)
            {
                var property = Shader.PropertyToID(vectorProperties[i]);
                serializationContext.BinaryWriter.Write(property);
                serializationContext.BinaryWriter.Write(source.GetVector(property));
            }

            var matrixProperties = source.GetPropertyNames(MaterialPropertyType.Matrix);
            serializationContext.BinaryWriter.Write(matrixProperties.Length);
            for (var i = 0; i < matrixProperties.Length; i++)
            {
                var property = Shader.PropertyToID(matrixProperties[i]);
                serializationContext.BinaryWriter.Write(property);
                serializationContext.BinaryWriter.Write(source.GetMatrix(property));
            }

            var textureProperties = source.GetPropertyNames(MaterialPropertyType.Texture);
            serializationContext.BinaryWriter.Write(textureProperties.Length);
            for (var i = 0; i < textureProperties.Length; i++)
            {
                var property = Shader.PropertyToID(textureProperties[i]);
                serializationContext.BinaryWriter.Write(property);
                serializationContext.BinaryWriter.WriteReference(source.GetTexture(property));
            }

            serializationContext.BinaryWriter.Write(source.doubleSidedGI);

            var enabledKeywords = source.enabledKeywords;
            serializationContext.BinaryWriter.Write(enabledKeywords.Length);
            for (var i = 0; i < enabledKeywords.Length; i++)
            {
                serializationContext.BinaryWriter.Write(enabledKeywords[i].name);
            }

            serializationContext.BinaryWriter.Write(source.enableInstancing);

            serializationContext.BinaryWriter.Write((int)source.globalIlluminationFlags);

            serializationContext.BinaryWriter.WriteReference(source.parent);

            serializationContext.BinaryWriter.Write(source.renderQueue);
        }

        public override IEnumerable Deserialize(SerializationContext serializationContext)
        {
            foreach (var item in base.Deserialize(serializationContext))
            {
                yield return item;
            }
            
            var shaderName = serializationContext.BinaryReader.ReadString();

            var shader = Shader.Find(shaderName);

            Debug.Assert(shader != null);

            var material = new Material(shader);

            var floatPropertiesCount = serializationContext.BinaryReader.ReadInt32();
            for (var i = 0; i < floatPropertiesCount; i++)
            {
                var property = serializationContext.BinaryReader.ReadInt32();
                float value = default;
                value = serializationContext.BinaryReader.Read(value);
                material.SetFloat(property, value);
            }

            var intPropertiesCount = serializationContext.BinaryReader.ReadInt32();
            for (var i = 0; i < intPropertiesCount; i++)
            {
                var property = serializationContext.BinaryReader.ReadInt32();
                int value = default;
                value = serializationContext.BinaryReader.Read(value);
                material.SetInt(property, value);
            }

            var vectorPropertiesCount = serializationContext.BinaryReader.ReadInt32();
            for (var i = 0; i < vectorPropertiesCount; i++)
            {
                var property = serializationContext.BinaryReader.ReadInt32();
                Vector4 value = default;
                value = serializationContext.BinaryReader.Read(value);
                material.SetVector(property, value);
            }

            var matrixPropertiesCount = serializationContext.BinaryReader.ReadInt32();
            for (var i = 0; i < matrixPropertiesCount; i++)
            {
                var property = serializationContext.BinaryReader.ReadInt32();
                Matrix4x4 value = default;
                value = serializationContext.BinaryReader.Read(value);
                material.SetMatrix(property, value);
            }

            var texturePropertiesCount = serializationContext.BinaryReader.ReadInt32();
            for (var i = 0; i < texturePropertiesCount; i++)
            {
                var property = serializationContext.BinaryReader.ReadInt32();
                Texture value = default;
                value = serializationContext.ReadReference(value);
                material.SetTexture(property, value);
            }

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

            material.parent = serializationContext.ReadReference<Material>();

            material.renderQueue = serializationContext.BinaryReader.Read(material.renderQueue);

            serializationContext.AddObject(serializationContext.InstanceId, material);
        }
    }

    public class TransformSerializer : ComponentSerializer<Transform>
    {
        public override ObjectIdentifier Identifier => "TRF";

        public override IEnumerable Serialize(SerializationContext serializationContext, Transform source)
        {
            foreach (var item in base.Serialize(serializationContext, source))
            {
                yield return item;
            }

            serializationContext.BinaryWriter.WriteReference(source.transform.parent);

            serializationContext.BinaryWriter.Write(source.transform.hasChanged);
            serializationContext.BinaryWriter.Write(source.transform.hierarchyCapacity);
            serializationContext.BinaryWriter.Write(source.transform.localPosition);
            serializationContext.BinaryWriter.Write(source.transform.localRotation);
            serializationContext.BinaryWriter.Write(source.transform.localScale);

            yield break;
        }

        public override IEnumerable Deserialize(SerializationContext serializationContext)
        {
            foreach (var item in base.Deserialize(serializationContext))
            {
                yield return item;
            }

            var destination = serializationContext.GameObject.transform;
            serializationContext.SetupDestination(destination);

            var parent = serializationContext.ReadReference<Transform>();
            destination.SetParent(parent, false);

            destination.hasChanged = serializationContext.BinaryReader.Read(destination.transform.hasChanged);
            destination.hierarchyCapacity = serializationContext.BinaryReader.Read(destination.transform.hierarchyCapacity);
            destination.localPosition = serializationContext.BinaryReader.Read(destination.transform.localPosition);
            destination.localRotation = serializationContext.BinaryReader.Read(destination.transform.localRotation);
            destination.localScale = serializationContext.BinaryReader.Read(destination.transform.localScale);

            yield break;
        }
    }


    public class GameObjectSerializer : ObjectSerializer<GameObject>
    {
        public override ObjectIdentifier Identifier => "GOB";

        public override IEnumerable Serialize(SerializationContext serializationContext, GameObject source)
        {
            foreach (var item in base.Serialize(serializationContext, source))
            {
                yield return item;
            }

            serializationContext.AddObject(source);

            serializationContext.BinaryWriter.Write(source.active);
            serializationContext.BinaryWriter.Write(source.isStatic);
            serializationContext.BinaryWriter.Write(source.layer);
            serializationContext.BinaryWriter.Write(source.tag);
            serializationContext.BinaryWriter.Write(source.name);

            yield break;
        }

        public override IEnumerable Deserialize(SerializationContext serializationContext)
        {
            var destination = new GameObject();

            foreach (var item in base.Deserialize(serializationContext))
            {
                yield return item;
            }

            serializationContext.SetupDestination(destination);

            destination.active = serializationContext.BinaryReader.Read(destination.active);
            destination.isStatic = serializationContext.BinaryReader.Read(destination.isStatic);
            destination.layer = serializationContext.BinaryReader.Read(destination.layer);
            destination.tag = serializationContext.BinaryReader.Read(destination.tag);
            destination.name = serializationContext.BinaryReader.Read(destination.name);

            yield break;
        }
    }
}