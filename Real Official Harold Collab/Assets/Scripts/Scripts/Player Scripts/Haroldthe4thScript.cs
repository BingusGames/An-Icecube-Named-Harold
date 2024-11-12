using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haroldthe4thScript : MonoBehaviour
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
    public bool isFacingRight = true;

    private float pressTime;

    private float elapsedTime;

    private bool isTiming = false;
    private float timerStartTime;

    private bool canDash = true;
    private bool canRegularDash = true;
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

    private float timeSinceLanded = 0f;
    private float shortDashWindow = 0.05f;
    private bool wasGroundedLastFrame = false;
    public bool isGrounded;
    private int numOfDashes = 0;
    private bool canPressButton = true;
    public float timerDuration = 0.85f;
    private float timer;


    public GameObject player;
    public Transform respawnPoint;
    private Animator animate;

    //followcameraobject trying not to fuck this up

    private FollowCameraObject _followCameraObject;

    private float _fallSpeedYDampingChangeThreshold;

    [Header("Camera Stuff")]
    [SerializeField] private GameObject _followCameraGO;



    // knockback effect
    public float KBForce;
    public float KBCounter;
    public float KBTotalTime;

    public bool KnockFromRight;












    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;



    private void Start()
    {
        animate = gameObject.GetComponent<Animator>();
        
        rb = gameObject.GetComponent<Rigidbody2D>();
        //moveSpeed = 3f;

        //for followcameraobject script
        _followCameraObject = _followCameraGO.GetComponent<FollowCameraObject>();

        _fallSpeedYDampingChangeThreshold = CameraManager.instance._fallSpeedYDampingChangeThreshold;



    }



    // Update is called once per frame
    void Update()
    {
        if (canMove == false)
        {
            return;
        }




        //knockback
        if (KBCounter <= 0)
        {
            canMove = true;
        }
        else
        {
            if (KnockFromRight == true)
            {
                rb.velocity = new Vector2(-KBForce, KBForce);
            }
            if (KnockFromRight == false)
            {
                rb.velocity = new Vector2(KBForce, KBForce);
            }

            KBCounter -= Time.deltaTime;
        }











        //if we are falling past a certain speed threshold
        if (rb.velocity.y < _fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        //if we are standing still or moving up
        if (rb.velocity.y >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            //reset so it can be called again
            CameraManager.instance.LerpedFromPlayerFalling = false;

            CameraManager.instance.LerpYDamping(false);
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

        //trying out evans dash reset idea:
        
        if(numOfDashes > 0)
        {
            canDash = true;
        }
        else
        {
            canDash = false;
        }
        

        //timeSinceLanded += Time.deltaTime;

        /*
        if (timeSinceLanded < shortDashWindow)
        {
            canDash = true;
        }
       */

        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, groundLayer).collider != null;


        if (isGrounded && !wasGroundedLastFrame)
        {
            // The player just touched the ground
            if(numOfDashes == 0)
            {
                //canDash = true;
                numOfDashes++;
            }

            //numOfDashes = 1;
            //ResetTimeSinceLanded();
        }

        if (numOfDashes > 1 && isGrounded)
        {
            numOfDashes--;
        }
        else if (numOfDashes < 0)
        {
            numOfDashes = 0;
        }


        Debug.Log(canDash);


        if (timer > 0f)
        {
            // Decrease the timer each frame
            timer -= Time.deltaTime;
            canRegularDash = false;

         
        }else if(timer <= 0f)
        {
            canRegularDash = true;
            // Timer has elapsed, perform actions or stop the timer
            TimerExpired();
        }






        if (((isTouchingLeft || isTouchingRight) && !IsGrounded()) && Input.GetButtonDown("Jump"))
        {
            numOfDashes++;
           
        }
        
        //end of evans dash section


        if (Input.GetButtonDown("Fire1") && canDash == true)
        {
            if (vertical > 0.5f && Mathf.Abs(horizontal) > 0.1f)
            {
                StartCoroutine(UpDiagonalDash());
              
            }
            else if (vertical < -0.5f && Mathf.Abs(horizontal) > 0.1f)
            {
                if (!isGrounded)
                {
                    StartCoroutine(DownDiagonalDash());
                }else if (isGrounded)
                {
                    if(canRegularDash == true)
                    {
                        StartCoroutine(Dash());
                    }
                    
                }
                        
                
            }
            else
            {
                if (vertical > 0.4f && Mathf.Abs(horizontal) <= 0.1f)
                {
                    StartCoroutine(UpDash());
                  

                }
                else if (vertical < -0.4f && Mathf.Abs(horizontal) <= 0.1f)
                {
                    StartCoroutine(DownDash());

                }
                else
                {
                    if(canRegularDash == true)
                    {
                        StartCoroutine(Dash());
                    }
                    


                }
            }




        }





        animate.SetBool("IsAttack", Input.GetKeyDown(KeyCode.O));

        
        animate.SetFloat("Speed", Mathf.Abs(moveHorizontal));

        WallJump();




        Flip();

        wasGroundedLastFrame = isGrounded;



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

            //turn the follow camera object
            _followCameraObject.CallFlip();
        }
    }

    
  


    private IEnumerator ButtonPressCooldown(float cooldownDuration)
    {
        canPressButton = false;
        yield return new WaitForSeconds(cooldownDuration); // Adjust the duration as needed
        canPressButton = true;
    }

    private IEnumerator Dash()
    {
        canMove = false;
        numOfDashes = 0;
        //canDash = false;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        

        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        canMove = true;
        StartTimer();
        //yield return new WaitForSeconds(dashingCD);
        //timeSinceLanded = 0.1f;
        /*
        if (isGrounded)
        {
            if(numOfDashes <=0)
            {
                
                numOfDashes++;
            }
            
        }else if (!isGrounded)
        {
            numOfDashes--;
        }
        */

        numOfDashes++;
        
        
    }

    private void StartTimer()
    {
        timer = timerDuration;
        Debug.Log("Timer started!");
    }

    private void TimerExpired()
    {
        timer = 0f;
        Debug.Log("Timer expired!");
        // Perform actions when the timer expires
    }

    private IEnumerator UpDash()
    {
        numOfDashes = 0;
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
        //timeSinceLanded = 0.1f;
        //numOfDashes = 1;
        //canDash = true;
        

    }

    private IEnumerator DownDash()
    {
        numOfDashes = 0;
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
        //timeSinceLanded = 0.1f;
        //numOfDashes = 1;
        //canDash = true;

    }

    private IEnumerator UpDiagonalDash()
    {
        numOfDashes = 0;
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
        //timeSinceLanded = 0.1f;
        //numOfDashes = 1;
        //canDash = true;
        

    }

    private IEnumerator DownDiagonalDash()
    {
        numOfDashes = 0;
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
        //timeSinceLanded = 0.1f;
        //numOfDashes = 1;
        //canDash = true;

    }

    private void WallJump()
    {
        isTouchingLeft = Physics2D.OverlapBox(new Vector2(gameObject.transform.position.x - 0.65f, gameObject.transform.position.y), new Vector2(0.2f, 0.6f), 0f, groundLayer);
        isTouchingRight = Physics2D.OverlapBox(new Vector2(gameObject.transform.position.x + 0.65f, gameObject.transform.position.y), new Vector2(0.2f, 0.6f), 0f, groundLayer);

        if (isTouchingLeft)
        {
            touchingLeftOrRight = 1;

        }
        else if (isTouchingRight)
        {
            touchingLeftOrRight = -1;

        }


        if (((isTouchingLeft || isTouchingRight) && !IsGrounded()) && rb.velocity.y < -0.1f)
        {



            isWallSliding = true;

            //Debug.Log(isWallSliding);

            rb.velocity = new Vector2(0f, rb.velocity.y * wallSlideSpeed);







        }
        else if (((isTouchingLeft || isTouchingRight) && IsGrounded()) || IsGrounded() || (!IsGrounded() && !(isTouchingLeft || isTouchingRight)))
        {
            isWallSliding = false;
            //Debug.Log(isWallSliding);



        }


        if (isWallSliding)
        {
            canMove2 = false;
        }
        if (!isWallSliding)
        {
            canMove2 = true;
        }


        //UNCOMMENT THE INSIDE OF THIS IF STATEMENT FOR DASH BUFF**********************************************************
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            if(numOfDashes == 0)
            {
                //numOfDashes++;
            }
            
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            rb.gravityScale = 8.17f;
        }

        if (Input.GetButtonDown("Jump"))
        {
            rb.gravityScale = 8.17f;
            elapsedTime = 0f;
            isTiming = true;
            timerStartTime = Time.time;
        }

        if ((isTiming && GetElapsedTime() >= 0.05f) || (Input.GetButtonUp("Jump") && GetElapsedTime() < 0.05f))
        {
            //elapsedTime = 0f;
            elapsedTime = GetElapsedTime();
            //Debug.Log("Button held for " + elapsedTime + " seconds.");

            if ((isTouchingRight || isTouchingLeft) && !IsGrounded())
            {

                if (elapsedTime > 0.05f)
                {
                    elapsedTime = 0.05f;
                }



                wallJumping = true;
                //Debug.Log("Button held for " + elapsedTime + " seconds.");
                Invoke(nameof(SetJumpingToFalse), (elapsedTime * 1.5f));



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
        Gizmos.DrawCube(new Vector2(gameObject.transform.position.x - 0.65f, gameObject.transform.position.y), new Vector2(0.2f, 0.6f));
        Gizmos.DrawCube(new Vector2(gameObject.transform.position.x + 0.65f, gameObject.transform.position.y), new Vector2(0.2f, 0.6f));
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.down * 0.5f);
     
    }





}
