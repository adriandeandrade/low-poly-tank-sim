using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ProjectileCollisionEvent
{
    public readonly IProjectile projectile;
    public readonly Collider collider;
    public readonly int damage;

    public ProjectileCollisionEvent(
        IProjectile projectile, 
        Collider collider, 
        int damage)
    {
        this.projectile = projectile;
        this.collider = collider;
        this.damage = damage;
    }
}