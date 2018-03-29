using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour {
    public float health = 100f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>() == null)
            return;
        float damage = collision.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude * 10;
        health -= damage;
        if (health <= 0)
            Destroy(this.gameObject);
    }

}
