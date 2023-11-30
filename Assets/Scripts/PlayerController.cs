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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() 
    {
        horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * horizontalInput * Time.deltaTime * speed);
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
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isOnGround = true;
        doubleJump = false;
    }
}