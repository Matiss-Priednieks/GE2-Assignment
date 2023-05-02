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
        for (int i = 0; i < FishList.Count; i++)
        {
            if (FishList[i].Dead)
            {
                FishList.Remove(FishList[i]);
                FishList[i].QueueFree();
            }
        }
    }

    public void FishDied(Fish deadfish)
    {
        if (FishList.Contains(deadfish))
        {
            deadfish.Dead = true;
        }
    }
}
