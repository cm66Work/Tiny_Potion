using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ES.Mach3
{
  public class Mach3 : MonoBehaviour
  {
    [SerializeField] int _width = 8;
    [SerializeField] int _height = 8;
    [SerializeField] float _cellSize = 1f;
    [SerializeField] Vector3 _originPosition = Vector3.zero;
    [SerializeField] bool _debug = true;

    GridSystem2D<GridObject<Gem>> _grid;

    void Start()
    {
      _grid = GridSystem2D<GridObject<Gem>>.VerticalGrid(_width, _height, _cellSize, _originPosition, _debug);
    }
  }
}
