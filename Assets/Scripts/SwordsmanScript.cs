using UnityEngine;
using MLAgents;

public class SwordsmanScript : Agent
{
    // Hit points of swordsman. if 0, then he is dead
    [SerializeField] int MaxHp = 2;
    // Speed of agent. Value is overridden in Unity interface.
    [SerializeField] float Speed = 12;

    [SerializeField] ArcherScript Target = null;

    // Size of arena. Need to pass it automatically somehow?
    [SerializeField] Vector2 ArenaSize = new Vector2(16, 16);


    Rigidbody Rigidbody;

    // Minimal reached distance between agent and target
    float MinDistanceToTarget;
    int CurrentHp = 1;

    Vector3 CurrentPushDirection;


    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Push agent do not stay on one place forever. time is time
        AddReward(-0.0001f);

        CheckTarget();

        Debug.DrawRay(transform.position, CurrentPushDirection.normalized * 5, Color.magenta);
    }

    public override void AgentReset()
    {
        // Move the target to a new spot
        float xPosition = Random.value;
        float yPosition = Random.value;
        if (Random.value <= 0.5f)
        {
            xPosition = xPosition <= 0.5f ? 0.0f : 1.0f;
        }
        else
        {
            yPosition = yPosition <= 0.5f ? 0.0f : 1.0f;
        }

        // Set opposite position to archer (target) (Xorboo: random values are not related to an archer in any way)
        transform.localPosition = new Vector3(-ArenaSize.x * (xPosition - 0.5f), 0.5f, -ArenaSize.y * (yPosition - 0.5f));

        // Restore HP
        CurrentHp = MaxHp;

        // Zero momentum of agent
        Rigidbody.angularVelocity = Vector3.zero;
        Rigidbody.velocity = Vector3.zero;

        // Initialize minimal distance of agent to target. Will be used to rewards further
        MinDistanceToTarget = Vector3.Distance(transform.localPosition, Target.transform.localPosition);
    }

    public override void CollectObservations()
    {
        // Target and Agent positions
        AddVectorObs(Target.transform.localPosition);
        AddVectorObs(transform.localPosition);

        // Agent velocity
        AddVectorObs(Rigidbody.velocity.x);
        AddVectorObs(Rigidbody.velocity.z);

        // Agent hitpoints
        // AddVectorObs(this.CurrentHp);
    }

    //
    // IDEIAS to try
    // - 
    //
    public override void AgentAction(float[] vectorAction)
    {
        // Actions, size = 2
        CurrentPushDirection = new Vector3(vectorAction[0], 0.0f, vectorAction[1]);
        Rigidbody.AddForce(CurrentPushDirection * Speed);
    }

    public override float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }
    
    public void OnHitByArrow()
    {
        AddReward(-0.5f);
        CurrentHp--;

        if (CurrentHp == 0)
        {
            AddReward(-1.0f);
            Done();
        }
    }

    void CheckTarget()
    {
        float distanceToTarget = Vector3.Distance(transform.localPosition, Target.transform.localPosition);

        // if agents is closer to the target, give him small award
        // and penatly otherwise
        if (distanceToTarget < MinDistanceToTarget)
        {
            AddReward(0.003f);
            MinDistanceToTarget = distanceToTarget;
        }
        else
        {
            AddReward(-0.001f);
        }

        // Reached target
        if (distanceToTarget < 1.0f)
        {
            AddReward(1.0f);
            Target.OnHitByEnemy();
            Done();
        }

        // Fell off platform
        if (transform.localPosition.y < 0)
        {
            AddReward(-1.0f);
            Done();
        }
    }
}
