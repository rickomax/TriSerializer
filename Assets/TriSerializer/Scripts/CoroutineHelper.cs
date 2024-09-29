using UnityEngine;

namespace TriSerializer
{
    public class CoroutineHelper : MonoBehaviour
    {
        private static CoroutineHelper _instance;

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