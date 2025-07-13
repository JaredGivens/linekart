using Godot;
using System;

public partial class Racer : CharacterBody2D
{
    [Export]
    public Double MaxSpeed = 128f;
    [Export]
    public Double Accel = 4f;
    [Export]
    public Double Decel = 128;
    [Export]
    public Double RPS = Mathf.Pi * 0.25f; // Radians per second
    [Export]
    public Double MinSpeedRatio = 0.1;   // Minimum speed is 50% of top speed during tight turns
    public Double Speed = 0f;
    public Double AnglularVelocity;
    public override void _Ready()
    {
        MotionMode = MotionModeEnum.Floating;
    }
}
