using UnityEngine;
using MLAgents;

public class ArcherScript : Agent
{
    // public Rigidbody projectile;
    public GameObject arrow;
    public float arrow_speed = 20;
    // speed of rotating archer
    public float angle_speed = 2.0f;
    // timer to make delays between shots
    public float targetTime_g = 2.5f;

    public GameObject swordsman;

    float targetTime;
    // Size of arena. Need to pass it automatically somehow?
    int arenaSizeX = 16;
    int arenaSizeY = 16;

    // Is archer collided by swordsman?
    private bool is_collided = false;

    GameObject instArrow;

    void Start()
    {
        //        hunter = GameObject.Find("Hunter_agent");
        targetTime = targetTime_g;
        instArrow = arrow;
    }

    public override void AgentReset()
    {
        // Move the agent to a new spot
        float randomVal1 = Random.value;
        float randomVal2 = Random.value;
        this.transform.localPosition = new Vector3(randomVal1 * arenaSizeX - arenaSizeX / 2, 0.5f, randomVal2 * arenaSizeY - arenaSizeY / 2);
        is_collided = false;

    }

    public override void CollectObservations()
    {
        // Target and Agent positions
        AddVectorObs(swordsman.transform.localPosition);
        // should we really recall previous position of swordsman?
//        AddVectorObs(swordsman.prevLocalPosition);
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

        Vector3 delta = new Vector3(vectorAction[0], 0.5f, vectorAction[1]);

        //Debug.Log(delta);

        // Determine which direction to rotate towards
        Vector3 targetDirection = swordsman.transform.localPosition - this.transform.localPosition + delta;

        // The step size is equal to speed times frame time.
        float singleStep = angle_speed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(this.transform.localPosition, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);

        targetTime -= Time.deltaTime;
        //        if (Input.GetButtonDown("Fire1"))
        if (targetTime <= 0.0f)
        {
            targetTime = targetTime_g;
            Shoot();
        }

        ///// HOWTO ch

        // Target is dead
//        Debug.Log(instArrow.GetComponent<ArrowScript>().is_collided);
        if (instArrow != null && instArrow.GetComponent<ArrowScript>().is_collided)
        {
            instArrow.GetComponent<ArrowScript>().is_collided = false;
//            Debug.Log("Arrow collided with swordsman");
            AddReward(0.5f);
            Done();
        }

        // Hit by swordsman
        if (is_collided)
        {
            AddReward(-0.5f);
            Done();
        }
    }

    void Shoot()
    {
        instArrow = Instantiate(arrow, transform.position, Quaternion.identity) as GameObject;
        Rigidbody instArrowRigidbody = instArrow.GetComponent<Rigidbody>();

        instArrowRigidbody.velocity = transform.TransformDirection(Vector3.forward * arrow_speed);

        Destroy(instArrow, 2.0f); // 1 - time in seconds. time live range of arrow
    }

    void OnCollisionEnter(Collision collided)
    {
        if (collided.gameObject.tag == "swordsman")
        {
            is_collided = true;
        }

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
}
