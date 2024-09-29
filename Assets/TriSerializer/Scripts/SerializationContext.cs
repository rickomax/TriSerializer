using System.Collections.Generic;
using System.IO;
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
    }
}