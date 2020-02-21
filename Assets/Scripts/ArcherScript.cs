using UnityEngine;
using MLAgents;

public class ArcherScript : Agent
{
    [SerializeField] ArrowScript ArrowPrefab = null;

    [SerializeField] float ArrowSpeed = 20;

    // Speed of rotating archer
    [SerializeField] float AngleSpeed = 2.0f;

    // Timer to make delays between shots
    [SerializeField] float ShootDelay = 2.5f;

    // Lifetime of arrow
    [SerializeField] float ArrowLifetime = 2.0f;

    [SerializeField] GameObject TargetSwordsman = null;


    // Size of arena. Need to pass it automatically somehow?
    [SerializeField] Vector2 ArenaSize = new Vector2(16, 16);
    
    float CurrentShootDelay;

    // Archer rotation parameters
    Vector3 DesiredDirection;
    Vector3 CurrentDirection;


    void Start()
    {
        // hunter = GameObject.Find("Hunter_agent");
        CurrentShootDelay = ShootDelay;
    }

    void Update()
    {
        // Rotate archer
        RotateTowardsTarget();

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, CurrentDirection.normalized * 10, Color.red);
        Debug.DrawRay(transform.position, DesiredDirection.normalized * 10, Color.green);

        // Shoot an arrow if needed
        CurrentShootDelay -= Time.deltaTime;
        // if (Input.GetButtonDown("Fire1"))
        if (CurrentShootDelay <= 0.0f)
        {
            CurrentShootDelay = ShootDelay;
            Shoot();
        }
    }

    public override void AgentReset()
    {
        // Move the agent to a new spot
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

        transform.localPosition = new Vector3( ArenaSize.x * (xPosition - 0.5f), 0.5f, ArenaSize.y * (yPosition - 0.5f));
        // Debug.Log("Archer reset");
    }

    public override void CollectObservations()
    {
        // Target and Agent positions
        AddVectorObs(TargetSwordsman.transform.localPosition);
        // Should we really recall previous position of swordsman?
        // AddVectorObs(swordsman.prevLocalPosition);
        AddVectorObs(this.transform.localPosition);

        // Agent rotation
        AddVectorObs(this.transform.rotation);
    }

    // vectorAction[] is passed by brains of the agent. Brains here is Behavior Parameters script.
    // vectorAction[] reflects number of Vector Action Space Size from Behavior Parameters.
    // https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Learning-Environment-Design-Agents.md
    public override void AgentAction(float[] vectorAction)
    {
        // Actions of archer
        // - turn to the target. Turn exactly to target, but use "delta" from vectorAction[]
        // to find where to shoot.
        // - shoot

        Vector3 delta = new Vector3(vectorAction[0], 0f, vectorAction[1]);
        // Debug.Log(delta);

        // Determine which direction to rotate towards
        DesiredDirection = TargetSwordsman.transform.localPosition - transform.localPosition + delta;
        DesiredDirection.y = 0f;
    }

    void RotateTowardsTarget()
    {
        // The step size is equal to speed times frame time.
        float singleStep = AngleSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        CurrentDirection = Vector3.RotateTowards(transform.forward, DesiredDirection, singleStep, 0.0f);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(CurrentDirection);
    }

    void Shoot()
    {
        var arrow = Instantiate(ArrowPrefab, transform.position, Quaternion.identity);
        arrow.OnArrowHit += OnArrowHit;

        Rigidbody instArrowRigidbody = arrow.GetComponent<Rigidbody>();
        instArrowRigidbody.velocity = transform.TransformDirection(Vector3.forward * ArrowSpeed);

        // Destroy after ArrowLifetime seconds
        Destroy(arrow.gameObject, ArrowLifetime);
    }

    void OnArrowHit(ArrowScript arrow)
    {
        // Target is dead (Xorboo: just a hit, swordsman might not be actually dead yet)

        // Debug.Log("Arrow collided with swordsman");
        AddReward(0.5f);
        Done();
    }

    public override float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }

    //    void LookAtSwordsman(Transform target)
    //    {
    // Determine which direction to rotate towards
    //        Vector3 targetDirection = target.position - transform.position;

    // The step size is equal to speed times frame time.
    //        float singleStep = angle_speed * Time.deltaTime;

    // Rotate the forward vector towards the target direction by one step
    //        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

    // Draw a ray pointing at our target in
    //        Debug.DrawRay(transform.position, newDirection, Color.red);

    // Calculate a rotation a step closer to the target and applies rotation to this object
    //        transform.rotation = Quaternion.LookRotation(newDirection);
    //    }


    /*    void Shoot()
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, hunter.transform.position, out hit, range))
            {
                Debug.Log(hit.transform.name);
            }
            else
            {
                Debug.Log("no hit");
            }
        }
    */

    public void OnHitByEnemy()
    {
        // Debug.Log("Hit by swordsman");
        AddReward(-0.5f);
        Done();
    }
}
