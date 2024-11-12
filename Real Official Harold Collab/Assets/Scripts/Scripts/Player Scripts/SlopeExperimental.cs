using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeExperimental : MonoBehaviour
{
    public bool isGrounded;
    public float slopeCheckDistance = 0.1f; // distance to check the slope
    //public float slopeRotationSpeed = 10f; // speed at which to rotate the player

    private Rigidbody2D rb; // reference to the rigidbody component
    private SpriteRenderer spriteRenderer; // reference to the sprite renderer component

    private void Start()
    {
        // get the references to the rigidbody and sprite renderer components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Awake()
    {
        Application.targetFrameRate = 120; // Adjust the frame rate as needed

    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 1f, LayerMask.GetMask("ground"));
        //RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x - 0.35f, transform.position.y + 0.77f), -Vector2.up, 1f, LayerMask.GetMask("ground"));

        isGrounded = hit.collider != null;
        // get the normal vector of the ground below the player
        Vector2 slopeNormal = GetSlopeNormal();

        // rotate the player to match the slope of the ground
        
        if (isGrounded)
        {
            
            float slopeAngle = Mathf.Atan2(slopeNormal.x, slopeNormal.y) * Mathf.Rad2Deg;
            /*
            if (slopeAngle > 1f || slopeAngle < -1f)
            {
                //rb.constraints = RigidbodyConstraints2D.none

                if (slopeAngle > 45f || slopeAngle < -45f)
                {
                    rb.freezeRotation = true;
                }
                else
                {
                    rb.freezeRotation = false;
                }


            }
            else
            {
                transform.rotation = Quaternion.identity;
                rb.freezeRotation = true;
            }
            */
            if (Mathf.Abs(slopeAngle) > 1f)
            {
                // Enable rotation within plus or minus 45 degrees on the Z-axis
                rb.freezeRotation = Mathf.Abs(slopeAngle) > 44f;

                // Ensure that rotation is clamped within -45 to 45 degrees
                float clampedRotation = Mathf.Clamp(rb.rotation, -44f, 44f);
                rb.rotation = clampedRotation;
            }
            else
            {
                // Reset the character's rotation when the slope is too shallow
                transform.rotation = Quaternion.identity;
                rb.freezeRotation = true;
            }


        }
        else
        {
            // Reset the character's rotation when in the air
            //transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, Time.deltaTime * rotationSpeed);
            transform.rotation = Quaternion.identity;
            rb.freezeRotation = true;
        }
        

    }

    private Vector2 GetSlopeNormal()
    {
        // check the slope below the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, slopeCheckDistance, LayerMask.GetMask("ground"));
        //RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x - 0.35f, transform.position.y + 0.77f), Vector2.down, slopeCheckDistance, LayerMask.GetMask("ground"));
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

    private void OnDrawGizmosSelected()
    {
        //Debug.DrawRay(new Vector2(transform.position.x -0.35f, transform.position.y + 0.77f), Vector2.down * slopeCheckDistance, Color.green);
        //Debug.DrawRay(transform.position, Vector2.down * slopeCheckDistance, Color.green);
    }
}
