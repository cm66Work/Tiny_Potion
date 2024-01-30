using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public event Action<int, int, T> OnValueChangedEvent;

    #region Grid Factorys
    public static GridSystem2D<T> VerticalGrid(int width, int height, float cellSize,
                                                Vector3 origin, bool debug = false)
    {
      return new GridSystem2D<T>(width, height, cellSize, origin, new VerticalConverter(), debug);
    }
    #endregion

    public GridSystem2D(int width, int height, float cellSize, Vector3 origin, CoordinateConverter coordinateConverter, bool debug)
    {
      _width = width;
      _height = height;
      _cellSize = cellSize;
      _origin = origin;
      // ?? (null coalescing) checking if this exists
      // if it is null then do the other instead
      _coordinateConverter = coordinateConverter ?? new VerticalConverter(); 

      _gridArray = new T[width,height];

      if (debug)
        DrawDebugLines();
    }

    #region Set a value from a grid position.
    public void SetValue(Vector3 worldPosition, T Value)
    {
      Vector2Int pos = _coordinateConverter.WorldToGrid(worldPosition, _cellSize, _origin);
      SetValue(pos.x, pos.y, Value);
    }

    public void SetValue(int x, int y, T Value)
    {
      if(IsValid(x, y))
      {
        _gridArray[x,y] = Value;
        OnValueChangedEvent?.Invoke(x,y,Value);
      }
    }
    #endregion

    #region Get a Value form a grid position.
    public T GetValue(Vector3 worldPosition)
    {
      Vector2Int pos = GetXY(worldPosition);
      return GetValue(pos.x,pos.y);
    }

    public T GetValue(int x, int y)
    {
      return IsValid(x,y) ? _gridArray[x,y] : default(T);
    }
    #endregion

    /// <summary>
    /// is the x and y a valid position within the grid?
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    bool IsValid(int x, int y) => x >= 0 && y >= 0 && x < _width && y < _height;

    /// <summary>
    /// Helper for getting the x and y position from a world position.
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Vector2Int GetXY(Vector3 worldPosition) => _coordinateConverter.WorldToGrid(worldPosition, _cellSize, _origin);

    /// <summary>
    /// Helper for getting the centre of a grid box
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector3 GetWorldPositionCentre(int x, int y) => _coordinateConverter.GridToWorldCentre(x, y, _cellSize, _origin);

    /// <summary>
    /// helper function for drawing and positioning our grid lines.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    Vector3 GetWorldPosition(int x, int y) => _coordinateConverter.GridToWorld(x, y, _cellSize, _origin);

    void DrawDebugLines()
    {
      // this is slow but it only has to be done once.
      const float duration = 100f;
      GameObject parent = new GameObject("Debugging");
      for (int x = 0; x < _width; x++)
      {
        for (int y = 0; y < _height; y++)
        {
          CreateWorldText(parent, $"{x},{y}", GetWorldPositionCentre(x, y), _coordinateConverter.Forward);
          Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x,y+1), Color.white, duration);
          Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, duration);
        }
      }

      Debug.DrawLine(GetWorldPosition(0,_height), GetWorldPosition(_width,_height), Color.white, duration);
      Debug.DrawLine(GetWorldPosition(_width,0), GetWorldPosition(_width,_height), Color.white, duration);
    }

    /// <summary>
    /// Helper function for creating a text mech pro text in the world.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="text"></param>
    /// <param name="position"></param>
    /// <param name="dir"></param>
    /// <param name="fontSize"></param>
    /// <param name="colour"></param>
    /// <param name="textAnchor"></param>
    /// <param name="sortingOrder"></param>
    /// <returns></returns>
    TextMeshPro CreateWorldText(GameObject parent, string text, Vector3 position, Vector3 dir,
                                int fontSize = 2, Color colour = default,
                                TextAlignmentOptions textAnchor = TextAlignmentOptions.Center,
                                int sortingOrder = 0)
    {
      GameObject gameObject = new GameObject("DebugText_" + text, typeof(TextMeshPro));
      gameObject.transform.SetParent(parent.transform);
      gameObject.transform.position = position;
      gameObject.transform.forward = dir;

      TextMeshPro textMechPro = gameObject.GetComponent<TextMeshPro>();
      textMechPro.text = text;
      textMechPro.fontSize = fontSize;
      textMechPro.color = colour = default ? Color.white : colour;
      textMechPro.alignment = textAnchor;
      textMechPro.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

      return textMechPro;
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
