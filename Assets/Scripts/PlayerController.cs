using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float horizontalInput;
    public float speed = 10.0f;
    public float jump;
    public float doubleJumpForce;
    private Rigidbody2D rb;
    public bool isOnGround = true;
    private bool doubleJump = false;

    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    //fix dash cooldown
    private float dashingCooldown = 10f;
    int direction;

    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() 
    {
        if (isDashing)
        {

            return;
        }

        horizontalInput = Input.GetAxis("Horizontal");

        if(horizontalInput > 0)
        {
            animator.SetFloat("MoveX", horizontalInput);
        }
        else if(horizontalInput < 0)
        {
            animator.SetFloat("MoveX", horizontalInput);
        }

        // Double jump
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
        {
            isOnGround = false;
            doubleJump = true;
            rb.AddForce(new Vector2(rb.velocity.x, jump));
        }

        else if (Input.GetKeyDown(KeyCode.Space) && !isOnGround && doubleJump)
        {
            rb.AddForce(new Vector2(rb.velocity.x, doubleJumpForce));
            doubleJump = false;
        }

        WallSlide();
        WallJump();

        // just incase the player has different keys binded, or just an entirely new playing style
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) && canDash)
        {
            StartCoroutine(Dash());
        }
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isOnGround = true;
        doubleJump = false;
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            //animator.SetTrigger("Dash");
            return;
        }

        if (!isWallJumping)
        {
            rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
        }

        rb.velocity = new Vector2(speed * horizontalInput, rb.velocity.y);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    //checks wall sliding
    private void WallSlide()
    {
        if (IsWalled() && !isOnGround && horizontalInput != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower * direction, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}