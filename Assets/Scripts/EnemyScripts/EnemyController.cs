using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{

    protected Animator enemyAnimator; /// PROTECTED VARIABLES ///
    protected Rigidbody2D enemy;
    protected AudioSource deathSound;
    [SerializeField] protected AudioClip impact;
    public Text scoreText;

    protected virtual void Start()
    /// PROTECTED ACCESS: Only children can access ///
    /// VIRTUAL  ///
    {
        enemyAnimator = GetComponent<Animator>();
        enemy = GetComponent<Rigidbody2D>();
        deathSound = GetComponent<AudioSource>();
    }

    public void HeadSmashedIn()
    {
        ScoreController.totalScore += 1;
        scoreText.text = "SCORE: " + ScoreController.totalScore;
        enemyAnimator.SetTrigger("Death");
        deathSound.PlayOneShot(impact, 0.7f);
        // prevent dead enemy from moving and colliding
        enemy.velocity = Vector2.zero;
        enemy.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;
    }

    private void Death()
    {
        Destroy(this.gameObject);
    }
}
