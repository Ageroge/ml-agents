using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class SwordsmanScript1 : Agent
{
    Rigidbody rBody;
    private bool is_collided = false;
    // Size of arena. Need to pass it automatically somehow?
    int arenaSizeX = 16;
    int arenaSizeY = 16;
    float prevDistanceToTarget;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform Target;
    public override void AgentReset()
    {
        // Move the target to a new spot
        float randomVal1 = Random.value;
        float randomVal2 = Random.value;
        float randomVal3 = Random.value;
        if (randomVal3 <= 0.5f)
        {
            if (randomVal1 <= 0.5f) randomVal1 = 0.0f; else randomVal1 = 1.0f;
        }
        else
        {
            if (randomVal2 <= 0.5f) randomVal2 = 0.0f; else randomVal2 = 1.0f;
        }

        Target.localPosition = new Vector3(randomVal1 * arenaSizeX - arenaSizeX / 2, 0.5f, randomVal2 * arenaSizeY - arenaSizeY / 2);

        if (this.transform.localPosition.y < 0 || is_collided)
        {
            // If the Agent fell, zero its momentum
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            // make opposite position to archer
            this.transform.localPosition = new Vector3(-(randomVal1 * arenaSizeX - arenaSizeX / 2), 0.5f, -(randomVal2 * arenaSizeY - arenaSizeY / 2));
            is_collided = false;
        }

        prevDistanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);
//        Debug.Log(initDistanceToTarget);
    }

    public override void CollectObservations()
    {
        // Target and Agent positions
        AddVectorObs(Target.localPosition);
        AddVectorObs(this.transform.localPosition);

        // Agent velocity
        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.z);
    }

    public float speed = 12;
    public override void AgentAction(float[] vectorAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition,
                                                  Target.localPosition);

        if (prevDistanceToTarget > distanceToTarget) AddReward(0.01f);
        else AddReward(-0.01f);

        prevDistanceToTarget = distanceToTarget;

        // Reached target
        if (distanceToTarget < 1.42f)
        {
            AddReward(1.0f);
            Done();
        }

        // Fell off platform
        if (this.transform.localPosition.y < 0)
        {
            AddReward(-1.0f);
            Done();
        }
        // Killed by arrow
        if (is_collided)
        {
            AddReward(-0.5f);
            Done();
        }

    }

    public override float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }

    void OnCollisionEnter(Collision collided)
    {
        if (collided.gameObject.tag == "arrow")
        {
            is_collided = true;
        }

    }
}
