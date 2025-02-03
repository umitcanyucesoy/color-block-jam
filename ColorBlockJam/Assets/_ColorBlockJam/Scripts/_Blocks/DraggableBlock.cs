using System;
using DG.Tweening;
using UnityEngine;

namespace _ColorBlockJam.Scripts._Blocks
{
    public class DraggableBlock : MonoBehaviour
    {
        [Header("----- Drag Settings & Elements ------")]
        [SerializeField] private GridManager gridManager;
        [SerializeField] private float xClampMin, xClampMax;
        [SerializeField] private float zClampMin, zClampMax;
        
        [Header("----- Block Grid Dimensions ------")]
        [Tooltip("Width of the block on the grid (in tiles)")]
        [SerializeField] private int blockWidth = 1;
        [Tooltip("Height of the block on the grid (in tiles)")]
        [SerializeField] private int blockHeight = 1;
        
        private Vector3 _startPosition, _currentPosition, _offset;
        private RaycastHit _hit;
        private Tween _tween;
        private Vector3 _lastValidPosition;
        [SerializeField] private LayerMask blockLayerMask;
        private BoxCollider _collider;
        private Camera Cam => Camera.main;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
        }

        private void Start()
        {
            _startPosition = transform.position;
        }

        private void OnMouseDown()
        {
            _lastValidPosition = transform.position;
            _offset = transform.position - MouseWorldPosition();
        }

        private void OnMouseDrag()
        {
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
                _tween = transform.DOMove(targetPos, 0.3f).SetEase(Ease.OutQuad);
            }
            else
            {
                _tween?.Kill();
                _tween = transform.DOMove(_startPosition, 0.3f);
            }
        }
        
        private bool IsCollidingWithOtherBlock()
        {
            Vector3 boxCenter = _collider.bounds.center;
            Vector3 boxExtents = _collider.bounds.extents;

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

            return false;
        }
        
        private Vector3 MouseWorldPosition()
        {
            var mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = Cam.WorldToScreenPoint(transform.position).z;
            return Cam.ScreenToWorldPoint(mouseScreenPos);
        }
    }
}
