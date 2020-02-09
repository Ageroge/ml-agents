using UnityEngine;

public class ArcherScript : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    // public Rigidbody projectile;
    public GameObject arrow;
    public float arrow_speed = 20;
    // speed of rotating archer
    public float angle_speed = 2.0f;
    // timer to make delays between shots
    public float targetTime = 2.5f;

    public GameObject swordsman;
//    private GameObject hunter;

    void Start()
    {
//        hunter = GameObject.Find("Hunter_agent");
    }

    // Update is called once per frame
    void Update()
    {
        LookAtSwordsman(swordsman.transform);

        targetTime -= Time.deltaTime;
//        if (Input.GetButtonDown("Fire1"))
        if (targetTime <= 0.0f)
        {
            targetTime = 2.5f;
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject instArrow = Instantiate(arrow, transform.position, Quaternion.identity) as GameObject;
        Rigidbody instArrowRigidbody = instArrow.GetComponent<Rigidbody>();

//        Debug.Log("this rot=" + transform.rotation);

        instArrowRigidbody.velocity = transform.TransformDirection(Vector3.forward * arrow_speed);
        
        Destroy(instArrow, 2.0f); // 1 - time in seconds. Live range of arrow
    }

    void LookAtSwordsman(Transform target)
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = target.position - transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = angle_speed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);
    }


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
