using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private GameObject[] _spawnObjects;
        [SerializeField] private float _timeBetweenSpawn;

        private float _timeSpawn;

        private void Update()
        {
            _timeSpawn += Time.deltaTime;

            if (_timeSpawn > _timeBetweenSpawn)
            {
                var go = _spawnObjects[Random.Range(0, _spawnObjects.Length)];
                Instantiate(go, transform.position, Quaternion.identity);
                _timeSpawn = 0;
            }
        }
    }
}