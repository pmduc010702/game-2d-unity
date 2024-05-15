using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobMonster : EnemyController
{
    private SpriteRenderer blobFace;
    private float currentBlobX;

    /// POLYMORPHISM AND INHERITANCE ///
    protected override void Start() /// child override
    {
        base.Start(); /// ACCESS parent start method
        currentBlobX = transform.position.x;
        blobFace = GetComponent<SpriteRenderer>();
        if (blobFace == null)
        {
            Debug.LogError("Blob is missin a sprite renderer");
        }
    }
    void Update()
    {
        // check blob position and flip sprite accordingly
        if (transform.position.x < currentBlobX)
        {
            blobFace.flipX = false;
            currentBlobX = transform.position.x;
        }
        else if (transform.position.x > currentBlobX)
        {
            blobFace.flipX = true;
            currentBlobX = transform.position.x;
        }
    }
}
