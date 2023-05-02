using Godot;
using System;
using System.Collections.Generic;

public class PolarBear : KinematicBody
{
    RandomNumberGenerator RNG = new RandomNumberGenerator();
    Vector3 Gravity = new Vector3(0, -40f, 0);
    AnimationPlayer Animator;

    float Speed = 3;

    bool Hunting = false;
    Vector3 Direction = Vector3.Zero;
    Tween Rotator;
    Camera BearCam;

    Timer DirectionTimer;
    float LocalDelta;
    List<Penguin> HuntList;
    Penguin Target;
    int TargetIndex;
    bool SearchForLand, Attacking = false;
    bool Rotating = true;

    int Hunger = 50;
    Label3D HungerLabel;
    public override void _Ready()
    {
        BearCam = GetNode<Camera>("Camera");
        HungerLabel = GetNode<Label3D>("Label3D");

        Animator = GetNode<AnimationPlayer>("smoothPolarBear/Skeleton/AnimationPlayer");
        HuntList = new List<Penguin>();
        DirectionTimer = GetNode<Timer>("Timer");
        RNG.Randomize();
        NewDirection();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        if (BearCam.Current)
        {
            HungerLabel.Hide();
        }
        else
        {
            HungerLabel.Show();
        }

        RNG.Randomize();
        LocalDelta = delta;
        MoveAndCollide(Gravity * LocalDelta);

        MoveAndCollide(Direction * Speed * LocalDelta);
        CorrectRotation();

        if (Hunting)
        {
            Hunt();
        }
        else if (!Hunting)
        {
            Wander();
        }
    }

    public void Wander()
    {
        if (Hunger <= 20)
        {
            Animator.PlaybackSpeed = 1;
            Speed = 7f;
            HungerLabel.Text = "Starving";
            HungerLabel.OutlineModulate = new Color("#c40000");
        }
        else if (Hunger <= 70)
        {
            Animator.PlaybackSpeed = 1;
            Speed = 4f;
            HungerLabel.Text = "Hungry";
            HungerLabel.OutlineModulate = new Color("#cf962b");
        }
        else
        {
            Speed = 0.5f;
            if (Animator.CurrentAnimation != "attack")
            {
                Animator.PlaybackSpeed = 0.1f;
            }
            HungerLabel.Text = "Full";
            HungerLabel.OutlineModulate = new Color("#0076ff");
        }

        if (Direction == Vector3.Zero)
        {
            if (Hunger > 20)
            {
                Idle();
            }
            else
            {
                NewDirection();
            }
        }
        else
        {
            if (!Attacking)
            {
                Animator.Play("walking_loop");
            }
        }
    }

    public void Hunt()
    {
        if (!Attacking)
        {
            Animator.Play("running_loop");
        }
        Hunting = true;
        if (Hunger <= 20)
        {
            Speed = 12;
        }
        else if (Hunger <= 70)
        {
            Speed = 6;
        }
        else
        {
            Speed = 3;
        }
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
            Direction = -Direction;
        }
    }
    public void _on_Area_body_entered(StaticBody ice)
    {

    }
    public void _on_Area3_body_exited(StaticBody ice)
    {
        Direction = -Direction;
    }
    public void _on_Area2_body_exited(StaticBody ice)
    {
        Direction = -Direction;
    }

    public void Idle()
    {
        if (!Attacking)
        {
            Animator.Play("idle_loop");
        }
    }


    public void _on_Timer_timeout()
    {
        GD.Print(Hunger);
        if (Hunger > 0)
        {
            Hunger -= 10;
        }
        else
        {
            this.QueueFree();
        }
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
        Animator.PlaybackSpeed = 1;
        Animator.Play("attack");
        Attacking = true;
        if (Hunger < 100)
        {
            Hunger += 10;
            if (Hunger > 100) { Hunger = 100; }
        }

        Target = null;
    }

    public void _on_AnimationPlayer_animation_finished(String animName)
    {
        if (animName == "attack")
        {
            Attacking = false;
        }
    }
    public void CorrectRotation()
    {
        var localRotation = Rotation;
        localRotation.y = Mathf.LerpAngle(localRotation.y, Mathf.Atan2(-Direction.x, -Direction.z), LocalDelta * 2);
        Rotation = localRotation;
    }
}
