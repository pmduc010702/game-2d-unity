using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMonster : EnemyController
{
    private PinkPlayerController thePlayer;
    [SerializeField] private float flightSpeed;
    [SerializeField] private float patrolRange;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private bool isPlayerInRange;
    
    private SpriteRenderer birdFace;
    private float currentBirdX;

    /// POLYMORPHISM AND INHERITANCE ///
    protected override void Start() /// child override
    {
        base.Start(); /// ACCESS parent start method
        thePlayer = FindObjectOfType<PinkPlayerController>();
        currentBirdX = transform.position.x;
        birdFace = GetComponent<SpriteRenderer>();
        if (birdFace == null)
        {
            Debug.LogError("Bird Sprite is missing a renderer");
        }
    }

    private void Update()
    {
        isPlayerInRange = Physics2D.OverlapCircle(transform.position, patrolRange, playerLayer);

        if (isPlayerInRange)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                thePlayer.transform.position, flightSpeed * Time.deltaTime);
        }

        // check birds position.x and flip renderer accordingly
        if (transform.position.x < currentBirdX)
        {
            birdFace.flipX = false;
            currentBirdX = transform.position.x;
        } else if (transform.position.x > currentBirdX)
        {
            birdFace.flipX = true;
            currentBirdX = transform.position.x;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, patrolRange);
    }
}
