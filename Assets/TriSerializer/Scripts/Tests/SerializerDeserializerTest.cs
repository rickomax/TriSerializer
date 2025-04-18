using UnityEngine;

namespace TriSerializer.Samples
{
    /// <summary>
    /// A test component that serializes a GameObject to a file and deserializes it into a new GameObject, positioning it with an offset.
    /// </summary>
    public class SerializerDeserializerTest : MonoBehaviour
    {
        /// <summary>
        /// The file path where the GameObject is serialized. Defaults to "Assets/test.data".
        /// </summary>
        public string Filename = "Assets/test.data";

        /// <summary>
        /// Indicates whether to compress the serialized file.
        /// </summary>
        public bool Compress;

        /// <summary>
        /// The horizontal offset to position the deserialized GameObject relative to its original position. Defaults to 1f.
        /// </summary>
        public float HorizontalOffset = 1f;

        /// <summary>
        /// Custom user data passed to the serialization/deserialization methods and callbacks. Defaults to "My serialized file".
        /// </summary>
        private object UserData = "My serialized file";

        /// <summary>
        /// Initiates the serialization of the GameObject to the specified file on start, followed by deserialization.
        /// </summary>
        private void Start()
        {
            Serializer.SerializeToFile(filename: Filename,
            gameObject: gameObject,
            nonLock: true,
            onFinish: OnSerialize,
            onHash: OnHash,
            userData: UserData,
            compress: Compress);
        }

        /// <summary>
        /// Callback invoked when serialization completes, triggering deserialization of the GameObject from the file.
        /// </summary>
        /// <param name="userData">The user data passed during serialization.</param>
        private void OnSerialize(object userData)
        {
            Serializer.DeserializeToGameObject(filename: Filename,
            nonLock: true,
            userData: userData,
            onFinish: OnDeserialize);
        }

        /// <summary>
        /// Callback invoked when deserialization completes, logging the result and positioning the deserialized GameObject.
        /// </summary>
        /// <param name="serializationContext">The context containing the deserialized GameObject and user data.</param>
        private void OnDeserialize(SerializationContext serializationContext)
        {
            var rootGameObject = serializationContext.RootGameObject;
            Debug.Log($"{rootGameObject} created. User data: {UserData}");
            rootGameObject.transform.Translate(HorizontalOffset, 0f, 0f);
        }

        /// <summary>
        /// Callback invoked with the SHA-256 hash of the serialized file contents, useful for verification or database storage.
        /// </summary>
        /// <param name="serializationContext">The serialization context.</param>
        /// <param name="hash">The computed SHA-256 hash of the serialized data.</param>
        private void OnHash(SerializationContext serializationContext, string hash)
        {
            Debug.Log($"Hash is: {hash}");
        }
    }
}