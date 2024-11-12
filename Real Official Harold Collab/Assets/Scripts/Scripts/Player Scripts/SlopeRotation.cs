using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class SlopeRotation : MonoBehaviour
{
    private bool isGrounded;
    public float slopeCheckDistance = 0.1f; // distance to check the slope
    public float slopeRotationSpeed = 10f; // speed at which to rotate the player

    private Rigidbody2D rb; // reference to the rigidbody component
    private SpriteRenderer spriteRenderer; // reference to the sprite renderer component

    private void Start()
    {
        // get the references to the rigidbody and sprite renderer components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 1f, LayerMask.GetMask("ground"));
        isGrounded = hit.collider != null;
        // get the normal vector of the ground below the player
        Vector2 slopeNormal = GetSlopeNormal();

        // rotate the player to match the slope of the ground
        if (isGrounded)
        {
            float slopeAngle = Mathf.Atan2(slopeNormal.x, slopeNormal.y) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, -slopeAngle);
            // transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * slopeRotationSpeed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * slopeRotationSpeed);
        }
        else
        {
            // Reset the character's rotation when in the air
            //transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, Time.deltaTime * rotationSpeed);
            transform.rotation = Quaternion.identity;
        }
    }

    private Vector2 GetSlopeNormal()
    {
        // check the slope below the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, slopeCheckDistance, LayerMask.GetMask("ground"));

        // return the normal vector of the slope
        if (hit.collider != null)
        {
            return hit.normal;
        }
        else
        {
            return Vector2.up;
        }
    }
}
