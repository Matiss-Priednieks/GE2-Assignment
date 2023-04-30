using Godot;
using System;
using System.Collections.Generic;

public class Penguin : KinematicBody
{
    RandomNumberGenerator RNG = new RandomNumberGenerator();
    AnimationPlayer Animator;
    Vector3 Gravity = new Vector3(0, -40f, 0);
    int Health = 100;
    int Hunger = 0;
    int Speed = 2;
    bool Scared = false;

    Vector3 Direction = Vector3.Forward;
    public List<Penguin> Penguins;
    Tween Rotator;

    Timer DirectionTimer;
    float LocalDelta;

    bool Rotating = true;
    public override void _Ready()
    {
        Animator = GetNode<AnimationPlayer>("Penguin-with-anim/AnimationPlayer");
        DirectionTimer = GetNode<Timer>("Timer");
        RNG.Randomize();
        Hunger = RNG.RandiRange(0, 100);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        LocalDelta = delta;
        RotationDegrees = new Vector3(0, RotationDegrees.y, 0);
        MoveAndCollide(Gravity * LocalDelta);

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
            Wander();
        }
        MoveAndCollide(Direction * Speed * LocalDelta);

    }

    public void Wander()
    {
        Speed = 2;
        Animator.Play("Walk-loop");
        var localRotation = Rotation;
        localRotation.y = Mathf.LerpAngle(localRotation.y, Mathf.Atan2(-Direction.x, -Direction.z), LocalDelta * 2);
        Rotation = localRotation;
    }
    public void Hunt()
    {

    }
    public void Run(Vector3 bearLocation)
    {
        Speed = 6;
        Direction = new Vector3(Translation.x - bearLocation.x, 0, Translation.z - bearLocation.z).Normalized();
        var localRotation = Rotation;
        localRotation.y = Mathf.LerpAngle(localRotation.y, Mathf.Atan2(-Direction.x, -Direction.z), LocalDelta * 2);

        Rotation = localRotation;
        Animator.PlaybackSpeed = 2;
        Animator.Play("Walk-loop");
    }
    public void Idle()
    {
        Animator.Play("Idle");
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
        Hunger--;
        if (!Scared)
        {
            NewDirection();
        }
    }
    public void NewDirection()
    {
        RNG.Randomize();
        Direction = new Vector3(RNG.RandiRange(-1, 1), 0, RNG.RandiRange(-1, 1));
        Rotating = true;
    }

    public void _on_Area_area_entered(Area area)
    {
        if (area.Name == "Ocean")
        {
            Gravity = new Vector3(0, -5f, 0);
        }
        if (area.Name == "PolarBear")
        {
            Scared = true;
            Run(area.Translation);
        }
    }

    public void _on_Area_area_exited(Area area)
    {
        if (area.Name == "Ocean")
        {
            Gravity = new Vector3(0, -40f, 0);
        }
        if (area.Name == "PolarBear")
        {
            Scared = false;
        }
    }

    public void _on_Area_body_entered(KinematicBody body)
    {
        if (body is PolarBear bear)
        {
            //maybe send signal to bear?

            GD.Print("Death");
            this.QueueFree();
        }
    }
}
