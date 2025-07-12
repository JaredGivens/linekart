using Godot;
using System;
public partial class Track : Node2D
{

    public override void _Ready()
    {
        // Load scene paths from directories
        LoadScenePaths();
        // Set up background
        // Set up car
        // Generate initial track
        GenerateTrack();
    }

    private void LoadScenePaths()
    {
        _blScenes = DirAccess.GetFilesAt("res://Bl_section");
        _brScenes = DirAccess.GetFilesAt("res://Br_section");
        _trScenes = DirAccess.GetFilesAt("res://Tr_section");
        _tlScenes = DirAccess.GetFilesAt("res://Tl_section");

        GD.Print($"Loaded {blScenes.Count} BL, {brScenes.Count} BR, {trScenes.Count} TR, {tlScenes.Count} TL scenes");
    }

    private void GenerateTrack()
    {
        // Clear previous sections
        foreach (var section in loadedSections)
        {
            section.QueueFree();
        }
        loadedSections.Clear();

        // Set random background color
        background.Color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());

        // Instantiate one scene per quadrant
        if (blScenes.Count > 0)
        {
            var blScene = GD.Load<PackedScene>(blScenes[random.Next(blScenes.Count)]);
            var blInstance = blScene.Instantiate<Node2D>();
            blInstance.Position = new Vector2(0, 1024); // Bottom-left
            AddChild(blInstance);
            loadedSections.Add(blInstance);
        }
        if (brScenes.Count > 0)
        {
            var brScene = GD.Load<PackedScene>(brScenes[random.Next(brScenes.Count)]);
            var brInstance = brScene.Instantiate<Node2D>();
            brInstance.Position = new Vector2(1024, 1024); // Bottom-right
            AddChild(brInstance);
            loadedSections.Add(brInstance);
        }
        if (trScenes.Count > 0)
        {
            var trScene = GD.Load<PackedScene>(trScenes[random.Next(trScenes.Count)]);
            var trInstance = trScene.Instantiate<Node2D>();
            trInstance.Position = new Vector2(1024, 0); // Top-right
            AddChild(trInstance);
            loadedSections.Add(trInstance);
        }
        if (tlScenes.Count > 0)
        {
            var tlScene = GD.Load<PackedScene>(tlScenes[random.Next(tlScenes.Count)]);
            var tlInstance = tlScene.Instantiate<Node2D>();
            tlInstance.Position = new Vector2(0, 0); // Top-left
            AddChild(tlInstance);
            loadedSections.Add(tlInstance);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.Space)
        {
            GenerateTrack();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        // Basic car movement
        float speed = 0;
        float maxSpeed = 300;
        float rotationSpeed = 2.0f;

        if (Input.IsActionPressed("ui_up"))
            speed = maxSpeed;
        else if (Input.IsActionPressed("ui_down"))
            speed = -maxSpeed / 2;
        if (Input.IsActionPressed("ui_left"))
            car.Rotation -= rotationSpeed * (float)delta;
        if (Input.IsActionPressed("ui_right"))
            car.Rotation += rotationSpeed * (float)delta;

        var velocity = new Vector2(Mathf.Cos(car.Rotation), Mathf.Sin(car.Rotation)) * speed;
        car.Velocity = velocity;
        car.MoveAndSlide();

        // Keep car within track bounds
        car.Position = new Vector2(
            Mathf.Clamp(car.Position.X, 0, 2048),
            Mathf.Clamp(car.Position.Y, 0, 2048)
        );
    }
}
