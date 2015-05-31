namespace Emme.Models
{
  static class DirectionExtentions
  {
    public static int Delta(this Direction direction)
    {
      return direction == Direction.Next ? 1 : -1;
    }
  }
}
