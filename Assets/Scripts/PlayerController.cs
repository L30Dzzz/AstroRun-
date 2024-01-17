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

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;
    int direction;

    [SerializeField] private TrailRenderer tr;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
            direction = 1;
        }
        else if(horizontalInput < 0)
        {
            direction = -1;
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

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
        
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        rb.velocity = new Vector2(speed * horizontalInput, rb.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isOnGround = true;
        doubleJump = false;
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