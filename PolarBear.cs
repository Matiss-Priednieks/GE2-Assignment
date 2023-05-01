using Godot;
using System;
using System.Collections.Generic;

public class PolarBear : KinematicBody
{
    RandomNumberGenerator RNG = new RandomNumberGenerator();
    Vector3 Gravity = new Vector3(0, -40f, 0);

    int Speed = 3;

    bool Hunting = false;
    Vector3 Direction = Vector3.Zero;
    Tween Rotator;

    Timer DirectionTimer;
    float LocalDelta;
    List<Penguin> HuntList;
    Penguin Target;
    int TargetIndex;
    bool SearchForLand = false;
    bool Rotating = true;
    public override void _Ready()
    {
        HuntList = new List<Penguin>();
        DirectionTimer = GetNode<Timer>("Timer");
        RNG.Randomize();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        RNG.Randomize();
        LocalDelta = delta;
        // RotationDegrees = new Vector3(0, RotationDegrees.y, 0);
        MoveAndCollide(Gravity * LocalDelta);

        MoveAndCollide(Direction * Speed * LocalDelta);
        CorrectRotation();

        if (Hunting)
        {
            Hunt();
        }
        else
        {
            Wander();
        }
        if (SearchForLand)
        {
            NewDirection();
        }
    }

    public void Wander()
    {
        Speed = 3;
    }
    public void Hunt()
    {
        Hunting = true;
        Speed = 20;
        if (Target == null && HuntList.Count > 0)
        {
            Target = HuntList[RNG.RandiRange(0, HuntList.Count - 1)];
        }
        else if (Target != null)
        {
            Direction = (Target.Translation - Translation).Normalized();
        }
        else
        {
            Hunting = false;
        }



    }

    public void _on_Area_body_exited(StaticBody ice)
    {
        if (ice.Name == "ice")
        {
            GD.Print("Searching");
            SearchForLand = true;
        }
    }
    public void _on_Area_body_entered(StaticBody ice)
    {
        if (ice.Name == "ice")
        {
            GD.Print("Not Searching");
            SearchForLand = false;
        }
    }

    public void Idle()
    {
    }


    public void _on_Timer_timeout()
    {
        if (!Hunting)
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


    public void _on_PolarBear_body_entered(KinematicBody body)
    {
        if (body is Penguin penguin)
        {
            HuntList.Add(penguin);
            Hunting = true;
        }
    }
    public void _on_PolarBear_body_exited(KinematicBody body)
    {
        if (body is Penguin penguin)
        {
            HuntList.Remove(penguin);
            Target = null;
        }
    }

    public void HuntSuccess()
    {
        Target = null;
    }

    public void CorrectRotation()
    {
        var localRotation = Rotation;
        localRotation.y = Mathf.LerpAngle(localRotation.y, Mathf.Atan2(-Direction.x, -Direction.z), LocalDelta * 2);
        Rotation = localRotation;
    }
}
