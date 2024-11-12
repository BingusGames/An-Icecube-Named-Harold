using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class HaroldScript : MonoBehaviour
{
    private float horizontal;
    private float moveHorizontal;
    private float vertical;
    //public float speed = 8f;
    public float moveSpeed;
    public float wallJumpSideWaysPower;
    public float wallJumpUpPower;
    public float wallSlideSpeed;
    public float acceleration = 5f;
    public float decceleration = -5f;
    public float velPower = 1f;
    public float jumpingPower = 16f;
    private bool isFacingRight = true;

    private float pressTime;

    private float elapsedTime;

    private bool isTiming = false;
    private float timerStartTime;

    private bool canDash = true;
    //private bool isDashing;
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCD = 1f;
    //private bool hasDash = false;
    private bool canMove = true;
    private bool canMove2 = true;
    private bool isTouchingLeft;
    private bool isTouchingRight;
    private bool wallJumping;
    private float touchingLeftOrRight;

    private bool isWallSliding;
    
    public float upDashingPower = 24f;
    public float downDashingPower = -24f;
    public float upDiagonalDashingPower = 24f;
    public float downDiagonalDashingPower = -24f;
    public float upDashingTime = 0.2f;
    public float upDashingCD = 1f;

    

    public GameObject player;
    public Transform respawnPoint;
    private Animator animate;


    //this actually works!!!!



    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;



    private void Start()
    {
        animate = gameObject.GetComponent<Animator>();

        rb = gameObject.GetComponent<Rigidbody2D>();
        //moveSpeed = 3f;
    }


    // Update is called once per frame
    void Update()
    {
        if (canMove == false)
        {
            return;
        }

        




        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }
        
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }





        if (((isTouchingLeft || isTouchingRight) && !IsGrounded()) && Input.GetButtonDown("Jump"))
        {
            canDash = true;
        }else if(isWallSliding == true)
        {
            canDash = true;
        }


        if (Input.GetButtonDown("Fire1") && canDash == true)
        {
            if ( vertical > 0.5f && Mathf.Abs(horizontal) > 0.2f)
            {
                StartCoroutine(UpDiagonalDash());
            }
            else if (vertical < - 0.5f && Mathf.Abs(horizontal) > 0.2f)
            {
                StartCoroutine(DownDiagonalDash());
            }
            else
            {
                if (vertical > 0.4f)
                {
                    StartCoroutine(UpDash());

                }
                else if (vertical < -0.4f)
                {
                    StartCoroutine(DownDash());

                }
                else
                {
                    StartCoroutine(Dash());
                }
            }

            


        }







        animate.SetFloat("Speed", Mathf.Abs(horizontal));


        WallJump();





        Flip();

    

    }

    private void FixedUpdate()
    {
        
        if (canMove == false)
        {
            return;
        }

        

        //rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        if (canMove2 == true)
        {
            float targetSpeed = moveHorizontal * moveSpeed;

            float speedDif = targetSpeed - rb.velocity.x;

            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;

            float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

            rb.AddForce(movement * Vector2.right);
        }


   

    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.45f, groundLayer);
       
    }



    
    private void Flip()
    {
        if (isFacingRight == true && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    



  


    private IEnumerator Dash()
    {
        canMove = false;
        canDash = false;
        //isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);


        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        //isDashing = false;
        canMove = true;
        yield return new WaitForSeconds(dashingCD);
        canDash= true;
        
    }

    private IEnumerator UpDash()
    {
        canDash = false;
        //isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 20f;

        rb.velocity = new Vector2(0f, transform.localScale.y * upDashingPower);


        tr.emitting = true;
        yield return new WaitForSeconds(upDashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        //isDashing = false;
        yield return new WaitForSeconds(upDashingCD);
        canDash = true;

    }

    private IEnumerator DownDash()
    {
        canDash = false;
        //isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        rb.velocity = new Vector2(0f, transform.localScale.y * downDashingPower);


        tr.emitting = true;
        yield return new WaitForSeconds(upDashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        //isDashing = false;
        yield return new WaitForSeconds(upDashingCD);
        canDash = true;

    }

    private IEnumerator UpDiagonalDash()
    {
        canDash = false;
        //isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 20f;

        rb.velocity = new Vector2(transform.localScale.x * dashingPower, transform.localScale.y * upDiagonalDashingPower);


        tr.emitting = true;
        yield return new WaitForSeconds(upDashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        //isDashing = false;
        yield return new WaitForSeconds(upDashingCD);
        canDash = true;

    }

    private IEnumerator DownDiagonalDash()
    {
        canDash = false;
        //isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        rb.velocity = new Vector2(transform.localScale.x * dashingPower, transform.localScale.y * downDiagonalDashingPower);


        tr.emitting = true;
        yield return new WaitForSeconds(upDashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        //isDashing = false;
        yield return new WaitForSeconds(upDashingCD);
        canDash = true;

    }

    private void WallJump()
    {
        isTouchingLeft = Physics2D.OverlapBox(new Vector2(gameObject.transform.position.x - 0.52f, gameObject.transform.position.y), new Vector2(0.1f, 0.5f), 0f, groundLayer);
        isTouchingRight = Physics2D.OverlapBox(new Vector2(gameObject.transform.position.x + 0.52f, gameObject.transform.position.y), new Vector2(0.1f, 0.5f), 0f, groundLayer);

        if (isTouchingLeft)
        {
            touchingLeftOrRight = 1;

        }
        else if (isTouchingRight)
        {
            touchingLeftOrRight = -1;

        }

        


        if (((isTouchingLeft || isTouchingRight) && !IsGrounded()) && rb.velocity.y < 0f)
        {

            

            isWallSliding = true;
         

            rb.velocity = new Vector2(0f, rb.velocity.y * wallSlideSpeed);


           




        }
        else if (((isTouchingLeft || isTouchingRight) && IsGrounded()) || IsGrounded() || (!IsGrounded() && !(isTouchingLeft || isTouchingRight)))
        {
            isWallSliding = false;
         



        }


        if (isWallSliding)
        {
            canMove2 = false;
        }
        if (!isWallSliding)
        {
            canMove2 = true;
        }


        if (Input.GetButtonDown("Jump"))
        {
            elapsedTime = 0f;
            isTiming = true;
            timerStartTime = Time.time;
        }

        if ((isTiming && GetElapsedTime() >= 0.16f) || (Input.GetButtonUp("Jump") && GetElapsedTime() < 0.16f))
        {
            //elapsedTime = 0f;
            elapsedTime = GetElapsedTime();
            Debug.Log("Button held for " + elapsedTime + " seconds.");

            if ((isTouchingRight || isTouchingLeft) && !IsGrounded())
            {

                if (elapsedTime > 0.16f)
                {
                    elapsedTime = 0.16f;
                }



                wallJumping = true;
                Invoke(nameof(SetJumpingToFalse), elapsedTime);



            }
            elapsedTime = 0f;
            isTiming = false;
            //timerStartTime = 0f;


            // Disengage the button
            //Input.GetButtonUp("Jump");


        }

        if (Input.GetButtonDown("Jump") && (isTouchingRight || isTouchingLeft) && IsGrounded())
        {
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }

            if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }

        }



        if (wallJumping)
        {

            //canMove = false;


            rb.velocity = new Vector2(wallJumpSideWaysPower * touchingLeftOrRight, wallJumpUpPower);
            //canMove = true;
        }
    }


  

  


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Death"))
        {
            player.transform.position = respawnPoint.position;
        }
    }


  

    void SetJumpingToFalse()
    {
        wallJumping = false;
    }

    void SetCanMoveToFalse()
    {
        canMove2 = false;
    }



    private float GetElapsedTime()
    {
        return Time.time - timerStartTime;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(new Vector2(gameObject.transform.position.x - 0.52f, gameObject.transform.position.y), new Vector2(0.1f, 0.5f));
        Gizmos.DrawCube(new Vector2(gameObject.transform.position.x + 0.52f, gameObject.transform.position.y), new Vector2(0.1f, 0.5f));
    }








}



