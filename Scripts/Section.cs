using Godot;
using System;

public partial class Section : Node2D
{
    [Export]
    public Polygon2D offroad;
    [Export]
    public Godot.Collections.Array<Polygon2D> offroad2;
    [Export]
    public Godot.Collections.Array<Polygon2D> boost;
    [Export]
    public Path2D Road;
}
