using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    static private FollowCam S; // another private singleton
    static public GameObject POI; // the staitc point of interest
    public enum eView{none, slingshot, castle, both};

    [Header("Inscribed")]
    public float easing = 0.05f;
    public Vector2 minXY = Vector2.zero; // Vector.zero is [0,0]
    public GameObject viewBothGO;

    

    [Header("Dynamic")]
    public  float camZ; // The desired Z pos of of the camera
    public eView nextView = eView.slingshot;

   
    


    void Awake(){
        S = this;
        camZ = this.transform.position.z;
    }

    void FixedUpdate(){

    Vector3 destination = Vector3.zero;

    if(POI != null){
        // if the POI is rigibody, check to see if it sleeping 
        Rigidbody poiRigid = POI.GetComponent<Rigidbody>();
        if((poiRigid != null) && poiRigid.IsSleeping()){
            POI = null;
        }
    }

    if (POI != null){
        destination = POI.transform.position;
    }
       
        // limit the min values of destination.x & destination.y
        destination.x = Mathf.Max(minXY.x, destination.x);
        destination.y = Mathf.Max(minXY.y, destination.y);
        // INterpolate from the current Camera position toward destination
        destination = Vector3.Lerp(transform.position, destination, easing);
        // Force destination.z to be cam Z to keep the camera far enough way 
        destination.z = camZ;
        // set camera to the destination
        transform.position = destination;
        // Set the orthographicSize if the camera to keep the ground in view
        Camera.main.orthographicSize = destination.y + 10;
    }

    public void SwitchView(eView newView){
        if(newView == eView.none){
            newView = nextView;
        }

    switch(newView){

        case eView.slingshot:
            POI = null;
            nextView = eView.castle;
            break;
        case eView.castle:
            POI = MissionDemolition.GET_CASTLE();
            nextView = eView.both;
            break;
        case eView.both:
            POI = viewBothGO;
            nextView = eView.slingshot;
            break;
            }
        }

public void SwitchView(){
    SwitchView(eView.none);
    }
static public void SWITCH_VIEW(eView newView){
    S.SwitchView(newView);
}

}
