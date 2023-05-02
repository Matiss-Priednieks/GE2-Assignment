using Godot;
using System;

public class Fish : KinematicBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    Spatial FollowPoint;
    Vector3 Direction;
    float LocalDelta;
    bool TurnRight, TurnLeft, TurnUp = false;
    int Speed = 20;
    public bool Dead = false;
    public override void _Ready()
    {
        FollowPoint = GetParent().GetNode<Spatial>("Pivot/Target");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        LocalDelta = delta;
        Direction = (FollowPoint.GlobalTranslation - Translation).Normalized();
        if (TurnRight)
        {
            Direction.x -= 0.3f;
        }
        if (TurnLeft)
        {
            Direction.x += 0.3f;
        }
        if (TurnUp)
        {
            Direction.y += 0.3f;
        }
        CorrectRotation();
        MoveAndCollide(Direction * Speed * delta);
    }
    public void _on_LeftSensor_body_entered(PhysicsBody body)
    {
        if (body is Fish fish)
        {
            TurnRight = true;
        }
    }
    public void _on_RightSensor_body_entered(PhysicsBody body)
    {
        if (body is Fish fish)
        {
            TurnLeft = true;
        }
    }
    public void _on_MiddleSensor_body_entered(PhysicsBody body)
    {
        if (body is Fish fish)
        {
            TurnUp = true;
        }
    }

    public void _on_MiddleSensor_body_exited(PhysicsBody body)
    {
        if (body is Fish fish)
        {
            TurnUp = false;
        }
    }
    public void _on_RightSensor_body_exited(PhysicsBody body)
    {
        if (body is Fish fish)
        {
            TurnLeft = false;
        }
    }
    public void _on_LeftSensor_body_exited(PhysicsBody body)
    {
        if (body is Fish fish)
        {
            TurnRight = false;
        }
    }
    public void CorrectRotation()
    {
        var localRotation = Rotation;
        localRotation.y = Mathf.LerpAngle(localRotation.y, Mathf.Atan2(Direction.x, Direction.z), LocalDelta * 2);
        // localRotation.x = Mathf.LerpAngle(localRotation.y, Mathf.Atan2(Direction.z, Direction.y), LocalDelta * 2);
        // localRotation.z = Mathf.LerpAngle(localRotation.y, Mathf.Atan2(Direction.x, Direction.y), LocalDelta * 2);
        Rotation = localRotation;
    }
}
