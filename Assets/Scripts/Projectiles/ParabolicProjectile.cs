using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolicProjectile : MonoBehaviour, IProjectile
{
    const float RAYCAST_EPSILON = 1.01f;
    const float SIMULATION_EPSILON = 0.01f;
    const int MAX_SIMULATION_RECURSION = 3;

    [Range(0f, 1f)]
    [SerializeField] float bounciness = 1f;
    [SerializeField] float initialSpeed = 40f;
    [SerializeField] Vector3 gravity;
    [SerializeField] LayerMask collisionMask = ~0;
    [SerializeField] float maxTimestep = 0.1f;
    [SerializeField] bool extrapolate = false;
    [SerializeField] int damage = 10;

    [SerializeField] bool useSpherecast;
    [SerializeField] float spherecastRadius = 0.05f;

    public OnProjectileCollision OnCollision { get; set; }

    public Vector3 Velocity { get; set; }
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;

    Vector3 currentPhysxPos;
    float simulationTimeDebt = 0f;
    float timeSinceLastFixedUpdate;

    void Awake()
    {
        currentPhysxPos = transform.position;
        Velocity = transform.forward * initialSpeed;
    }

    void Update()
    {
        if (extrapolate)
        {
            timeSinceLastFixedUpdate += Time.deltaTime;
            transform.position = currentPhysxPos + Velocity * timeSinceLastFixedUpdate;
        }
    }

    void FixedUpdate()
    {
        simulationTimeDebt += Time.fixedDeltaTime;

        if (simulationTimeDebt > Time.fixedDeltaTime)
        {
            int iterations = (int)(simulationTimeDebt / Time.fixedDeltaTime);
            iterations /= 2;

            if (iterations == 0)
                iterations = 1;

            for (int ii = 0; ii < iterations; ii++)
                Simulate(Time.fixedDeltaTime);
        }

        transform.position = currentPhysxPos;
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, Velocity);
        timeSinceLastFixedUpdate = 0f;
    }

    public void Simulate(float timestep)
    {
        if (!enabled)
            return;

        Simulate(timestep, 0);
    }

    void Simulate(float timestep, int recursionDepth)
    {
        if (recursionDepth >= MAX_SIMULATION_RECURSION)
            return;

        if (timestep <= SIMULATION_EPSILON)
            return;

        simulationTimeDebt -= timestep;

        if (CollisionCheck(timestep, out RaycastHit hit))
        {
            var avgVel = Velocity + 0.5f * timestep * gravity;
            var hitOffset = currentPhysxPos.To(hit.point);

            float timeToHit = hitOffset.magnitude / avgVel.magnitude;

            Step(timeToHit);
            currentPhysxPos = hit.point;
            Velocity = Velocity.Reflect(hit.normal);

            var collisionEvent = new ProjectileCollisionEvent(this, hit.collider, damage);
            OnCollision?.Invoke(collisionEvent);

            float remainingTime = timestep - timeToHit;

            if (enabled)
                Simulate(remainingTime, recursionDepth + 1);
            else
                simulationTimeDebt += remainingTime;
        }
        else
        {
            Step(timestep);
        }
    }

    bool CollisionCheck(float timestep, out RaycastHit hit)
    {
        if (useSpherecast)
        {
            return Physics.SphereCast(
                    currentPhysxPos,
                    spherecastRadius,
                    Velocity + timestep * 0.5f * gravity,
                    out hit,
                    Velocity.magnitude * timestep * RAYCAST_EPSILON,
                    collisionMask
                );
        }
        else
        {
            return Physics.Raycast(
                    currentPhysxPos,
                    Velocity + timestep * 0.5f * gravity,
                    out hit,
                    Velocity.magnitude * timestep * RAYCAST_EPSILON,
                    collisionMask
                );
        }
    }

    /// <summary>
    /// Moves the projectile by the given timestep, applying gravitational acceleration.
    /// </summary>
    void Step(float timestep)
    {
        Vector3 acc = gravity * timestep;
        Vector3 vel = Velocity + acc * 0.5f;
        currentPhysxPos += vel * timestep;
        Velocity += acc;
    }

    public void Deactivate()
    {
        enabled = false;
    }
}
