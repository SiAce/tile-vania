using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float climbSpeed;
    [SerializeField] Vector2 deathKick = new Vector2(20, 20);
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletPoint;
    float startGravityScale;


    Rigidbody2D rb;
    Animator animator;
    CapsuleCollider2D bodyCollider;
    BoxCollider2D feetCollider;


    Vector2 moveInput;

    public bool playerHasHorizontalSpeed;
    bool isAlive = true;


    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
        startGravityScale = rb.gravityScale;
    }


    // Update is called once per frame
    void Update()
    {
        if (!isAlive) { return; }
        playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;

        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    private void OnFire(InputValue inputValue)
    {
        if (!isAlive) { return; }
        Instantiate(bulletPrefab, bulletPoint.position, transform.rotation);
    }

    void OnMove(InputValue inputValue)
    {
        if (!isAlive) { return; }
        moveInput = inputValue.Get<Vector2>();
    }

    void OnJump(InputValue inputValue)
    {
        if (!isAlive) { return; }
        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }

        if (inputValue.isPressed)
        {
            rb.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
        }
    }


    private void Run()
    {
        rb.velocity = new Vector2(moveSpeed * moveInput.x, rb.velocity.y);
        animator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    private void FlipSprite()
    {
        if (moveInput.x == 0) { return; }
        transform.localScale = new Vector2(Mathf.Sign(moveInput.x), 1);
    }

    private void ClimbLadder()
    {
        if (!bodyCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            rb.gravityScale = startGravityScale;
            animator.SetBool("isClimbing", false);
            return;
        }
        rb.gravityScale = 0;
        rb.velocity = new Vector2(rb.velocity.x, climbSpeed * moveInput.y);

        bool playerHasVerticalSpeed = Mathf.Abs(rb.velocity.y) > Mathf.Epsilon;

        animator.SetBool("isClimbing", playerHasVerticalSpeed);
    }

    private void Die()
    {
        if (bodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazard")))
        {
            isAlive = false;
            animator.SetTrigger("die");
            rb.velocity = deathKick;
            StartCoroutine(GameSession.Instance.ProcessPlayerDeath());
        }
    }
}
