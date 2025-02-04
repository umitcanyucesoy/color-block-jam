using System;
using _ColorBlockJam.Scripts._Enums;
using DG.Tweening;
using UnityEngine;

namespace _ColorBlockJam.Scripts._Blocks
{
    public class VacuumBlock : MonoBehaviour
    {
        [Header("----- Vacuum Elements -----")]
        public BlockColor vacuumColor;
        
        [Header("----- Vacuum Push Animation -----")]
        [SerializeField] private float downDuration = .5f;
        [SerializeField] private float stayDuration = 2f;
        [SerializeField] private float upDuration = .5f;
        
        private Vector3 _originalPosition;

        private void Start()
        {
            _originalPosition = transform.position;
        }

        public void VacuumPushAnimation()
        {
            Vector3 targetPosition = new Vector3(_originalPosition.x, -.5f, _originalPosition.z);
            
            var seq = DOTween.Sequence();

            seq.Append(transform.DOMove(targetPosition, downDuration)).SetEase(Ease.OutQuad);
            seq.AppendInterval(stayDuration);
            seq.Append(transform.DOMove(_originalPosition, upDuration).SetEase(Ease.OutQuad));
        }
    }
}