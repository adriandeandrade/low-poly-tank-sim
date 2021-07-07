using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnProjectileCollision(ProjectileCollisionEvent e);

public interface IProjectile
{
    OnProjectileCollision OnCollision { get; set; }
    Vector3 Velocity { get; set; }
    Vector3 Position { get; }
    Quaternion Rotation { get; }
    void Deactivate();
}
