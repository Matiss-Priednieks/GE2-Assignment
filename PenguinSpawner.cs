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

            PenguinList.Add(penguinInstance);
        }

        for (int i = 0; i < 10; i++)
        {
            var tempTranslation = PenguinList[i].Translation;
            tempTranslation.x += 2 * i;
            tempTranslation.y = 60;
            PenguinList[i].Translation = tempTranslation;
            GetParent().CallDeferred("add_child", PenguinList[i]);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }
}
