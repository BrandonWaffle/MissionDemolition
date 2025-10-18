using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public enum GameMode{
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour
{
  static private MissionDemolition S; // a private singleton
  public static int SHOTS_TAKEN => (S != null) ? S.shotsTaken : 0;


  [Header("Inscribed")]

  public Text           uitLevel;
  public Text           uitShots;
  public Vector3        castlePos;
  public GameObject[]   castles;
  public GameObject gameOverPanel;
  public int maxShotsPerLevel =  10;


[Header("Dynamic")]

public int              level;
public int              levelMax;
public int              shotsTaken;
public GameObject       castle;
public GameMode         mode = GameMode.idle;
public string           showing = "Show SlingShot";


void Start(){
    S = this;
     

     level = 0;
     shotsTaken = 0;
     levelMax = castles.Length;
     Time.timeScale = 1f;
     if (gameOverPanel) gameOverPanel.SetActive(false);
     StartLevel();

}

void StartLevel(){
    if(castle != null){
        Destroy(castle);
    }
        Projectile.DESTROY_PROJECTILES();

        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;

        Goal.goalMet = false;

        shotsTaken  = 0;
        UpdateGUI();

        mode = GameMode.playing;

        // Zoom out to show both 
        FollowCam.SWITCH_VIEW(FollowCam.eView.both);
}

void UpdateGUI(){
    uitLevel.text = "Level: "+(level+1)+"of "+levelMax;
    uitShots.text = "Shots Taken: "+shotsTaken;
}

void Update(){
    UpdateGUI();

    if ((mode == GameMode.playing) && Goal.goalMet){
        mode = GameMode.levelEnd;
        FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);
        Invoke("NextLevel", 2f);
    }
    if(shotsTaken >= maxShotsPerLevel){
            GameOver();
        }
}

void GameOver(){
    mode = GameMode.levelEnd;
    Time.timeScale = 0f;
    if (gameOverPanel) gameOverPanel.SetActive(true);
}
public void RestartGame(){
    Time.timeScale = 1f;
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);

}
void NextLevel(){
    level++;
    if(level == levelMax){
        level = 0;
        shotsTaken = 0;
    }
    shotsTaken = 0;
    StartLevel();
}

static public void SHOT_FIRED(){
    S.shotsTaken++;
}

static public GameObject GET_CASTLE(){
    return S.castle;
}

}
