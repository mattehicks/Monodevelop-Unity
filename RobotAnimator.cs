using UnityEngine;
using System.Collections;

public class RobotAnimator : MonoBehaviour {

    //private AnimationState overdrive_walk;
    //private AnimationState overdrive_idle;
    private bool attack = true;
    private float dist_range;
    public float range; //input
    public GameObject victim;
    public string name; //input
    private bool pDebug = false;

    // Use this for initialization
    void Start()
    {
        //overdrive_walk = animation["overdrive_walk"];
        //overdrive_idle = animation["mech_idle"];

    }

    // Update is called once per frame
    void Update()
    {
        dist_range = Vector3.Distance(victim.transform.position, transform.position);
        if (pDebug)
        { Debug.Log(name + " distance: " + victim.transform.position); }

        if (dist_range > range) { attack = true; }
        if (attack == true)
        {
            // rigidbody.AddForce(transform.forward * 200, ForceMode.Force);
            transform.Translate(Vector3.forward * 10 * Time.deltaTime);
            animation.Play("robotwalk");
            if (dist_range < range) { attack = false; }
        }

        if (attack == false)
        {
            if (pDebug) { Debug.Log("IDLE NOW"); }
            animation.Play("robotidle");
        }

    }
}
