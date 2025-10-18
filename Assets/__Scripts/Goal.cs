using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour{
    // A static field accessible by code anywhere
    static public bool goalMet = false;

    void OnTriggerEnter(Collider other){
        // when the trigger is hit by something 
        // check to see if its a projectile
        Projectile proj = other.GetComponent<Projectile>();
        if (proj != null){
            // if so, set goalmet to true
            Goal.goalMet = true;
            // also se the alpha of the color to higher opacity
            Material mat = GetComponent<Renderer>().material;
            Color c  = mat.color;
            c.a = .75f;
            mat.color = c;
        }
    }



   








}
