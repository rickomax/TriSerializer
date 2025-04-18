using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Unity.Collections;
using UnityEngine;
namespace TriSerializer
{
    /// <summary>
    /// The serialization context, containing data shared between serialization/deserialization steps.
    /// </summary>
    public class SerializationContext
    {
        private readonly Dictionary<System.Type, object> _temporaryLists = new Dictionary<System.Type, object>();
        private Stopwatch _stopwatch;
        public SerializationContext(bool nonLock = false)
        {
            if (nonLock)
            {
                _stopwatch = new Stopwatch();
                _stopwatch.Start();
            }
        }
        /// <summary>
        /// The BinaryReader used to read the serialized data.
        /// </summary>
        public BinaryReader BinaryReader { get; set; }
        /// <summary>
        /// The BinaryWriter used to write the serialized data.
        /// </summary>
        public BinaryWriter BinaryWriter { get; set; }
        /// <summary>
        /// The GameObject used to serialize the data.
        /// </summary>
        public GameObject GameObject { get; set; }
        /// <summary>
        /// The HideFlags for the last deserialized object.
        /// </summary>
        public HideFlags HideFlags { get; internal set; }
        /// <summary>
        /// The InstanceId for the last deserialized object.
        /// </summary>
        public int InstanceId { get; internal set; }
        /// <summary>
        /// The maximum interval in milliseconds the main thread can stay busy.
        /// </summary>
        public int MaxDelayInMS { get; set; } = 66;
        /// <summary>
        /// The deserialized objects.
        /// </summary>
        public Dictionary<int, Object> Objects { get; private set; } = new Dictionary<int, Object>();
        /// <summary>
        /// The resources to be serialized.
        /// </summary>
        public HashSet<Object> Resources { get; private set; } = new HashSet<Object>();
        /// <summary>
        /// The deserialized root GameObject.
        /// </summary>
        public GameObject RootGameObject { get; private set; }
        /// <summary>
        /// The serialized stream version.
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// The action to call when serializing Objects.
        /// </summary>
        public System.Action<SerializationContext, Object> SerializationCallback { get; set; }
        /// <summary>
        /// The action to call when deserializing Objects.
        /// </summary>
        public System.Action<SerializationContext, Object> DeserializationCallback { get; set; }
        /// <summary>
        /// The user data from the current context.
        /// </summary>
        public System.Object UserData { get; set; }
        /// <summary>
        /// Adds an object to the objects list.
        /// </summary>
        public void AddObject(int instanceId, Object value)
        {
            if (RootGameObject == null && value is GameObject gameObject)
            {
                RootGameObject = gameObject;
            }
            Objects.Add(instanceId, value);
        }
        /// <summary>
        /// Adds an object to the objects list.
        /// </summary>
        public void AddObject(Object value)
        {
            AddObject(value.GetInstanceID(), value);
        }
        /// <summary>
        /// Adds a resource to the resouces list.
        /// </summary>
        public void AddResource(Object value)
        {
            if (value == null)
            {
                return;
            }
            Resources.Add(value);
        }
        /// <summary>
        /// Gets a new NativeArray with the given length.
        /// </summary>
        public NativeArray<T> GetNewNativeArray<T>(int length) where T : unmanaged
        {
            return new NativeArray<T>(length, Allocator.Persistent);
        }
        /// <summary>
        /// Gets a deserialized object from the list.
        /// </summary>
        public T GetObject<T>(int instanceID) where T : Object
        {
            if (Objects.TryGetValue(instanceID, out var value))
            {
                return (T)value;
            }
            return null;
        }
        /// <summary>
        /// Gets a temporary list with the given type.
        /// </summary>
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
        /// <summary>
        /// Reads the reference from the stream, and returns the object with that reference from the objects list. Or <c> null</c>.
        /// </summary>
        public T ReadReference<T>() where T : Object
        {
            var instanceId = BinaryReader.ReadInt32();
            var found = GetObject<T>(instanceId);
            return found;
        }
        /// <summary>
        /// Reads the reference from the stream, and returns the object with that reference from the objects list. Or <c> null</c>.
        /// </summary>
        public T ReadReference<T>(T destination) where T : Object
        {
            return ReadReference<T>();
        }

        /// <summary>
        /// Yields if the main thread has been busy for more than "MaxDelayInMS" value in milliseconds.
        /// </summary>
        public IEnumerable ReleaseMainThread()
        {
            if (_stopwatch == null)
            {
                yield break;
            }
            if (_stopwatch.ElapsedMilliseconds > MaxDelayInMS)
            {
                yield return null;
                _stopwatch.Restart();
            }
        }
        /// <summary>
        /// Assigns the "HideFlags" to the given destination object.
        /// </summary>
        public void SetupDestination(Object destination)
        {
            destination.hideFlags = HideFlags;
            AddObject(InstanceId, destination);
        }
        /// <summary>
        /// Writes an object reference to the output stream.
        /// </summary>
        public void WriteReference(Object value)
        {
            BinaryWriter.Write(value == null ? 0 : value.GetInstanceID());
        }
    }
}