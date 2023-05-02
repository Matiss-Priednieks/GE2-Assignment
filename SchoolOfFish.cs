using Godot;
using System;

public class SchoolOfFish : Spatial
{
    PackedScene FishScene;
    public override void _Ready()
    {
        FishScene = (PackedScene)ResourceLoader.Load("res://assets/models/fish/goodFish.tscn");
        for (int i = 0; i < 15; i++)
        {
            var fishInstance = FishScene.Instance<Fish>();
            fishInstance.Translation = new Vector3(i * Mathf.Pi, 0, i * Mathf.Pi);
            AddChild(fishInstance);
        }
    }

}
