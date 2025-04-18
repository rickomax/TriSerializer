using UnityEngine;

namespace TriSerializer
{
    /// <summary>
    /// A singleton MonoBehaviour class that provides a persistent instance for managing coroutines in Unity.
    /// </summary>
    public class CoroutineHelper : MonoBehaviour
    {
        /// <summary>
        /// The single instance of the CoroutineHelper class.
        /// </summary>
        private static CoroutineHelper _instance;

        /// <summary>
        /// Gets the singleton instance of the CoroutineHelper. Creates a new instance if none exists.
        /// </summary>
        /// <value>
        /// The singleton instance of the CoroutineHelper, attached to a GameObject named "CoroutineHelper".
        /// </value>
        public static CoroutineHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("CoroutineHelper").AddComponent<CoroutineHelper>();
                }
                return _instance;
            }
        }
    }
}