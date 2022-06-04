using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Slicer
{
    public class Slicer : MonoBehaviour
    {
        [SerializeField] private Transform _tip;
        [SerializeField] private float _forceAppliedToCut;
        [SerializeField] private Vector3 _normal;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Obstacle obstacle))
            {
                Reload();
            }
        }

        private void Reload()
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Sliceable sliceable))
            {
                var triggerExitTipPosition = _tip.position;

                Vector3 normal = _normal;

                Vector3 transformedNormal = ((Vector3)(other.gameObject.transform.localToWorldMatrix.transpose * normal)).normalized;

                Vector3 transformedStartingPoint = other.gameObject.transform.InverseTransformPoint(triggerExitTipPosition);

                Plane plane = new Plane();

                plane.SetNormalAndPosition(transformedNormal, transformedStartingPoint);
                float direction = Vector3.Dot(Vector3.up, transformedNormal);

                if (direction < 0 )
                {
                    plane = plane.flipped;
                }
                GameObject[] newObjects = Slice.SliceGameObject(plane, sliceable.gameObject);
                Destroy(sliceable.gameObject);

                var rigidbody = newObjects[1].GetComponent<Rigidbody>();
                Vector3 newNormal = transformedNormal + Vector3.up * _forceAppliedToCut;
                rigidbody.AddForce(newNormal, ForceMode.Impulse);
            }
        }
    }
}