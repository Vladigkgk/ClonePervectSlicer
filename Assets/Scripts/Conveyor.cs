using Assets.Scripts.Slicer;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Conveyor : MonoBehaviour
    {
        [SerializeField] private Transform _endPoint;
        [SerializeField] private float _speed;

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out Sliceable sliceable) || other.TryGetComponent(out Obstacle obstacle))
            {
                other.transform.position = Vector3.MoveTowards(other.transform.position, _endPoint.position, _speed * Time.deltaTime);
            }
        }
    }
}