using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class PlayerController : MonoBehaviour
{
    
    private Rigidbody2D rb;
    private Animator anim;
    private enum State { IDLE, RUN, JUMP, FALL, HURT };
    private State state = State.IDLE;
    private Collider2D coll;
    private AudioSource footsteps;

    [Header("Private Player Movement")]
    [SerializeField] private LayerMask Ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float hurtForce = 20f; //rebound/kickback from bumping enemy

    [Header("Private Collectables")]
    [SerializeField] private int cherries = 0;
    [SerializeField] private Text cherryText;

    [Header("Private Player Stats")]
    [SerializeField] private int health;
    [SerializeField] private Text healthAmount;



    //[Header("Public Variables")]


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        healthAmount.text = health.ToString();
        footsteps = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(state != State.HURT)
        {
            Movement();
        }
        

        VelocityState();
        anim.SetInteger("State", (int)state);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Collectable")
        {
            Destroy(collision.gameObject);
            cherries += 1;
            cherryText.text = cherries.ToString();
        }

        //powerup collision trigger
        if(collision.tag == "Powerup")
        {
            Debug.Log("POWERUP!!");
            Destroy(collision.gameObject);
            jumpForce = 30f;
            GetComponent<SpriteRenderer>().color = Color.yellow;
            StartCoroutine(ResetPower());
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            if(state == State.FALL)
            {
                Destroy(other.gameObject);
            }
            else
            {
                state = State.HURT;
                HandleHealth();
                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    //enemy is to the right
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                }
                else
                {
                    //enemy is to the left
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }


            }
        }


    }

    private void HandleHealth()
    {
        health -= 1;
        healthAmount.text = health.ToString();
        //if player dies, load current scene
        if(health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void VelocityState()
    {
        if(state == State.JUMP)
        {
            if(rb.velocity.y < 0.1f)
            {
                state = State.FALL;
            }

        }
        else if(state == State.FALL)
        {
            if(coll.IsTouchingLayers(Ground))
            {
                state = State.IDLE;
            }
        }
        
        else if(Mathf.Abs(rb.velocity.x) > 2f)
        {
            state = State.RUN;
        }
        else
        {
            state = State.IDLE;
        }
    }

    private void Movement()
    {
        float hDirection = Input.GetAxis("Horizontal");

        //move right
        if (hDirection > 0)
        {
            //rb.velocity = new Vector2(speed, 0);
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);

        }
        //move left
        else if (hDirection < 0)
        {
            //rb.velocity = new Vector2(-speed, 0);
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);

        }
        else
        {   // if player not moving, set to default idle

        }
        //Jump
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(Ground))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            state = State.JUMP;
        }
    }

    private IEnumerator ResetPower()
    {
        yield return new WaitForSeconds(5);
        jumpForce = 15;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void FootSounds()
    {
        footsteps.Play();
    }
}
