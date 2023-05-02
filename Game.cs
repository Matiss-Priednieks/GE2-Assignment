using Godot;
using System;

public class Game : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    Camera PlayerCam;
    PackedScene PolarBearScene;
    public override void _Ready()
    {
        PolarBearScene = (PackedScene)ResourceLoader.Load("res://PolarBear.tscn");

        PlayerCam = GetNode<Camera>("PolarBear/Camera");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (Input.IsActionJustReleased("camSwitch"))
        {
            PlayerCam.Current = !PlayerCam.Current;
        }

    }

    public void _on_BearChecker_timeout()
    {
        var bear = 0;
        for (int i = 0; i < GetChildCount(); i++)
        {
            if (GetChild(i).Name.Contains("PolarBear"))
            {
                bear++;
            }
        }
        if (bear < 1)
        {
            GD.Print("Bear spawned");
            var polarBearInstance = PolarBearScene.Instance<PolarBear>();
            polarBearInstance.Translation = new Vector3(polarBearInstance.Translation.x, 20, polarBearInstance.Translation.y);
            CallDeferred("add_child", polarBearInstance);
        }
    }
}
