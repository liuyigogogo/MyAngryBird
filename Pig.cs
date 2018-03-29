using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour {
    public float health = 150f;
    public Sprite spriteShownWhenHurt;
    private float changeSpriteHealth;
    private Animator animator;
    

    // Use this for initialization
    void Start () {
        changeSpriteHealth = health - 30f;

        animator = GetComponent<Animator>();

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>() == null)
            return;
        if (collision.gameObject.tag == "Bird")
        {
            
            animator.SetBool("dead", true);
            GetComponent<Rigidbody2D>().isKinematic = false;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            Destroy(gameObject,0.3f);
        }
        else
        {
            float damage = collision.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude * 10;
            health -= damage;

            if (health < changeSpriteHealth)
            {
                //GetComponent<SpriteRenderer>().sprite = spriteShownWhenHurt;
                animator.SetBool("wounded", true);
            }
            if (health <= 0)
            {
                print("bird dead");
                animator.SetBool("dead", true);
                GetComponent<Rigidbody2D>().isKinematic = false;
                GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                Destroy(gameObject, 0.3f);
            }
                
        }
    }

    // Update is called once per frame
    void Update () {
        
    }
}
