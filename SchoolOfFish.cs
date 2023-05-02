using Godot;
using System;
using System.Collections.Generic;

public class SchoolOfFish : Spatial
{
    PackedScene FishScene;
    public List<Fish> FishList;
    public override void _Ready()
    {
        FishList = new List<Fish>();
        FishScene = (PackedScene)ResourceLoader.Load("res://assets/models/fish/goodFish.tscn");
        for (int i = 0; i < 15; i++)
        {
            var fishInstance = FishScene.Instance<Fish>();
            fishInstance.Translation = new Vector3(i * Mathf.Pi, 0, i * Mathf.Pi);
            AddChild(fishInstance);
            FishList.Add(fishInstance);
        }
    }
    public override void _Process(float delta)
    {
        if (FishList.Count < 5)
        {
            for (int i = 0; i < 5; i++)
            {
                var fishInstance = FishScene.Instance<Fish>();
                fishInstance.Translation = new Vector3(i * Mathf.Pi, 0, i * Mathf.Pi);
                AddChild(fishInstance);
                FishList.Add(fishInstance);
            }
        }
    }

    public void FishDied(Fish deadfish)
    {
        for (int i = 0; i < FishList.Count; i++)
        {
            if (FishList[i] == deadfish)
            {
                GD.Print("Fish dead");
                deadfish.Dead = true;
                FishList.Remove(deadfish);
                deadfish.QueueFree();
            }
        }
    }
}
