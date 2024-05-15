using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// ENCAPUSLATION via SerializeField ///
public class PinkPlayerController : MonoBehaviour
{
    /// ABSTRACTION VIA FINITE STATE MACHINE ///
    private enum State { IDLE, RUN, JUMP, FALL, HURT };
    private State state = State.IDLE;

    [Header("Movement")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpSpeed = 18f;
    [SerializeField] private float hurtForce = 0.02f; //rebound/kickback from bumping enemy
    private float direction = 0f;

    [Header("Ground Checks")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;
    private bool isTouchingGround;

    private Rigidbody2D player;
    private Animator playerAnimation;
    private CapsuleCollider2D playerHitBox; // for better re-use on different players, use Collider2D instead
                                            // CapsuleCollider2D is a child of Collider2D, as is BoxCollider2D, etc.

    [Header("Audio Mess")]
    [SerializeField] private AudioSource footsteps;
    [SerializeField] private AudioSource crystalSound;
    [SerializeField] private AudioSource powerupSound;
    [SerializeField] private AudioSource bumpSound;
    [SerializeField] private AudioSource mainMusic;

    [Header("Player Transitions")]
    [SerializeField] private Vector3 respawnPoint;
    [SerializeField] private GameObject blackOutScreen;

    [Header("Public Variables")]
    public GameObject fallDetector;
    public Text scoreText;
    public HealthBar healthBar; // access the public health script
    
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<Animator>();
        playerHitBox = GetComponent<CapsuleCollider2D>(); //could just use generic Collider2D
                                                          //playerHitBox unused atm; using alternate ground check method
        respawnPoint = transform.position; // stores players initial position to respawn to
        scoreText.text = "SCORE: " + ScoreController.totalScore;
    }

    // Update is called once per frame
    void Update()
    {
        isTouchingGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (state != State.HURT)
        {
            MovementHandler(); /// ABSTRACTION PRINCIPLE
        }
        // --- ///
        AnimationStateHandler(); /// ABSTRACTION PRINCIPLE
        playerAnimation.SetInteger("State", (int)state);//updates animation based on Enumerator state

        // fall detection - follow the player on the x axis
        fallDetector.transform.position = new Vector2(transform.position.x, fallDetector.transform.transform.position.y);
    }

    private void MovementHandler()
    {
        
        direction = Input.GetAxis("Horizontal");

        // for left/right movement
        if (direction > 0f)
        {
            player.velocity = new Vector2(direction * speed, player.velocity.y);
            transform.localScale = new Vector2(1, 1);
        }
        else if (direction < 0f)
        {
            // if player is on a tilted platform, player can slide when keys not pressed
            player.velocity = new Vector2(direction * speed, player.velocity.y);
            transform.localScale = new Vector2(-1, 1);
        }
        else
        {
            player.velocity = new Vector2(0, player.velocity.y);
        }

        //for jump movement
        if (Input.GetButtonDown("Jump") && isTouchingGround)
        // could use (Input.GetButtonDown("Jump") && playerHitBox.isTouchingGround(groundLayer))
        {
            JumpHandler();
        }

        // to change speed parameter in animator
        playerAnimation.SetFloat("Speed", Mathf.Abs(player.velocity.x));
        playerAnimation.SetBool("OnGround", isTouchingGround);          

    }

    private void JumpHandler()
    {
        player.velocity = new Vector2(player.velocity.x, jumpSpeed);
        state = State.JUMP;
    }

    private void AnimationStateHandler()
        // TODO: State.FALL doesn't detect the player falling off of a floating platform, only "falls" after jump. Add from platforms
    {
        if (state == State.JUMP)
        {
            if (player.velocity.y < 0.1f)
            {
                state = State.FALL;
            }
        }
        else if (state == State.FALL)
        {
            if (isTouchingGround)
            {
                state = State.IDLE;
            }
        }
        else if (state == State.HURT)
        {
            // if we're hurt and no longer moving, reset to IDLE so that we can move again
            if (Mathf.Abs(player.velocity.x) < 0.1f)
            {
                state = State.IDLE;
            }
            //else if (!isTouchingGround)
            //{
            //    state = State.FALL;
            //}
        }
        //else if (Mathf.Abs(player.velocity.x) > 2f) // 2f gives player a small slide in movements
        else if (Mathf.Abs(player.velocity.x) > Mathf.Epsilon) // epsilon prevents the slide

        {
            state = State.RUN;
        }
        else
        {
            state = State.IDLE;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "FallDetector")
        {
            transform.position = respawnPoint;
            state = State.IDLE;
        }
        if (collision.tag == "Checkpoint")
        {
            respawnPoint = transform.position; // set to the new player position at checkpoint
        }
        if (collision.tag == "NextLevel")
        {
            StartCoroutine(LevelTransition());
            Invoke("NextLevel", 2.0f);
            //stop player from moving
            player.constraints = RigidbodyConstraints2D.FreezePosition;
        }

        if (collision.tag == "Crystal")
        {
            ////// ABSTRACTION OF SCORE: ScoreController integer carried between scenes ////
            ScoreController.totalScore += 1;
            crystalSound.Play();
            scoreText.text = "SCORE: " + ScoreController.totalScore; 
            collision.gameObject.SetActive(false); // disable object
        }
        if (collision.tag == "Powerup")
        {
            speed = speed + 10f;
            powerupSound.Play();
            GetComponent<SpriteRenderer>().color = Color.green;
            StartCoroutine(PowerReset());
            collision.gameObject.SetActive(false); // disable object
        }
        if (collision.tag == "TRAPS/Saw")
        {          
            StartCoroutine(SawYourSkull());         
        }
    }

    IEnumerator LevelTransition(float fadeSpeed = 0.7f, int secondsToFadeOut = 3)
    {
        Color objectColor = blackOutScreen.GetComponent<Image>().color;
        float fadeAmount;

        mainMusic = Camera.main.GetComponent<AudioSource>();
        while (mainMusic.volume > 0.01f)
        {
            mainMusic.volume -= Time.deltaTime / secondsToFadeOut;
            yield return null;
        }

        mainMusic.volume = 0;
        mainMusic.Stop();

        while (blackOutScreen.GetComponent<Image>().color.a < 1)
        {
            fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            blackOutScreen.GetComponent<Image>().color = objectColor;
            yield return null;
        }
    }

    void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        respawnPoint = transform.position; // reset spawn position
    }

    private IEnumerator SawYourSkull()
    {
        healthBar.Damage(0.05f);
        state = State.HURT;       
        player.velocity = new Vector2(hurtForce/2, player.velocity.y);
        yield return new WaitForSeconds(1);
        state = State.IDLE;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            EnemyController monster = other.gameObject.GetComponent<EnemyController>();
            
            if (state == State.FALL)
            {
                monster.HeadSmashedIn();
                JumpHandler();
            }
            else
            {
                state = State.HURT;
                bumpSound.Play();
                HandleHealth();
                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    //enemy is to the right
                    player.velocity = new Vector2(-hurtForce, player.velocity.y);
                }
                else
                {
                    //enemy is to the left
                    player.velocity = new Vector2(hurtForce, player.velocity.y);
                }              
            }
        }            
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //TODO: Fix - It doesn't run continuously while player stays on spikes
        if (collision.tag == "TRAPS/Spikes")
        {
            healthBar.Damage(0.002f);
        }

    }


    private void HandleHealth()
    {
        healthBar.Damage(0.1f);
    }

    private void FootSounds()
    {
        footsteps.Play();
    }

    private IEnumerator PowerReset()
    {
        yield return new WaitForSeconds(10);
        speed = speed - 5f;
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
