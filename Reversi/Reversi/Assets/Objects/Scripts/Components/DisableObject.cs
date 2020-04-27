using System;
using UnityEngine;

namespace Objects.Scripts
{
    public class DisableObject : MonoBehaviour
    {
        public GameObject gameObject;

        private void Start()
        {
            gameObject.SetActive(false);
        }
    }
}