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
        private Camera Cam => Camera.main;


        private void Start()
        {
            _startPosition = transform.position;
        }

        private void OnMouseDown()
        {
            _offset = transform.position - MouseWorldPosition();
        }

        private void OnMouseDrag()
        {
            var position = MouseWorldPosition() + _offset;
            position = new Vector3(Mathf.Clamp(position.x, xClampMin, xClampMax), 1f,
                Mathf.Clamp(position.z, zClampMin, zClampMax));
            transform.position = position;
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
        
        private Vector3 MouseWorldPosition()
        {
            var mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = Cam.WorldToScreenPoint(transform.position).z;
            return Cam.ScreenToWorldPoint(mouseScreenPos);
        }
    }
}