using UnityEngine;

namespace TriSerializer.Samples
{
    /// <summary>
    /// A test component that deserializes a GameObject from a specified file and attaches it as a child of the current GameObject.
    /// </summary>
    public class DeserializerTest : MonoBehaviour
    {
        /// <summary>
        /// The path to the file containing the serialized GameObject data.
        /// </summary>
        public string Filename;

        /// <summary>
        /// Called when the MonoBehaviour is initialized. Deserializes a GameObject from the specified file,
        /// sets it as a child of the current transform, and positions it at the local origin.
        /// </summary>
        private void Start()
        {
            Serializer.DeserializeToGameObject(Filename, false, null, delegate (SerializationContext serializationContext)
            {
                serializationContext.RootGameObject.transform.SetParent(transform, false);
                serializationContext.RootGameObject.transform.localPosition = Vector3.zero;
            });
        }
    }
}