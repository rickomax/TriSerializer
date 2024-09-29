using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.VisualScripting.Member;
using static UnityEngine.Mesh;
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
            serializationContext.AddResource(source.mesh);
            yield break;
        }
    }

    public class RendererSerializer<T> : ComponentSerializer<T> where T : Renderer
    {
        protected void SerializeMeshData(SerializationContext serializationContext, T source)
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
        }

        protected void DeserializeMeshData(SerializationContext serializationContext)
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

            SerializeMeshData(serializationContext, source);
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

            DeserializeMeshData(serializationContext);
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

    public partial class MeshSerializer : ObjectSerializer<Mesh>
    {
        public override ObjectIdentifier Identifier => "MSH";

        private static int GetVertexAttributeFormatSize(VertexAttributeFormat format)
        {
            switch (format)
            {
                case VertexAttributeFormat.Float32:
                    return 4;
                case VertexAttributeFormat.Float16:
                    return 2;
                case VertexAttributeFormat.UNorm8:
                case VertexAttributeFormat.SNorm8:
                case VertexAttributeFormat.UInt8:
                case VertexAttributeFormat.SInt8:
                    return 1;
                case VertexAttributeFormat.UNorm16:
                case VertexAttributeFormat.SNorm16:
                case VertexAttributeFormat.UInt16:
                case VertexAttributeFormat.SInt16:
                    return 2;
                case VertexAttributeFormat.UInt32:
                case VertexAttributeFormat.SInt32:
                    return 4;
            }
            throw new ArgumentOutOfRangeException();
        }

        public override IEnumerable Serialize(SerializationContext serializationContext, Mesh source)
        {
            foreach (var item in base.Serialize(serializationContext, source))
            {
                yield return item;
            }

            if (!source.isReadable)
            {
                serializationContext.BinaryWriter.Write(0);
                yield break;
            }

            var vertexDataSizePerStream = new Dictionary<int, int>();

            // Vertex Attribute Descriptors
            var vertexAttributeDescriptors = source.GetVertexAttributes();
            serializationContext.BinaryWriter.Write(vertexAttributeDescriptors.Length);
            for (var i = 0; i < vertexAttributeDescriptors.Length; i++)
            {
                var vertexAttributeDescriptor = vertexAttributeDescriptors[i];
                serializationContext.BinaryWriter.Write(vertexAttributeDescriptor.dimension);
                serializationContext.BinaryWriter.Write(vertexAttributeDescriptor.stream);
                serializationContext.BinaryWriter.Write((int)vertexAttributeDescriptor.attribute);
                serializationContext.BinaryWriter.Write((int)vertexAttributeDescriptor.format);
                vertexDataSizePerStream.TryAdd(vertexAttributeDescriptor.stream, 0);
                vertexDataSizePerStream[vertexAttributeDescriptor.stream] += GetVertexAttributeFormatSize(vertexAttributeDescriptor.format) * vertexAttributeDescriptor.dimension;
            }

            // Mesh Data Array
            using (var meshDataArray = Mesh.AcquireReadOnlyMeshData(source))
            {
                serializationContext.BinaryWriter.Write(meshDataArray.Length);
                for (var i = 0; i < meshDataArray.Length; i++)
                {
                    // Mesh Data
                    var meshData = meshDataArray[i];
                    foreach (var kvp in vertexDataSizePerStream)
                    {
                        Write(serializationContext.BinaryWriter, meshData, kvp.Value, kvp.Key);
                    }
                }
            }

            // Sub Meshes Descriptors
            serializationContext.BinaryWriter.Write(source.subMeshCount);
            for (var i = 0; i < source.subMeshCount; i++)
            {
                var subMeshDescriptor = source.GetSubMesh(i);
                serializationContext.BinaryWriter.Write(subMeshDescriptor.baseVertex);
                serializationContext.BinaryWriter.Write(subMeshDescriptor.firstVertex);
                serializationContext.BinaryWriter.Write(subMeshDescriptor.indexCount);
                serializationContext.BinaryWriter.Write(subMeshDescriptor.indexStart);
                serializationContext.BinaryWriter.Write(subMeshDescriptor.vertexCount);
                serializationContext.BinaryWriter.Write((int)subMeshDescriptor.topology);
                serializationContext.BinaryWriter.Write(subMeshDescriptor.bounds);
            }

            // Skin Weight Buffer Layout
            serializationContext.BinaryWriter.Write((int)source.skinWeightBufferLayout);

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
                    serializationContext.BinaryWriter.Write(deltaVertices);
                    serializationContext.BinaryWriter.Write(deltaNormals);
                    serializationContext.BinaryWriter.Write(deltaTangents);
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

            var vertexDataSizePerStream = new Dictionary<int, int>();

            var vertexAttributeDescriptorLength = serializationContext.BinaryReader.ReadInt32();
            var vertexAttributeDescriptors = new VertexAttributeDescriptor[vertexAttributeDescriptorLength];
            for (var i = 0; i < vertexAttributeDescriptors.Length; i++)
            {
                var vertexAttributeDescriptor = vertexAttributeDescriptors[i];

                vertexAttributeDescriptor.dimension = serializationContext.BinaryReader.Read(vertexAttributeDescriptor.dimension);
                vertexAttributeDescriptor.stream = serializationContext.BinaryReader.Read(vertexAttributeDescriptor.stream);
                vertexAttributeDescriptor.attribute = serializationContext.BinaryReader.ReadEnum(vertexAttributeDescriptor.attribute);
                vertexAttributeDescriptor.format = serializationContext.BinaryReader.ReadEnum(vertexAttributeDescriptor.format);

                vertexAttributeDescriptors[i] = vertexAttributeDescriptor;

                vertexDataSizePerStream.TryAdd(vertexAttributeDescriptor.stream, 0);
                vertexDataSizePerStream[vertexAttributeDescriptor.stream] += GetVertexAttributeFormatSize(vertexAttributeDescriptor.format) * vertexAttributeDescriptor.dimension;
            }

            var meshDataArrayLength = serializationContext.BinaryReader.ReadInt32();
            using (var meshDataArray = Mesh.AllocateWritableMeshData(meshDataArrayLength))
            {
                for (var i = 0; i < meshDataArray.Length; i++)
                {
                    // Mesh Data
                    var meshData = meshDataArray[i];
                    foreach (var kvp in vertexDataSizePerStream)
                    {
                        Read(serializationContext.BinaryReader, meshData, kvp.Value, kvp.Key);
                    }
                }
            }

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