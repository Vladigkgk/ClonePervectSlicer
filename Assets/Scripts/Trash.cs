using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Trash : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Destroy(other.gameObject);
        }
    }
}