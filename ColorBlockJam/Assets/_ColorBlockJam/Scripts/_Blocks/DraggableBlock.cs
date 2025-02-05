using System;
using _ColorBlockJam.Scripts._Enums;
using _ColorBlockJam.Scripts._Level;
using _ColorBlockJam.Scripts._SFX;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace _ColorBlockJam.Scripts._Blocks
{
    public class DraggableBlock : MonoBehaviour
    {
        [Header("----- Drag Settings & Elements ------")]
        [SerializeField] private GridManager gridManager;
        [SerializeField] private Transform modelHolder;
        [SerializeField] private ParticleSystem shrinkParticle;
        [SerializeField] private LayerMask blockLayerMask;
        [SerializeField] private float xClampMin, xClampMax;
        [SerializeField] private float zClampMin, zClampMax;
        public BlockColor blockColor;
        
        [Header("----- Block Grid Dimensions ------")]
        [Tooltip("Width of the block on the grid (in tiles)")]
        [SerializeField] private int blockWidth = 1;
        [Tooltip("Height of the block on the grid (in tiles)")]
        [SerializeField] private int blockHeight = 1;

        [Header("----- Block Shrink Animation -----")] 
        [SerializeField] private float duration;
        [SerializeField] private float moveForward;
        
        private Vector3 _startPosition, _currentPosition, _offset;
        private Vector3 _lastValidPosition;
        private RaycastHit _hit;
        private Tween _tween;
        private BoxCollider[] _colliders;
        private Camera Cam => Camera.main;

        private void Awake()
        {
            _colliders = GetComponents<BoxCollider>();
        }

        private void Start()
        {
            _startPosition = transform.position;
        }

        private void OnMouseDown()
        {
            if (LevelManager.Instance.gameEnded) return;
            
            SoundManager.Instance.PlaySound(SoundManager.SoundType.Drag);
            _lastValidPosition = transform.position;
            _offset = transform.position - MouseWorldPosition();
        }

        private void OnMouseDrag()
        {
            if (LevelManager.Instance.gameEnded) return;
            
            Vector3 wantedPos = MouseWorldPosition() + _offset;
            wantedPos = new Vector3(
                Mathf.Clamp(wantedPos.x, xClampMin, xClampMax),
                .1f,
                Mathf.Clamp(wantedPos.z, zClampMin, zClampMax)
            );

            transform.position = wantedPos;

            if (IsCollidingWithOtherBlock())
            {
                transform.position = _lastValidPosition;
            }
            else
            {
                _lastValidPosition = wantedPos;
            }
        }
        
        private void OnMouseUp()
        {
            if (gridManager)
            {
                Vector3 gridOrigin = gridManager.transform.position;
                float cellSize = gridManager.GetCellSize();

                float snapOffsetX = (blockWidth % 2 == 0) ? cellSize / 2f : 0f;
                float snapOffsetZ = (blockHeight % 2 == 0) ? cellSize / 2f : 0f;

                Vector3 relativePos = transform.position - gridOrigin;

                float snappedX = Mathf.Round((relativePos.x - snapOffsetX) / cellSize) * cellSize + snapOffsetX;
                float snappedZ = Mathf.Round((relativePos.z - snapOffsetZ) / cellSize) * cellSize + snapOffsetZ;

                Vector3 targetPos = new Vector3(gridOrigin.x + snappedX, transform.position.y, gridOrigin.z + snappedZ);
                
                _tween?.Kill();
                _tween = transform.DOMove(targetPos, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    
                });
            }
            else
            {
                _tween?.Kill();
                _tween = transform.DOMove(_startPosition, 0.3f);
            }
        }
        
        private bool IsCollidingWithOtherBlock()
        {
            foreach (var col in _colliders)
            {
                Vector3 boxCenter = col.bounds.center;
                Vector3 boxExtents = col.bounds.extents;

                Collider[] hits = Physics.OverlapBox(
                    boxCenter,
                    boxExtents,
                    transform.rotation, 
                    blockLayerMask
                );

                foreach (var hit in hits)
                {
                    if (hit.gameObject != gameObject)
                        return true;
                }
            }

            return false;
        }


        public void ShrinkAndDestroyZ()
        {
            foreach (var col in _colliders)
            {
                col.enabled = false;
            }
            
            Transform modelTransform = modelHolder.transform; 
            
            var seq = DOTween.Sequence();
            
            shrinkParticle.Play();
            SoundManager.Instance.PlaySound(SoundManager.SoundType.Vacuum);
            seq.Join(
                modelTransform.DOScaleZ(0f, duration)
                    .SetEase(Ease.InOutQuad)
            );
            seq.Join(
                modelTransform.DOLocalMoveZ(modelTransform.localPosition.z + moveForward, 2f)
                    .SetEase(Ease.InOutQuad)
            );

            seq.OnComplete(() =>
                {
                    shrinkParticle.Stop();   
                    gameObject.SetActive(false);
                    SoundManager.Instance.StopSound(SoundManager.SoundType.Vacuum);
                });
        }
        
        private Vector3 MouseWorldPosition()
        {
            var mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = Cam.WorldToScreenPoint(transform.position).z;
            return Cam.ScreenToWorldPoint(mouseScreenPos);
        }
    }
}
