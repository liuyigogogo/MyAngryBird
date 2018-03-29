using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
enum BirdState {
    BeforeThrown,
    Thrown
}

namespace Assets.Scripts
{
    public static class Constants
    {
        public static readonly float MinVelocity = 2.5f;
        public static readonly float BirdColliderRadiusNormal = 0.234f;
        public static readonly float BirdColliderRadiusBig = 0.5f;
    }
}

public class Bird : MonoBehaviour {

    BirdState state;
    
   
    // Use this for initialization
    void Start () {
        state = BirdState.BeforeThrown;
        GetComponent<TrailRenderer>().enabled = false;
       

    }

    // Update is called once per frame
    void Update () {
        
    }

    private void FixedUpdate()
    {
        //如果在飞行状态且速度小于最小速度，就删除
        if (state == BirdState.Thrown && GetComponent<Rigidbody2D>().velocity.sqrMagnitude <= Constants.MinVelocity)
        {
            StartCoroutine(DestroyAfter(2));
        }
    }
    IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    public void OnThrow() {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<TrailRenderer>().enabled = true;
        state = BirdState.Thrown;
    }
}
