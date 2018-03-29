using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlingshotState {
    Idle,
    Pulling,
    Release,
    BirdFlying
}

public class Catapult : MonoBehaviour {
    public GameObject BirdToThrow;

    public Transform BirdWaitPosition;

    public Transform LeftCatapultOrigin;
    public Transform RightCatapultOrigin;
    Vector3 CatapultMiddleVector;
    [HideInInspector]
    public SlingshotState slingshotState;

    public LineRenderer SlingshotLineRenderer1;
    public LineRenderer SlingshotLineRenderer2;

    public LineRenderer TrajectoryLineRenderer;

    public float TimeSinceThrown;

    public float ThrowSpeed;

    // Use this for initialization
    void Start () {

        SlingshotLineRenderer1.sortingLayerName = "Foreground";
        SlingshotLineRenderer2.sortingLayerName = "Foreground";
        TrajectoryLineRenderer.sortingLayerName = "Foreground";

        SlingshotLineRenderer1.SetPosition(0, LeftCatapultOrigin.position);
        SlingshotLineRenderer2.SetPosition(0, RightCatapultOrigin.position);

        slingshotState = SlingshotState.Idle;

        CatapultMiddleVector = new Vector3((LeftCatapultOrigin.position.x + RightCatapultOrigin.position.x) / 2,
            (LeftCatapultOrigin.position.y + RightCatapultOrigin.position.y) / 2,0);
	}
	
	// Update is called once per frame
	void Update () {
        
        switch (slingshotState) {
            //空闲状态
            case SlingshotState.Idle:
                //初始化球
                //print("idle");
                initializeBird();
                //渲染绳子
                DisplaySlingshotLineRenderers();
                //如果有鼠标press则进入pulling状态
                if (Input.GetMouseButtonDown(0))
                {
                    //get the point on screen user has tapped
                    Vector3 location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    //if user has tapped onto the bird
                    if (BirdToThrow.GetComponent<CircleCollider2D>() == Physics2D.OverlapPoint(location))
                    {
                        slingshotState = SlingshotState.Pulling;
                    }
                }
                break;
            //拉动状态
            case SlingshotState.Pulling:
                DisplaySlingshotLineRenderers();
                if (Input.GetMouseButton(0))
                {
                    //get where user is tapping
                    Vector3 location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    location.z = 0;
                    //we will let the user pull the bird up to a maximum distance
                    if (Vector3.Distance(location, CatapultMiddleVector) > 1.5f)
                    {
                        //basic vector maths :)
                        var maxPosition = (location - CatapultMiddleVector).normalized * 1.5f + CatapultMiddleVector;
                        BirdToThrow.transform.position = maxPosition;
                    }
                    else
                    {
                        BirdToThrow.transform.position = location;
                    }
                    float distance = Vector3.Distance(CatapultMiddleVector, BirdToThrow.transform.position);
                    //display projected trajectory based on the distance
                    DisplayTrajectoryLineRenderer2(distance);
                }
                else//user has removed the tap 
                {
                    //SetTrajectoryLineRenderesActive(false);
                    //throw the bird!!!
                    TimeSinceThrown = Time.time;
                    float distance = Vector3.Distance(CatapultMiddleVector, BirdToThrow.transform.position);
                    if (distance > 1)
                    {
                        SetSlingshotLineRenderersActive(false);
                        ThrowBird(distance);
                        slingshotState = SlingshotState.BirdFlying;
                        
                    }
                    else//not pulled long enough, so reinitiate it
                    {
                        //distance/10 was found with trial and error :)
                        //animate the bird to the wait position
                        BirdToThrow.transform.position = BirdWaitPosition.position;

                    }
                }
                    break;
            case SlingshotState.BirdFlying:
                //对球施加力


                break;
        }
	}

    void initializeBird() {
        BirdToThrow.transform.position = BirdWaitPosition.position;
    }

    private void ThrowBird(float distance)
    {
        //get velocity
        Vector3 velocity = CatapultMiddleVector - BirdToThrow.transform.position;
        //BirdToThrow.GetComponent<Rigidbody2D>().AddForce(new Vector2(velocity.x, velocity.y) * ThrowSpeed * distance * 300 * Time.deltaTime);
        //set the velocity
        BirdToThrow.GetComponent<Bird>().OnThrow(); //make the bird aware of it
        BirdToThrow.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x, velocity.y) * ThrowSpeed * distance;
        SetTrajectoryLineRenderesActive(false);

        //notify interested parties that the bird was thrown
        //if (BirdThrown != null)
        //    BirdThrown(this, EventArgs.Empty);
    }

    void SetTrajectoryLineRenderesActive(bool active)
    {
        TrajectoryLineRenderer.enabled = active;
    }

    void DisplayTrajectoryLineRenderer2(float distance)
    {
        TrajectoryLineRenderer.sortingLayerName = "Foreground";
        SetTrajectoryLineRenderesActive(true);
        Vector3 v2 = CatapultMiddleVector - BirdToThrow.transform.position;
        int segmentCount = 15;
        float segmentScale = 2;
        Vector2[] segments = new Vector2[segmentCount];

        // The first line point is wherever the player's cannon, etc is
        segments[0] = BirdToThrow.transform.position;

        // The initial velocity
        Vector2 segVelocity = new Vector2(v2.x, v2.y) * ThrowSpeed * distance;

        float angle = Vector2.Angle(segVelocity, new Vector2(1, 0));
        float time = segmentScale / segVelocity.magnitude;
        for (int i = 1; i < segmentCount; i++)
        {
            //x axis: spaceX = initialSpaceX + velocityX * time
            //y axis: spaceY = initialSpaceY + velocityY * time + 1/2 * accelerationY * time ^ 2
            //both (vector) space = initialSpace + velocity * time + 1/2 * acceleration * time ^ 2
            float time2 = i * Time.fixedDeltaTime * 5;
            segments[i] = segments[0] + segVelocity * time2 + 0.5f * Physics2D.gravity * Mathf.Pow(time2, 2);
        }

        TrajectoryLineRenderer.positionCount = segmentCount;
        //TrajectoryLineRenderer.SetVertexCount(segmentCount);
        for (int i = 0; i < segmentCount; i++)
            TrajectoryLineRenderer.SetPosition(i, segments[i]);
    }

    void DisplaySlingshotLineRenderers()
    {
        //print(BirdToThrow.transform.position);
        SetSlingshotLineRenderersActive(true);
        SlingshotLineRenderer1.SetPosition(1, BirdToThrow.transform.position);
        SlingshotLineRenderer2.SetPosition(1, BirdToThrow.transform.position);
    }

    void SetSlingshotLineRenderersActive(bool active)
    {
        SlingshotLineRenderer1.enabled = active;
        SlingshotLineRenderer2.enabled = active;
    }
}
