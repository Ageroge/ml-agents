using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class SwordsmanScript1 : Agent
{
    // hit points of swordsman. if 0, then he is dead
    public int hitpoints_g = 2;
    // speed of agent. Value is overridden in Unity interface.
    public float speed = 12;

    Rigidbody rBody;

    // Is agent collided with arrow?
    private bool is_collided = false;
    // Size of arena. Need to pass it automatically somehow?
    int arenaSizeX = 16;
    int arenaSizeY = 16;
    // minimal distance between agent and target
    float minDistanceToTarget;
    int hitpoints = 2;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public Transform Target1;
    public Transform Target2;
    // HOW to ADD another archer???

    public override void AgentReset()
    {
        // Move the target to a new spot
        float randomVal1 = Random.value;
        float randomVal2 = Random.value;
        float randomVal3 = Random.value;
        if (randomVal3 <= 0.5f) {
            if (randomVal1 <= 0.5f) randomVal1 = 0.0f;  else randomVal1 = 1.0f;
        }
        else {
            if (randomVal2 <= 0.5f) randomVal2 = 0.0f; else randomVal2 = 1.0f;
        }

        Target1.localPosition = new Vector3(randomVal1 * arenaSizeX - arenaSizeX / 2, 0.5f, randomVal2 * arenaSizeY - arenaSizeY / 2);

        // zero momentum of agent
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        // set opposite position to archer (target)
        this.transform.localPosition = new Vector3(-(randomVal1 * arenaSizeX - arenaSizeX / 2), 0.5f, -(randomVal2 * arenaSizeY - arenaSizeY / 2));
        hitpoints = hitpoints_g;
        is_collided = false;

        // initialize minimal distance of agent to target. Will be used to rewards further
        minDistanceToTarget = Vector3.Distance(this.transform.localPosition, Target1.localPosition);
    }

    public override void CollectObservations()
    {
        // Target and Agent positions
        AddVectorObs(Target1.localPosition);
        AddVectorObs(this.transform.localPosition);

        // Agent velocity
        AddVectorObs(rBody.velocity.x);
        AddVectorObs(rBody.velocity.z);

        // Agent hitpoints
//        AddVectorObs(this.hitpoints);
    }

    //
    // IDEIAS to try
    // - 
    //
    public override void AgentAction(float[] vectorAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];
        rBody.AddForce(controlSignal * speed);

        float distanceToTarget = Vector3.Distance(this.transform.localPosition,
                                                  Target1.localPosition);

        // if agents is closer to the target, give him small award
        // and penatly otherwise
        if (distanceToTarget < minDistanceToTarget) {
            AddReward(0.003f);
            minDistanceToTarget = distanceToTarget;
        }
        else AddReward(-0.001f);

        // Reached target
        if (distanceToTarget < 1.42f) {
            AddReward(1.0f);
            Done();
        }

        // Fell off platform
        if (this.transform.localPosition.y < 0) {
            AddReward(-1.0f);
            Done();
        }
        // Hit by arrow and maybe even killed
        if (is_collided) {
            AddReward(-0.5f);
            hitpoints--;
            is_collided = false;
            if (hitpoints == 0) {
                AddReward(-1.0f);
                Done();
            }
        }
        // Push agent do not stay on one place forever. time is time
        AddReward(-0.0001f);
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
