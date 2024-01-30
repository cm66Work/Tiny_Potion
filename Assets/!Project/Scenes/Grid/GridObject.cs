namespace com.ES.Mach3
{
  /// <summary>
  /// Holds a reference to the game object that is being
  /// Created inside the Grid System.
  /// </summary>
  public class GridObject<T>
  {
    GridSystem2D<T> grid;
    int x;
    int y;

    public GridObject(GridSystem2D<T> grid, int x, int y)
    {
      this.grid = grid;
      this.x = x;
      this.y = y;
    }


  }
}