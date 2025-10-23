using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour {
    // fields set in the untiy inspector pane
    [Header("Inscribed")]
    public GameObject   projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;

    [Header("Audio")]
    public AudioClip slingSnapClip;
    public float slingSnapVolume = 1f;
    

    // fields set dynamically
    [Header("Dynamic")]
    public GameObject   launchPoint;
    public Vector3      launchPos;
    public GameObject   projectile;
    public bool         aimingMode;

  void Awake(){
    Transform launchPointTrans = transform.Find("LaunchPoint");
    launchPoint = launchPointTrans.gameObject;
    launchPoint.SetActive(false);
    launchPos = launchPointTrans.position;
  }


  void OnMouseEnter(){
    // print( "Slingshot:OnMouseEnter()");
    launchPoint.SetActive(true);
  }
  void OnMouseExit(){
    // print("Slingshot:OnMouseExit");
    launchPoint.SetActive(false);
  }

  void OnMouseDown(){
    // The player has pressed the mouse button while over slingshot
    aimingMode = true;
    // instaniate a projectile
    projectile = Instantiate(projectilePrefab) as GameObject;
    // start it at the launchpoint
    projectile.transform.position = launchPos;
    // set it to isKinematic for now
    projectile.GetComponent<Rigidbody>().isKinematic = true;
  }

    void Update(){
        // if slingshot is not in aimingMode, dont run this code
        if (!aimingMode) return;

        // get the current mouse position in 2d screen coordinates
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        // Find the delta from the LaunchPos to the mousePos3d
        Vector3 mouseDelta = mousePos3D -launchPos;
        // Limit mouseDelta to the radius of the slingshot sphereCollider
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if(mouseDelta.magnitude > maxMagnitude){
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

         // Move the projectile to this new position
         Vector3 projPos = launchPos + mouseDelta;
         projectile.transform.position = projPos;

         if(Input.GetMouseButtonUp(0)) {
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;
            if(slingSnapClip != null){
              AudioSource.PlayClipAtPoint(slingSnapClip, transform.position, slingSnapVolume);
            }
            int shotsAfterThisShot = MissionDemolition.SHOTS_TAKEN + 1;
            // every third shot multiply
            if(shotsAfterThisShot % 3 == 0){
              SpawnExtra(projectile, new Vector3(.5f, .1f, 0f));
              SpawnExtra(projectile, new Vector3(-.5f, .1f, 0f));
            }
            // every 5th shot: big ball
            if(shotsAfterThisShot % 5 == 0){
              var proj = projectile.GetComponent<Projectile>();
              if(proj != null) proj.type = ProjectileType.Big;
            }
            // every 7th shot: Homing ball
            if(shotsAfterThisShot % 7 == 0){
              var proj = projectile.GetComponent<Projectile>();
              if(proj != null){
                proj.type = ProjectileType.Homing;
                var castleGO = MissionDemolition.GET_CASTLE();
                if(castleGO != null){
                  var goal = castleGO.GetComponentInChildren<Goal>();
                  if(goal != null)
                      proj.homingTarget = goal.transform;
                }
              }
            }

            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);
            FollowCam.POI = projectile; // set the _MAINCAMERA POI
            // add a projectieline to the projectile
            Instantiate<GameObject>(projLinePrefab, projectile.transform);
            projectile = null;
            MissionDemolition.SHOT_FIRED();
         }
    }
  void SpawnExtra(GameObject source, Vector3 extraVelOffset){
    GameObject  extra  = Instantiate(source, source.transform.position, Quaternion.identity);
    Rigidbody rbSrc  = source.GetComponent<Rigidbody>();
    Rigidbody rbEx  = extra.GetComponent<Rigidbody>();
    if(rbSrc != null && rbEx != null){
    rbEx.velocity = rbSrc.velocity + extraVelOffset;
    }

  }

}

