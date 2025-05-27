using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFarm.Transition
{
    public class Teleport : MonoBehaviour
    {
        public string SceneToGo;
        public Vector3 Destination;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                EventHandler.CallTransitionEvent(SceneToGo, Destination);
            }
        }
    }
}
