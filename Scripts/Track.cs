using Godot;
using System;
using System.Collections.Generic;
enum SecEnum
{
    kTopLeft,
    kTopRight,
    kBottomRight,
    kBottomLeft,
    kAmt
}
public partial class Track : Node2D
{

    [Export]
    private PlayerController _pc;
    private static Vector2[] SectionPos = {

    }

    private static List<PackedScene>[] _packedSections;
    private CollisionPolygon2D _collider = new();
    private Line2D _border = new();

    public override void _Ready()
    {
        // Load scene paths from directories
        if (_packedSections == null)
        {
            LoadScenePaths();
        }
        // Set up background
        // Set up car
        // Generate initial track
        GenerateTrack();
    }
    private static List<PackedScene> LoadScenes(String dirname)
    {
        var res = new List<PackedScene>();
        var fns = DirAccess.GetFilesAt(dirname);
        foreach (var fn in fns)
        {
            var nameEnd = fn.IndexOf(".tscn");
            if (nameEnd == -1)
            {
                continue;
            }
            res.Add(GD.Load<PackedScene>($"{dirname}/{fn.Substr(0, nameEnd)}.tscn"));
        }
        return res;
    }

    static private void LoadScenePaths()
    {
        GD.Print("loaing sections");
        _packedSections = new List<PackedScene>[(Int32)SecEnum.kAmt];
        _packedSections[(Int32)SecEnum.kBottomLeft] = LoadScenes("res://Bl_section");
        _packedSections[(Int32)SecEnum.kBottomRight] = LoadScenes("res://Br_section");
        _packedSections[(Int32)SecEnum.kTopRight] = LoadScenes("res://Tr_section");
        _packedSections[(Int32)SecEnum.kTopLeft] = LoadScenes("res://Tl_section");
    }

    private void GenerateTrack()
    {
        GD.Print("generating track");
        Section[] sections = new Section[(Int32)SecEnum.kAmt];
        // Instantiate one scene per quadrant
        for (Int32 i = 0; i < (Int32)SecEnum.kAmt; ++i)
        {
            var packed = _packedSections[i][GD.RandRange(0, _packedSections[i].Count - 1)];
            var section = packed.Instantiate<Section>();
            section.Position = new Vector2((i & 1) * 1024, (i >> 1) * 1024);
            AddChild(section);
            sections[i] = section;
        }
        State.Instance.SetupTrack(sections);
        AddChild(State.Instance.RoadLine);
        AddChild(State.Instance.RoadArea);
        _pc.Player.Position = new Vector2(256, 512);
    }

    public override void _PhysicsProcess(double delta)
    {

    }
}
