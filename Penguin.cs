using Godot;
using System;

public class Penguin : KinematicBody
{
    Vector3 Gravity = new Vector3(0, -9.8f, 0);
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        MoveAndCollide(Gravity * delta);
    }
}
