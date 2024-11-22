using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 10f;
    public float groundDist = 2f; // for raycast
    public LayerMask groundLayer;
    Rigidbody2D rb;

    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0f);
        MovementUpdate(playerInput);
    }

    private void MovementUpdate(Vector2 playerInput)
    {

        rb.AddForce(transform.right * playerInput * moveSpeed);

        // Draw the groundCheck Raycast
        // https://docs.unity3d.com/ScriptReference/Debug.DrawRay.html
        Vector3 groundCheck = transform.TransformDirection(-Vector3.up) * groundDist;
        Debug.DrawRay(transform.position, groundCheck, Color.red);


        if (!IsGrounded())
        {
            Debug.Log("Not grounded!");
           // rb.AddForce(-transform.up * 5f);
        }
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
