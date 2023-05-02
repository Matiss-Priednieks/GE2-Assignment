using Godot;
using System;
using System.Collections.Generic;

public class Penguin : KinematicBody
{
    [Signal] public delegate void Death();
    RandomNumberGenerator RNG = new RandomNumberGenerator();
    AnimationPlayer Animator;
    Vector3 Gravity = new Vector3(0, -40f, 0);
    int Health = 100;
    int Hunger = 100;
    int Speed = 2;
    bool Scared = false;

    bool SearchingForLand = false;
    Vector3 Direction = Vector3.Forward;
    public List<Penguin> Penguins;
    Tween Rotator;

    KinematicBody PolarBearReference;
    Timer DirectionTimer;
    float LocalDelta;
    bool SignalConnected = false;

    bool Rotating = true;
    public override void _Ready()
    {
        Animator = GetNode<AnimationPlayer>("Penguin-with-anim/AnimationPlayer");
        DirectionTimer = GetNode<Timer>("Timer");
        RNG.Randomize();
        Hunger = RNG.RandiRange(0, 100);
        NewDirection();
    }

    public override void _Process(float delta)
    {
        var bear = 0;
        for (int i = 0; i < GetParent().GetParent().GetChildCount(); i++)
        {
            if (GetParent().GetParent().GetChild(i).Name.Contains("PolarBear"))
            {
                bear++;
            }
        }
        if (bear < 1)
        {
            SignalConnected = false;
        }
        else if (!SignalConnected)
        {
            PolarBearReference = GetNode<KinematicBody>("../../PolarBear");
            this.Connect("Death", PolarBearReference, "HuntSuccess");
            SignalConnected = true;
        }

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        LocalDelta = delta;
        RotationDegrees = new Vector3(0, RotationDegrees.y, 0);
        MoveAndCollide(Gravity * LocalDelta);

        if (Hunger <= 60)
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
        else if (!Scared)
        {
            Wander();
        }
        if (SearchingForLand)
        {
            NewDirection();
        }

        MoveAndCollide(Direction * Speed * LocalDelta);
        CorrectRotation();

    }

    public void Wander()
    {
        Speed = 2;
        if (Direction == Vector3.Zero)
        {
            Animator.PlaybackSpeed = 0.6f;
            Idle();
        }
        else
        {
            Animator.PlaybackSpeed = 1;
            Animator.Play("Walk-loop");
        }
    }
    public void Hunt()
    {
    }
    public void Run(Vector3 bearLocation)
    {
        Speed = 6;
        // GD.Print("Scared");
        Direction = (Translation - bearLocation).Normalized();
        Direction.y = 0;
        Animator.PlaybackSpeed = 2;
        Animator.Play("Walk-loop");
    }
    public void Idle()
    {
        Animator.Play("Idle-loop");
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
            Gravity = new Vector3(0, -0.5f, 0);
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
            EmitSignal("Death");
            this.QueueFree();
        }
        if (body is Penguin pengu)
        {
            Direction = -Direction;
        }
    }

    public void CorrectRotation()
    {
        var localRotation = Rotation;
        localRotation.y = Mathf.LerpAngle(localRotation.y, Mathf.Atan2(-Direction.x, -Direction.z), LocalDelta * 2);
        Rotation = localRotation;
    }

    public void _on_Area4_body_exited(PhysicsBody body)
    {
        if (body.Name == "ice")
        {
            Direction = -Direction;
        }
    }
    public void _on_Area3_body_exited(PhysicsBody body)
    {
        if (body.Name == "ice")
        {
            Direction = -Direction;
        }
    }
    public void _on_Area2_body_exited(PhysicsBody body)
    {
        if (body.Name == "ice")
        {
            Direction = -Direction;
        }
    }
    public void _on_Area2_body_entered(PhysicsBody body)
    {
        if (body.Name == "ice")
        {

        }
    }
}
