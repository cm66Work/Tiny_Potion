using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ES.Mach3
{
  public class GridSystem2D<T>
  {
    int _width;
    int _height;
    float _cellSize;
    Vector3 _origin;
    T[,] _gridArray;

    CoordinateConverter _coordinateConverter;

    public GridSystem2D(int width, int height, float cellSize, CoordinateConverter coordinateConverter, bool debug)
    {
      _width = width;
      _height = height;
      _cellSize = cellSize;
      // ?? (null coalescing) checking if this exists
      // if it is null then do the other instead
      _coordinateConverter = coordinateConverter ?? new VerticalConverter(); 

      _gridArray = new T[width,height];

      if (debug)
        DrawDebugLines();
    }

    void DrawDebugLines()
    {

    }

    public abstract class CoordinateConverter
    {
      public abstract Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin);
      public abstract Vector3 GridToWorldCentre(int x, int y, float cellSize, Vector3 origin);
      public abstract Vector2Int WorldToGrid(Vector3 worldPos, float cellSize, Vector3 origin);
      
      public abstract Vector3 Forward {  get; } 
    }

    /// <summary>
    /// A coordinate converter for vertical grids, where the grid lies on the X-Y plane
    /// </summary>
    public class VerticalConverter : CoordinateConverter
    {
      public override Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin)
      {
        return new Vector3(x, y, z: 0) * cellSize + origin;
      }
      public override Vector3 GridToWorldCentre(int x, int y, float cellSize, Vector3 origin)
      {
        return new Vector3(x: x * cellSize + cellSize * 0.5f, y: y * cellSize + cellSize * 0.5f, z: 0) + origin;
      }

      public override Vector2Int WorldToGrid(Vector3 worldPos, float cellSize, Vector3 origin)
      {
        Vector3 gridPos = (worldPos - origin) / cellSize;
        int x = Mathf.FloorToInt(gridPos.x);
        int y = Mathf.FloorToInt(gridPos.y);
        return new Vector2Int(x, y);
      }

      public override Vector3 Forward => Vector3.forward;
    }

  }
}
