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
    public LayerMask groundLayer;
    Rigidbody2D rb;

    public float apexHeight = 1f;
    public int apexTime = 1;

    public float calculatedGrav = 0f;

    public float terminalSpeed = 4f;

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

    // Update is called once per frame
    void FixedUpdate()
    {
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
        MovementUpdate(playerInput);

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            Jump();
            Debug.Log("TEST");
        }

        acceleration = maxSpeed / timeToReachMaxSpeed;
        deceleration = decelSpeed / timeDecel;

    }


    private void Jump()
    {
        Vector2 test = new Vector2(0f, (9 * (apexHeight / apexTime)));
        rb.AddForce(transform.up * test, ForceMode2D.Impulse);
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


        if (!IsGrounded())
        {
            Debug.Log("Not grounded!");
            rb.AddForce(-transform.up * calculatedGrav);
            rb.gravityScale = calculatedGrav;
        } else
        {
            rb.gravityScale = 0;
        }

        // terminal velocity
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, terminalSpeed);

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

        // better to get the previous valid direction instead
        return FacingDirection.left; 
    }
}
