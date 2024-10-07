using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;

namespace TriSerializer
{
    public class SerializationContext
    {
        public BinaryReader BinaryReader { get; set; }
        public BinaryWriter BinaryWriter { get; set; }
        public Object Destination { get; private set; }
        public GameObject GameObject { get; set; }
        public HideFlags HideFlags { get; internal set; }
        public int InstanceId { get; internal set; }
        public Dictionary<int, Object> Objects { get; private set; }
        public HashSet<Object> Resources { get; private set; }

        private readonly Dictionary<System.Type, object> _temporaryLists = new Dictionary<System.Type, object>();

        public void AddObject(int instanceId, Object value)
        {
            if (Objects == null)
            {
                Objects = new Dictionary<int, Object>();
            }
            Objects.Add(instanceId, value);
        }

        public void AddObject(Object value)
        {
            AddObject(value.GetInstanceID(), value);
        }

        public void AddResource(Object value)
        {
            Resources ??= new HashSet<Object>();
            Resources.Add(value);
        }

        public T GetObject<T>(int instanceID) where T : Object
        {
            if (Objects != null && Objects.TryGetValue(instanceID, out var value))
            {
                return (T)value;
            }
            return null;
        }

        public T ReadReference<T>() where T : Object
        {
            var instanceId = BinaryReader.ReadInt32();
            var parent = GetObject<T>(instanceId);
            return parent;
        }

        public T ReadReference<T>(T destination) where T : Object
        {
            return ReadReference<T>();
        }

        public void SetupDestination(Object destination)
        {
           Destination = destination;
           Destination.hideFlags = HideFlags;
           AddObject(InstanceId, destination);
        }

        public List<T> GetTemporaryList<T>()
        {
            var type = typeof(T);
            if (!_temporaryLists.TryGetValue(type, out var list))
            {
                list = new List<T>();
                _temporaryLists[type] = list;
            }
            var resultList = (List<T>)_temporaryLists[type];
            resultList.Clear();
            return resultList;
        }

        public NativeArray<T> GetNewNativeArray<T>(int length) where T : unmanaged
        {
            return new NativeArray<T>(length, Allocator.Persistent);
        }
    }
}