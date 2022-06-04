using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace Assets.Scripts
{
    public class Knife : MonoBehaviour
    {
        [SerializeField] private float _duration;

        private float _clickTime;

        private void Update()
        {
            _clickTime += Time.deltaTime;

            if (Input.GetMouseButton(0) && _clickTime > _duration)
            {
                transform.DORotate(new Vector3(-90, -180, 90), _duration / 2f).SetLoops(2, LoopType.Yoyo);
                _clickTime = 0;
            }
        }
    }
}