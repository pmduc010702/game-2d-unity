using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// OOP Inheritance from main EnemyController. AngryPig is a child of EnemyController
public class AngryPig : EnemyController
{
    [Header("Enemy Bounds")]
    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;

    [Header("Enemy Movement")]
    [SerializeField] private float jumpLength = 2;
    [SerializeField] private float jumpHeight = 2;
    [SerializeField] private LayerMask Ground;

    private Collider2D enemyHitBox;

    private bool isFacingLeft = true;

    /// POLYMORPHISM AND INHERITANCE ///
    protected override void Start() /// child override
    {
        base.Start(); /// ACCESS parent start method
        enemyHitBox = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovementHandler();
    }

    private void EnemyMovementHandler()
    {
        if (isFacingLeft)
        {
            if (transform.position.x > leftCap)
            {
                if (transform.localScale.x != 1)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }

                if (enemyHitBox.IsTouchingLayers(Ground))
                {
                    enemy.velocity = new Vector2(-jumpLength, jumpHeight);
                }
            }
            else
            {
                isFacingLeft = false;
            }
        }
        else
        {
            if (transform.position.x < rightCap)
            {
                if (transform.localScale.x != -1)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }

                if (enemyHitBox.IsTouchingLayers(Ground))
                {
                    enemy.velocity = new Vector2(jumpLength, jumpHeight);
                }
            }
            else
            {
                isFacingLeft = true;
            }
        }
    }


}
