using Godot;
using System;

public partial class PlayerController : Node2D
{
    [Export]
    public Racer Player;   // Minimum speed is 50% of top speed during tight turns
    [Export]
    private Camera2D _cam;   // Minimum speed is 50% of top speed during tight turns
    [Export]
    private Line2D _indicatorLine = new();
    [Export]
    private Line2D _turnLine = new();
    private Curve2D _indicatorCurve = new();   // Minimum speed is 50% of top speed during tight turns
    private Single minDistance = 50f;      // Minimum distance for control point
    private Single k = 0.5f;               // Control point distance factor
    private Vector2[] _turnPoints = new Vector2[0];
    private Double _turnSegProg;
    private Double _turnSegLen;
    private Int32 _turnIndex = 0;


    public override void _Ready()
    {
        _turnLine.Visible = false;
        _turnLine.AddPoint(new Vector2());
        _turnLine.AddPoint(new Vector2());

        _indicatorCurve.AddPoint(new Vector2());
        _indicatorCurve.AddPoint(new Vector2());
    }

    public override void _PhysicsProcess(Double delta)
    {

        var playerForward = Player.GlobalTransform.X;
        Vector2 cursorPos = GetGlobalMousePosition();
        _indicatorCurve.SetPointPosition(0, Player.GlobalPosition);
        _indicatorCurve.SetPointOut(0, playerForward * (Single)Player.Speed);
        _indicatorCurve.SetPointPosition(1, cursorPos);
        //var curveTsf = State.Instance.GetClosestPointAndRotation(cursorPos);
        //_indicatorCurve.SetPointIn(1, curveTsf.X * 256);
        var indicatorPoints = _indicatorCurve.Tessellate(3);
        _indicatorLine.Points = indicatorPoints;

        if (Input.IsActionPressed("turn"))
        {
            _turnLine.Points = indicatorPoints;
            _turnLine.Visible = true;
            _turnPoints = indicatorPoints;
            _turnIndex = 0;
            _turnSegProg = 0;
            _turnSegLen = _turnPoints[_turnIndex + 1].DistanceTo(_turnPoints[_turnIndex]);
        }
        if (_turnIndex < _turnPoints.Length - 1)
        {
            Turn(delta);
        }
        else
        {
            _turnLine.Visible = false;
            Player.Speed += Mathf.Min(Player.Accel * delta, Player.MaxSpeed - Player.Speed);
        }

        GD.Print("Speed", Player.Speed);
        Player.Velocity = playerForward * (Single)Player.Speed;

        // Apply velocity and move
        Player.MoveAndSlide();
    }
    private void Turn(Double delta)
    {

        var diff = (_turnPoints[_turnIndex + 1] - Player.GlobalPosition);
        if (_turnSegProg > _turnSegLen)
        {
            if (++_turnIndex >= _turnPoints.Length - 1)
            {
                return;
            }
            _turnSegProg = 0;
            diff = (_turnPoints[_turnIndex + 1] - Player.GlobalPosition);
            _turnSegLen = _turnPoints[_turnIndex + 1].DistanceTo(_turnPoints[_turnIndex]);
        }
        var ad = Mathf.AngleDifference(Player.GlobalRotation, diff.Angle());
        var rad = Player.RPS * delta;
        var straight = Mathf.Abs(ad) < rad;
        GD.Print(straight);
        var turn = (Single)(straight
            ? ad
            : rad * Mathf.Sign(ad));
        Player.GlobalRotation += turn;

        var targetSpeed = Player.MaxSpeed * Player.MinSpeedRatio
          + Mathf.Min(_turnSegLen * 4, Player.MaxSpeed * (1 - Player.MinSpeedRatio));
        GD.Print("target", targetSpeed, " ", targetSpeed - Player.Speed, "p", Player.Accel);

        if (Player.Speed < targetSpeed && straight)
        {
            Player.Speed += Mathf.Min(Player.Accel * delta, targetSpeed - Player.Speed);
        }
        else
        {
            Player.Speed -= Mathf.Min(Player.Decel * delta, Player.Speed - targetSpeed);
        }
        _turnSegProg += Player.Speed * delta;
    }
    private void ComputeTrajectory()
    {

    }

}
