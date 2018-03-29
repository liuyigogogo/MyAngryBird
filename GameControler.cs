using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Start,
    InitBird,
    Playing,
    Lose,
    Win
}


public class GameControler : MonoBehaviour {
    public CameraFollow cameraFollow;
    int currentBirdIndex;
    public Catapult catapult;
    [HideInInspector]
    public static GameState currentGameState = GameState.Start;
    private List<GameObject> bricks;
    private List<GameObject> birds;
    private List<GameObject> pigs;

	// Use this for initialization
	void Start () {
        currentGameState = GameState.Start;
        catapult.enabled = false;
        
        bricks = new List<GameObject>(GameObject.FindGameObjectsWithTag("Brick"));
        birds = new List<GameObject>(GameObject.FindGameObjectsWithTag("Bird"));
        pigs = new List<GameObject>(GameObject.FindGameObjectsWithTag("FakePig"));
        currentBirdIndex = birds.Count - 1;


    }
    private void OnGUI()
    {
        GUIStyle gUIStyle = new GUIStyle();
        gUIStyle.normal.textColor = Color.red;
        switch (currentGameState)
        {
            case GameState.Start:
                GUI.Label(new Rect(150, 150, 200, 100), "点击屏幕开始游戏", gUIStyle);
                break;
            case GameState.Win:
                GUI.Label(new Rect(150, 150, 200, 100), "胜利！点击屏幕后继续", gUIStyle);
                break;
            case GameState.Lose:
                GUI.Label(new Rect(150, 150, 200, 100), "失败~点击屏幕接着来", gUIStyle);
                break;
            default:
                break;
        }
    }
    // Update is called once per frame
    void Update () {
		switch(currentGameState)
        {
            case GameState.Start:
                if(Input.GetMouseButton(0))
                {
                    print("start");
                    
                    currentGameState = GameState.InitBird;
                }
            break;
            case GameState.InitBird:
                if(moveBirdToSlingShot(birds[currentBirdIndex]))
                {
                    currentGameState = GameState.Playing;
                    catapult.BirdToThrow = birds[currentBirdIndex];
                    catapult.enabled = true;
                    cameraFollow.isFollowing = true;
                    cameraFollow.birdToFollow = birds[currentBirdIndex].transform;
                }
                break;
            case GameState.Playing:
                if(catapult.slingshotState == SlingshotState.BirdFlying &&(isAllObjectStopped() || Time.time - catapult.TimeSinceThrown >= 5f))
                {
                    print("playing");
                    catapult.enabled = false;
                    //如果猪死光了，就赢了
                    if (isPigAllClear())
                    {
                        print("you win");
                        currentGameState = GameState.Win;
                    }else if(currentBirdIndex == 0)
                    {
                        print("you lose");
                        currentGameState = GameState.Lose;
                    }else
                    {
                        currentBirdIndex = currentBirdIndex - 1;
                        catapult.slingshotState = SlingshotState.Idle;
                        currentGameState = GameState.InitBird;
                    }
 
                }

            break;
            case GameState.Lose:
            case GameState.Win:
                if(Input.GetMouseButton(0))
                {
                    SceneManager.LoadScene("FirstTry");
                }
                break;
        }
	}

    bool moveBirdToSlingShot(GameObject gameObject)
    {
        float speed = 10f;
        if (gameObject.transform.position.Equals(catapult.BirdWaitPosition.position))
            return true;
        else
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, catapult.BirdWaitPosition.position, speed * Time.deltaTime);
            return false;
        }
    }

    bool isPigAllClear()
    {
        foreach(GameObject pig in pigs)
        {
            if (pig != null)
                return false;
        }

        return true;
    }

    bool isAllObjectStopped()
    {
        foreach (GameObject pig in pigs)
        {
            if (pig == null)
                continue;
            if(pig.GetComponent<Rigidbody2D>().velocity.magnitude > 1f )
            {
                return false;
            }
        }

        foreach (GameObject pig in bricks)
        {
            if (pig == null )
                continue;
            
            if (pig.GetComponent<Rigidbody2D>().velocity.magnitude > 1f)
            {
                return false;
            }
        }
        foreach (GameObject pig in birds)
        {
            if (pig == null)
                continue;
            if (pig.GetComponent<Rigidbody2D>().velocity.magnitude > 1f)
            {
                return false;
            }
        }

        return true;
    }
}
