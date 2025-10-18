using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ProjectileType{
    Normal,
    Big,
    Homing
}

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour{
    const int LOOKBACK_COUNT = 10;
    static List<Projectile> PROJECTILES = new List<Projectile>();
    public static int MAX_PROJECTILES = 10;


    [SerializeField]
    private bool _awake = true;
    public bool awake {
        get {return _awake; }
        private set {_awake = value;}
    }

    [Header("Variant Settings")]
    public ProjectileType type = ProjectileType.Normal;
    public float bigScale = 1.5f;
    public float homingTurnRate = 4f;
    public Transform homingTarget;
    public float homingStopDistance = .75f;
    public float homingMaxTime = 3f;
    public float homingMinSpeed = .5f;

    float homingTimer  =0f;
    bool homingActive = true;



   
    private Vector3  prevPos;
    // This priavte list stores the history of Projectile's move distance
    private List<float>  deltas = new List<float>();
    private Rigidbody rigid;


    void Start()
    {

     rigid = GetComponent<Rigidbody>();
     awake = true;
     prevPos = new Vector3(1000,1000,0);
     deltas.Add(1000); 
     PROJECTILES.Add(this);  
     EnforceCap();
     if(type == ProjectileType.Big){
        transform.localScale *= bigScale;
     }
    }

    void FixedUpdate(){
        if (rigid.isKinematic || !awake) return;

        if (type == ProjectileType.Homing && homingTarget != null && homingActive) {
        homingTimer += Time.fixedDeltaTime;

        float dist  = Vector3.Distance(transform.position, homingTarget.position);
        float speed = rigid.velocity.magnitude;

        // stop steering if near, too slow, or time exceeded
        if (dist <= homingStopDistance || speed <= homingMinSpeed || homingTimer >= homingMaxTime) {
            homingActive = false; // let physics take over (gravity + drag)
        } else {
            // steer toward target while preserving speed
            Vector3 toTargetDir = (homingTarget.position - transform.position).normalized;
            Vector3 desiredVel = toTargetDir * speed;
            rigid.velocity = Vector3.RotateTowards(
                rigid.velocity, desiredVel,
                homingTurnRate * Time.fixedDeltaTime,
                0f
            );
        }
    }


        Vector3 deltaV3  = transform.position - prevPos;
        deltas.Add(deltaV3.magnitude);
        prevPos = transform.position;


        // Limit lookback; one of very few times that Ill use while!
        while(deltas.Count > LOOKBACK_COUNT){
            deltas.RemoveAt(0);
        }

        // Iterate over deltas and find the greatest one 
        float maxDelta = 0;
        foreach(float f in deltas){
            if(f > maxDelta) maxDelta = f;
        }

        // if the projectile hasn't moved more than the sleepthreshold 
        if(maxDelta <= Physics.sleepThreshold){
            // set awake to fasle and put the rigidbody to sleep
            awake = false;
            rigid.Sleep();
        }
        if(type == ProjectileType.Homing && homingTarget != null){
            Rigidbody rb  = GetComponent<Rigidbody>();
            Vector3 toTarget  =(homingTarget.position - transform.position).normalized;
            Vector3 newVel = Vector3.RotateTowards(rb.velocity, toTarget * rb.velocity.magnitude, homingTurnRate * Time.fixedDeltaTime, 0f);
            rb.velocity = newVel;
        }
    }

    private void OnDestroy(){
        PROJECTILES.Remove(this);
    }

    static public void DESTROY_PROJECTILES(){
        foreach(Projectile p in PROJECTILES){
            Destroy(p.gameObject);
        }
    }

    static void EnforceCap() {
    while (PROJECTILES.Count > MAX_PROJECTILES) {
        var oldest = PROJECTILES[0];
        if (oldest != null) Destroy(oldest.gameObject);
        PROJECTILES.RemoveAt(0);
    }
}


}
