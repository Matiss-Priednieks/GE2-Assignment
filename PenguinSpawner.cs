using Godot;
using System;
using System.Collections.Generic;

public class PenguinSpawner : Node
{
    PackedScene Penguin;
    List<Penguin> PenguinList;

    int MaxPenugins = 100;


    public override void _Ready()
    {
        GD.Randomize();
        PenguinList = new List<Penguin>();

        Penguin = (PackedScene)ResourceLoader.Load("res://Penguin.tscn");
        for (int i = 0; i < MaxPenugins; i++)
        {
            var penguinInstance = Penguin.Instance<Penguin>();
            PenguinList.Add(penguinInstance);
        }

        for (int i = 0; i < 10; i++)
        {
            var tempTranslation = PenguinList[i].Translation;
            tempTranslation.x += (float)GD.RandRange(-5, 3) * (i);
            tempTranslation.z += (float)GD.RandRange(-5, 3) * (i);
            tempTranslation.y = 20;
            PenguinList[i].Translation = tempTranslation;
            PenguinList[i].Penguins = PenguinList;
            CallDeferred("add_child", PenguinList[i]);
            MaxPenugins--;
            PenguinList.RemoveAt(i);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }
    public void _on_Timer_timeout()
    {
        if (GetChildren().Count < 20)
        {
            for (int i = 0; i < 1; i++)
            {
                var penguinInstance = Penguin.Instance<Penguin>();
                PenguinList.Add(penguinInstance);
                GD.Randomize();
                var tempTranslation = PenguinList[i].Translation;
                tempTranslation.x += (float)GD.RandRange(-50, 30);
                tempTranslation.z += (float)GD.RandRange(-50, 30);
                tempTranslation.y = 20;
                PenguinList[i].Translation = tempTranslation;
                PenguinList[i].Penguins = PenguinList;
                CallDeferred("add_child", PenguinList[i]);
                MaxPenugins--;
                PenguinList.RemoveAt(i);
            }
        }
    }
}
