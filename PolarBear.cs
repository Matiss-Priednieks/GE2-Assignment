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
    Vector3 Target = Vector3.Zero;

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
        LocalDelta = delta;
        // RotationDegrees = new Vector3(0, RotationDegrees.y, 0);
        MoveAndCollide(Gravity * LocalDelta);

        MoveAndCollide(Direction * Speed * LocalDelta);
        if (Hunting)
        {
            Hunt();
        }
        else
        {
            Wander();
        }
    }

    public void Wander()
    {
        Speed = 3;
        var localRotation = Rotation;
        localRotation.y = Mathf.LerpAngle(localRotation.y, Mathf.Atan2(-Direction.x, -Direction.z), LocalDelta * 2);
        Rotation = localRotation;
    }
    public void Hunt()
    {
        Hunting = true;
        Speed = 5;

        var localRotation = Rotation;
        localRotation.y = Mathf.LerpAngle(localRotation.y, Mathf.Atan2(-Direction.x, -Direction.z), LocalDelta * 2);
        Rotation = localRotation;


        //TODO: This code needs to account for when a penguin is caught.
        if (Target == Vector3.Zero)
        {
            Target = HuntList[RNG.RandiRange(0, HuntList.Count - 1)].Translation;
        }
        else
        {
            // GD.Print(Target);
        }


        Direction = new Vector3(Translation.x - Target.x, 0, Translation.y - Target.y).Normalized();

        // GD.Print(HuntList[0].Translation);
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
            Hunt();
        }
    }
    public void _on_PolarBear_body_exited(KinematicBody body)
    {
        if (body is Penguin penguin)
        {

        }
    }

}
