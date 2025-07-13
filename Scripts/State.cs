using Godot;
using System.Collections.Generic;

public partial class State
{
    private static State _instance;
    public static State Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new State();
            }
            return _instance;
        }
    }

    public Line2D RoadLine;
    public Area2D RoadArea = new();
    public Curve2D RoadCurve = new();

    State()
    {
        // Initialize Line2D
        RoadLine = new Line2D
        {
            Width = 128.0f, // Thick line, adjust as needed
            DefaultColor = Colors.White // Adjust color as needed
        };
    }

    // Combines an array of Path2D nodes into a single Curve2D
    public void SetupTrack(Section[] sections)
    {

        // Combine curves in order
        foreach (var sec in sections)
        {
            if (sec == null || sec.Road.Curve == null)
                continue;

            for (int i = 0; i < sec.Road.Curve.PointCount; i++)
            {
                var pointPos = sec.Road.Curve.GetPointPosition(i);
                var pointIn = sec.Road.Curve.GetPointIn(i);
                var pointOut = sec.Road.Curve.GetPointOut(i);

                // Apply Path2D's global transform
                var globalPos = sec.GlobalTransform * pointPos;
                var globalIn = sec.GlobalTransform.BasisXform(pointIn);
                var globalOut = sec.GlobalTransform.BasisXform(pointOut);

                RoadCurve.AddPoint(globalPos, globalIn, globalOut);
            }
        }

        // Update visuals and collider
        var points = RoadCurve.GetBakedPoints();
        RoadLine.Points = points;
        var polygons = Geometry2D.OffsetPolyline(points, RoadLine.Width);

        GD.Print(polygons.Count);
        foreach (var poly in polygons)
        {
            var collider = new CollisionPolygon2D { Polygon = poly };

            RoadArea.AddChild(collider);
        }
    }

    // Example: Get the closest point and rotation to a position (e.g., mouse)
    public Transform2D GetClosestPointAndRotation(Vector2 worldPos)
    {
        var offset = RoadCurve.GetClosestOffset(worldPos);
        return RoadCurve.SampleBakedWithRotation(offset, false);
    }
}
