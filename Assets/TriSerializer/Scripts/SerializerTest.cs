using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TriSerializer.Samples
{
    public class SerializerTest : MonoBehaviour
    {
        private void Start()
        {
            Serializer.SerializeToFile("C://users//ricko//desktop//test.data", gameObject, false);
            Serializer.DeserializeToGameObject("C://users//ricko//desktop//test.data", new GameObject(), false);
        }
    }
}