using Godot;
using System;

public partial class Section : Node2D
{
  [Export]
  private Polygon2D _offroad;
  [Export]
  private Path2D _road;
  private CollisionPolygon2D _collider = new();
  private Line2D _border = new();
  public override void _Ready() {
    var points = _road.Curve.GetBakedPoints();
    _collider.Polygon = points;
    _border.Points = points;
    _border.Width = 128.0f;
    AddChild(_collider);
    AddChild(_border);
  }
}
