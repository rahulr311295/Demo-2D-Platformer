using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll; // To fix multi hop


    [SerializeField] private LayerMask Ground;
    [SerializeField] private int pizza = 0; //Pizza 
    [SerializeField] private Text pizzaCount; //Pizza Collected Count On UI
    [SerializeField] private float damageForce = 15f;
    [SerializeField] private float jumpforce = 15f;
    [SerializeField] private float speed = 5f;

    private enum State {idle,running,jumping,falling,hurt} //Player States to trigger animations 
    private State state = State.idle; //Default state of player
    private void Start() //Import Classes on game start
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }
    // Update is called once per frame
    private void Update()
    {
        if (state != State.hurt)
        {
            PlayerControl(); //Calls player controller Class
        }
        AnimState();
        anim.SetInteger("state", (int)state);

    }
    private void OnTriggerEnter2D(Collider2D collision) // Trigger Collition with Pizza Object
    {
        if(collision.tag == "Pizza")
        {
            Destroy(collision.gameObject); // Destroy Pizza
            pizza += 1; // Incremet Pizza Score
            pizzaCount.text = pizza.ToString(); // Show it in UI
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemies")
        {
            if (state == State.falling)
            {
                Destroy(other.gameObject);
                JumpFunction();
            }
           /*else
            {
                state = State.hurt;
                if(other.gameObject.transform.position.x > transform.position.x)
                {
                    //Enemy to Right Damage move to left
                    //rb.velocity = new Vector2(-damageForce, rb.velocity.y);
                    DeathAnimation();
                }
                else
                {
                    //Enemy to Left move to right
                    //rb.velocity = new Vector2(damageForce, rb.velocity.y);
                    DeathAnimation();
                }
            } */
        } 
        else if(other.gameObject.tag == "Enemies")
        {
            if (state == State.idle)
            {
                if(other.gameObject.transform.position.x > transform.position.x)
                {
                    state = State.hurt;
                    DeathAnimation();
                }
                    
            }
            else if(state == State.running)
            {
                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    state = State.hurt;
                    DeathAnimation();
                }
            }
        }
    }
    private void PlayerControl()
    {
        float Hdirection = Input.GetAxis("Horizontal"); //Using Unity's Input Manager   Horizontal = Left Right Movement   // Vertical = Jump
        if (Hdirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
           // anim.SetBool("running", true);

        }
        else if (Hdirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
           // anim.SetBool("running", true);

        }
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(Ground))
        {
            JumpFunction();

        }
        

    }
    private void JumpFunction()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        //   anim.SetTrigger("jumping");
        state = State.jumping;
    }
    private void DeathAnimation()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        state = State.hurt;
    }

    private void AnimState()
    {
        if (state == State.jumping)
        {
            if(rb.velocity.y < .1f)
            {
                state = State.falling;
            }
        }
        else if(state == State.falling)
        {
            if(coll.IsTouchingLayers(Ground))
            {
                state = State.idle;
            }
        }
        else if (state == State.hurt)
        {
            if(Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle;
            }
        }
        else if(Mathf.Abs(rb.velocity.x) > 2f)
        {
            state = State.running;
        }
        else
        {
            state = State.idle;
        }

    } 
}
