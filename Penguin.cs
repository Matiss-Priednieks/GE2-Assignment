using Godot;
using System;
using System.Collections.Generic;

public class Penguin : KinematicBody
{
    RandomNumberGenerator RNG = new RandomNumberGenerator();
    Vector3 Gravity = new Vector3(0, -40f, 0);
    int Health = 100;
    int Hunger = 0;
    int Speed = 2;

    Vector3 Direction = Vector3.Forward;
    public List<Penguin> Penguins;
    Tween Rotator;
    int Distance = 2;
    Timer DirectionTimer;

    public override void _Ready()
    {
        DirectionTimer = GetNode<Timer>("Timer");
        Rotator = GetNode<Tween>("Rotate");
        RNG.Randomize();
        Hunger = RNG.RandiRange(0, 100);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        RotationDegrees = new Vector3(0, RotationDegrees.y, 0);
        MoveAndCollide(Gravity * delta);

        if (Hunger >= 60)
        {
            Hunt();
        }
        if (Health <= 20 && Health > 0 && Hunger < 50)
        {
            Rest();
        }
        else if (Health > 20 && Health < 100)
        {
            Heal();
        }
        else
        {
            Wander(delta);
        }
    }

    public void Wander(float delta)
    {
        //look at and go to direction * distance - current position
        MoveAndCollide(Direction * Speed * delta);
        LookAt(Direction + Translation, Vector3.Up);
        // var _targetRotation = new Quat(Vector3.Up, Direction);

        // // smoothly interpolate between the current rotation and the target rotation
        // Rotation = Rotation.Slerp(_targetRotation, 0.1f);

        // // update the player's position
        // Position = newPos;
    }
    public void Hunt()
    {

    }
    public void Run()
    {

    }
    public void Rest()
    {
        Heal();
    }
    public async void Heal()
    {
        await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
        Health++;
    }

    public void _on_Timer_timeout()
    {
        NewDirection();
    }
    public void NewDirection()
    {
        RNG.Randomize();
        Direction = new Vector3(RNG.RandiRange(-1, 1), 0, RNG.RandiRange(-1, 1));
        GD.Print(Direction);
    }

    public void _on_Area_area_entered(Area area)
    {
        if (area.Name == "Ocean")
        {
            GD.Print("test");
            Gravity = new Vector3(0, -5f, 0);
        }
    }

    public void _on_Area_area_exited(Area area)
    {
        if (area.Name == "Ocean")
        {
            GD.Print("test2");
            Gravity = new Vector3(0, -40f, 0);
        }
    }
}
