using Godot;
using System;
using System.Collections.Generic;

public class PenguinSpawner : Node
{
    PackedScene Penguin;
    List<KinematicBody> PenguinList;

    int MaxPenugins = 100;


    public override void _Ready()
    {
        PenguinList = new List<KinematicBody>();

        Penguin = (PackedScene)ResourceLoader.Load("res://Penguin.tscn");
        for (int i = 0; i < MaxPenugins; i++)
        {
            var penguinInstance = Penguin.Instance<KinematicBody>();

            //TODO: Fix this, for some reason the penguins are not separating and thus causing each other to hit off of each other and fly off the map.
            var tempTranslation = penguinInstance.Translation;
            tempTranslation.x += 20;
            tempTranslation.y = 60;
            penguinInstance.Translation = tempTranslation;

            PenguinList.Add(penguinInstance);
        }

        for (int i = 0; i < PenguinList.Count; i++)
        {
            GetParent().CallDeferred("add_child", PenguinList[i]);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }
}
