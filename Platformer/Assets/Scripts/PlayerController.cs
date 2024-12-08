using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // New Mechanics:
    // [Horizontal] - Side dash (press 'left ctrl' while grounded to dash)
    // [Vertical] - Wall jumping (jump between the structures on the left side of the map)
    // [Physics / RB] - Death physics (player dies and flies up when they hit a spike. Press 'R' to restart after dying)


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
    public LayerMask deathLayer;
    Rigidbody2D rb;

    public float apexHeight = 1f;
    public int apexTime = 1;

    public float calculatedGrav = 0f;

    public float terminalSpeed = 4f;

    public float coyoteTime = 9f;


    // for getting input in update
    bool pressJump = false;
    bool pressDash = false;

    public bool isAlive = true;

    bool canJump = true; //for coyotetime
    public GameObject ghostSprite;

    public GameObject gameOverMsg; // for after the player dies

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

        if(HitDeathPlane() && isAlive)
        {
            isAlive = false;
            DeathFX();
            Debug.Log("Game Over!");
        }

        if (Input.GetKeyDown("r") && !isAlive)
        {
            Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
        }
    }

    // Handles the death physics mechanic
    // Player's movement is disabled, rb rotation is unfrozen, and they fly upwards
    private void DeathFX()
    {

        rb.freezeRotation = false;
        rb.AddForce(transform.up * 50f, ForceMode2D.Impulse);
        rb.AddForce(transform.right * 25f, ForceMode2D.Impulse);
        rb.rotation += 45f;
        gameOverMsg.SetActive(true);

    }

        // Update is called once per frame
        void FixedUpdate()
    {


        if (isAlive)
        {

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
                StartCoroutine(Dash());
                float dashDirection = playerInput.x;
                rb.AddForce(transform.right * (dashDirection * 20f), ForceMode2D.Impulse);
            }
        }


        acceleration = maxSpeed / timeToReachMaxSpeed;
        deceleration = decelSpeed / timeDecel;

    }


    private void Jump()
    {
        // if the player is jumping off a wall,
        // make them jump slightly horizontally to reach the adjacent wall
        if(IsAtWallLeft())
        {
            Debug.Log("jumping off left wall");
            rb.AddForce(transform.right * 15, ForceMode2D.Impulse);
        }

        if (IsAtWallRight())
        {
            Debug.Log("jumping off right wall");
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

    public bool HitDeathPlane()
    {
        bool hitDeathPlane = Physics2D.Raycast(transform.position, -transform.up, groundDist, deathLayer);
        return hitDeathPlane;
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

        return FacingDirection.left;
    }

    IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(coyoteTime);
        if (!IsGrounded()) { canJump = false; }
      //  Debug.Log("TimeEnded!");
    }


    IEnumerator Dash()
    {
        yield return new WaitForSeconds(0.1f);
        Instantiate(ghostSprite, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(ghostSprite, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.1f);
        Instantiate(ghostSprite, transform.position, transform.rotation);
        yield return new WaitForSeconds(0.35f);
    }
}
