using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float maxSpeed = 10f;
    public float timeToReachMaxSpeed = 1f;

    public float decelSpeed = 1f;
    public float timeDecel = 1f;

    private float acceleration;
    private float deceleration;
    public float groundDist = 2f; // for raycast
    public float wallDist = 0.2f; // for raycast
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    Rigidbody2D rb;

    public float apexHeight = 1f;
    public int apexTime = 1;

    public float calculatedGrav = 0f;

    public float terminalSpeed = 4f;

    public float coyoteTime = 9f;


    // for getting input in update
    bool pressJump = false;
    bool pressDash = false;

    bool canJump = true; //for coyotetime
    public GameObject ghostSprite;

    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        acceleration = maxSpeed / timeToReachMaxSpeed;
        deceleration = decelSpeed / timeDecel;
        rb = GetComponent<Rigidbody2D>();
        calculatedGrav = 2 * apexHeight / ((apexTime) ^ 2);
        rb.gravityScale = 0;
    }

    private void Update()
    {
        if(IsGrounded())
        {
            canJump = true;
        }

        if(IsAtWallLeft() || IsAtWallRight())
        {
            canJump = true;
           
        }

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            pressJump = true;

            if (!IsGrounded())
            { canJump = false; }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && IsGrounded())
        {
            pressDash = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
        MovementUpdate(playerInput);

        if (pressJump)
        {
            pressJump = false;
            Jump();
        }

        if (pressDash)
        {
            pressDash = false;
            StartCoroutine(ApplyBoost());
            float dashDirection = playerInput.x;
            rb.AddForce(transform.right * (dashDirection * 20f), ForceMode2D.Impulse);
        }

        acceleration = maxSpeed / timeToReachMaxSpeed;
        deceleration = decelSpeed / timeDecel;

    }


    private void Jump()
    {
        if(IsAtWallLeft())
        {
            Debug.Log("plase");
            rb.AddForce(transform.right * 15, ForceMode2D.Impulse);
        }

        if (IsAtWallRight())
        {
            Debug.Log("plase2x");
            rb.AddForce(-transform.right * 15, ForceMode2D.Impulse);
        }

        Vector2 jumpHeight = new Vector2(0f, (9 * (apexHeight / apexTime)));
        rb.AddForce(transform.up * jumpHeight, ForceMode2D.Impulse);
    }


    private void MovementUpdate(Vector2 playerInput)
    {

        // rb.AddForce(transform.right * playerInput * moveSpeed);
        rb.velocity += Vector2.right * playerInput * (acceleration * Time.deltaTime);

        if (playerInput.x == 0 && rb.velocity.x > 0.1f || rb.velocity.x < 0.1f)
        {
            // rb.velocity -= Vector2.right * deceleration * Time.deltaTime;
        }

        // Draw the groundCheck Raycast
        // https://docs.unity3d.com/ScriptReference/Debug.DrawRay.html
        Vector3 groundCheck = transform.TransformDirection(-Vector3.up) * groundDist;
        Debug.DrawRay(transform.position, groundCheck, Color.red);

        Vector3 wallCheck = transform.TransformDirection(-Vector3.right) * wallDist;
        Debug.DrawRay(transform.position, wallCheck, Color.blue);


        if (!IsGrounded())
        {
           // Debug.Log("Not grounded!");
            rb.gravityScale = calculatedGrav;
            StartCoroutine(CoyoteTime());

        }
        else
        {
            rb.gravityScale = 0;
            canJump = true;
        }

        // terminal velocity
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -terminalSpeed, terminalSpeed));

    }

    public bool IsWalking()
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            return true;
        }

        return false;
    }
    public bool IsGrounded()
    {
        bool isGrounded = Physics2D.Raycast(transform.position, -transform.up, groundDist, groundLayer);
        return isGrounded;
    }

    public bool IsAtWallLeft()
    {
        bool isAtWall = Physics2D.Raycast(transform.position, -transform.right, wallDist, wallLayer);
        return isAtWall;
    }

    public bool IsAtWallRight()
    {
        bool isAtWall = Physics2D.Raycast(transform.position, transform.right, wallDist, wallLayer);
        return isAtWall;
    }

    public FacingDirection GetFacingDirection()
    {
        if (rb.velocity.x < 0.1)
        {
            return FacingDirection.left;
        }

        if (rb.velocity.x > 0.1)
        {
            return FacingDirection.right;
        }

        // better to get the previous valid direction instead -- USE BOOL
        return FacingDirection.left;
    }

    IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(coyoteTime);
        if (!IsGrounded()) { canJump = false; }
      //  Debug.Log("TimeEnded!");
    }


    IEnumerator ApplyBoost()
    {
        yield return new WaitForSeconds(0.1f);
        Instantiate(ghostSprite, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(ghostSprite, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(ghostSprite, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(ghostSprite, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.35f);
        // hitboxDEMO.gameObject.SetActive(true);
    }
}
