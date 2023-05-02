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
    bool Scared, InWater = false;

    bool SearchingForLand = false;
    Vector3 Direction = Vector3.Forward;
    public List<Penguin> Penguins;
    Tween Rotator;

    KinematicBody PolarBearReference;
    Timer DirectionTimer;
    float LocalDelta;
    bool SignalConnected, Hunting = false;
    SchoolOfFish[] FishSchool;
    Fish Target;
    int TargetSchool;
    Spatial[] WaterExits;

    bool Choosing = false;

    int ExitTarget;
    public override void _Ready()
    {
        Animator = GetNode<AnimationPlayer>("Penguin-with-anim/AnimationPlayer");
        DirectionTimer = GetNode<Timer>("Timer");
        RNG.Randomize();
        Hunger = RNG.RandiRange(40, 90);

        WaterExits = new Spatial[4];
        for (int i = 0; i < WaterExits.Length; i++)
        {
            WaterExits[i] = GetNode<Spatial>("../../iceberg/CollectionPoints/CollectionPoint" + i);
        }
        ExitTarget = RNG.RandiRange(0, WaterExits.Length - 1);

        FishSchool = new SchoolOfFish[2];
        FishSchool[0] = GetNode<SchoolOfFish>("../../FishSchool1");
        FishSchool[1] = GetNode<SchoolOfFish>("../../FishSchool2");

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
        if (!InWater)
        {
            RotationDegrees = new Vector3(0, RotationDegrees.y, 0);
        }
        else if (InWater && !Hunting)
        {
            Direction = (WaterExits[ExitTarget].GlobalTranslation - GlobalTranslation).Normalized();
            if (GlobalTranslation.DistanceTo(WaterExits[ExitTarget].GlobalTranslation) < 15)
            {
                Translation = new Vector3(WaterExits[ExitTarget].GlobalTranslation.x, WaterExits[ExitTarget].GlobalTranslation.y + 10, WaterExits[ExitTarget].GlobalTranslation.z);
            }
        }
        MoveAndCollide(Gravity * LocalDelta);

        if (Hunger <= 60)
        {
            Hunting = true;
            Hunt();
        }
        if (!Scared && !Hunting)
        {
            Wander();
        }

        MoveAndCollide(Direction * Speed * LocalDelta);
        if (!InWater)
        {
            CorrectRotation();
        }
        else
        {
            CorrectWaterRotation();
        }

    }

    public void Wander()
    {
        if (InWater)
        {
            Speed = 10;
        }
        else
        {
            Speed = 2;
        }
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
        if (!InWater)
        {
            Speed = 10;
            Animator.PlaybackSpeed = 4;
            Animator.Play("Walk-loop");
        }
        else
        {
            Speed = 30;
        }
        if ((!Choosing && Target == null) || (!Choosing && Target.Dead))
        {
            Choosing = true;
        }
        if (Choosing)
        {
            RNG.Randomize();
            TargetSchool = RNG.RandiRange(0, FishSchool.Length - 1);
            Target = FishSchool[TargetSchool].FishList[0];
            Choosing = false;
        }
        if (Target != null)
        {
            if (!InWater)
            {
                Direction = new Vector3(Target.Translation.x - Translation.x, 0, Target.Translation.z - Translation.z).Normalized();
            }
            else
            {
                Direction = (Target.GlobalTranslation - GlobalTranslation).Normalized();
            }
        }
    }
    public void Run(Vector3 bearLocation)
    {
        Speed = 6;
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
        if (Hunger > 0)
        {
            Hunger--;
        }
        else if (Hunger < 0)
        {
            this.QueueFree();
        }
        if (!Scared)
        {
            NewDirection();
        }
    }
    public void NewDirection()
    {
        if (!Hunting)
        {
            RNG.Randomize();
            Direction = new Vector3(RNG.RandiRange(-1, 1), 0, RNG.RandiRange(-1, 1));
        }
    }

    public void _on_Area_area_entered(Area area)
    {
        if (area.Name == "OceanArea")
        {
            Gravity = new Vector3(0, -0.01f, 0);
            InWater = true;
        }
        if (area.Name == "PolarBear")
        {
            Scared = true;
            Run(area.Translation);
        }
    }

    public void _on_Area_area_exited(Area area)
    {
        if (area.Name == "OceanArea")
        {
            Gravity = new Vector3(0, -40f, 0);
            InWater = false;
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
            if (!InWater)
            {
                Direction = -Direction;
            }
            else
            {
                Direction.x += 1.5f;
                Direction.z += 1.5f;
            }

        }
        if (body is Fish fish)
        {
            Hunger = 100;
            FishSchool[TargetSchool].FishDied(Target);
            FishSchool[TargetSchool].FishList.Remove(Target);
            Hunting = false;
            Target = null;
        }
    }

    public void CorrectRotation()
    {
        var localRotation = Rotation;
        localRotation.y = Mathf.LerpAngle(localRotation.y, Mathf.Atan2(-Direction.x, -Direction.z), LocalDelta * 2);
        Rotation = localRotation;
    }
    public void CorrectWaterRotation()
    {
        var localRotation = Rotation;
        localRotation.y = Mathf.LerpAngle(localRotation.y, Mathf.Atan2(-Direction.x, -Direction.z), LocalDelta * 2);
        localRotation.x = Mathf.LerpAngle(localRotation.x, Mathf.Atan2(Direction.y, Direction.z), LocalDelta * 2);
        Rotation = localRotation;
    }

    public void _on_Area4_body_exited(PhysicsBody body)
    {
        if (body.Name == "ice")
        {
            if (!Hunting)
            {
                Direction = -Direction;
            }
            else
            {
                Animator.Play("Dive");
            }
        }
    }
    public void _on_Area3_body_exited(PhysicsBody body)
    {
        if (body.Name == "ice")
        {
            if (!Hunting)
            {
                Direction = -Direction;
            }
            else
            {
                Animator.Play("Dive");
            }
        }
    }
    public void _on_Area2_body_exited(PhysicsBody body)
    {
        if (body.Name == "ice")
        {
            if (!Hunting)
            {
                Direction = -Direction;
            }
            else
            {
                Animator.Play("Dive");
            }
        }
    }
    public void _on_Area2_body_entered(PhysicsBody body)
    {
        if (body.Name == "ice")
        {
        }
        if (body.Name.Contains("Penguin"))
        {
            Direction.x += 1.5f;
            Direction.z += 1.5f;
        }
    }
}
