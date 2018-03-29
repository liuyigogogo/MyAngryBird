using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [HideInInspector]
    public Vector3 startPosition;

    private const float minCameraX = 0;
    private const float maxCameraX = 13;
    [HideInInspector]
    public bool isFollowing;
    [HideInInspector]
    public Transform birdToFollow;

	// Use this for initialization
	void Start () {
        startPosition = transform.position;	
	}
	
	// Update is called once per frame
	void Update () {
        if (isFollowing)
        {
            if (birdToFollow != null)
            {

                var birdPositon = birdToFollow.transform.position;
                float x = Mathf.Clamp(birdPositon.x, minCameraX, maxCameraX);
                transform.position = new Vector3(x, startPosition.y, startPosition.z);
            }
            else
                isFollowing = false;
        }
	}
}
