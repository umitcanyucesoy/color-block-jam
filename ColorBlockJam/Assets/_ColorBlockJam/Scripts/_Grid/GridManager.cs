using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
   [Header("------ Grid Settings -------")]
   [SerializeField] private int gridWidth = 6;
   [SerializeField] private int gridHeight = 6;
   [SerializeField] private float cellSize = 1.1f;
   
   [Header("------ Grid Elements -------")]
   [SerializeField] private GameObject gridPrefab;
   
   private readonly Vector3 _gridOriginOffset = Vector3.zero;
   private GameObject[,] _gridArray;

   private void Start()
   {
      CreateGrid();
   }

   private void CreateGrid()
   {
      _gridArray = new GameObject[gridWidth, gridHeight];

      for (int x = 0; x < gridWidth; x++)
      {
         for (int z = 0; z < gridHeight; z++)
         {
            Vector3 gridPos = new Vector3(x * cellSize, 0, z * cellSize);
            GameObject grid = Instantiate(gridPrefab, gridPos, Quaternion.identity, transform);
            grid.name = $"Cell_{x}_{z}";
            
            _gridArray[x, z] = grid;
         }
      }
   }
   
   public float GetCellSize() => cellSize;
   
   private void OnDrawGizmos()
   {
      Gizmos.color = Color.green;

      Vector3 gridOrigin = transform.position + _gridOriginOffset;

      for (int x = 0; x < gridWidth; x++)
      {
         for (int z = 0; z < gridHeight; z++)
         {
            Vector3 cellCenter = gridOrigin + new Vector3(x * cellSize, 0, z * cellSize);

            Vector3 cellDimensions = new Vector3(cellSize, 0.01f, cellSize);

            Gizmos.DrawWireCube(cellCenter, cellDimensions);
         }
      }
   }
   
}
