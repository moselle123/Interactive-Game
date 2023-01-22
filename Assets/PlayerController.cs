using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour{

    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    Vector2 movementInput;
    Rigidbody2D rb;
    Animator animator;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    SpriteRenderer spriteRenderer;
    bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void FixedUpdate(){

        if (canMove)
        {
            if (movementInput != Vector2.zero)//if there is movement
            {
                bool success = TryMove(movementInput); // see if theres collisions
                                                       // if there are collisions check in just one direction and move that way
                if (!success)
                {
                    success = TryMove(new Vector2(movementInput.x, 0));
                    if (!success)
                    {
                        success = TryMove(new Vector2(0, movementInput.y));
                    }
                }
                // set animations based on direction and disable other directions
                if (movementInput.x > 0 || movementInput.x < 0)
                {
                    if (movementInput.x > 0)
                    {
                        spriteRenderer.flipX = false;
                    }
                    else if (movementInput.x < 0) //if left flip character
                    {
                        spriteRenderer.flipX = true;
                    }

                    animator.SetBool("isSide", true);
                    animator.SetBool("isBack", false);
                    animator.SetBool("isForward", false);
                }
                else if (movementInput.x < 0)
                {

                }

                else if (movementInput.y > 0)
                {
                    animator.SetBool("isBack", true);
                    animator.SetBool("isForward", false);
                    animator.SetBool("isSide", false);
                }
                else if (movementInput.y < 0)
                {
                    animator.SetBool("isForward", true);
                    animator.SetBool("isBack", false);
                    animator.SetBool("isSide", false);
                }
                animator.SetBool("isMoving", success); // if there is possible movement set isMoving to true 
            }
            else
            {
                animator.SetBool("isMoving", false); // if there is no movement set isMoving to false 
            }
        }
    }

    private bool TryMove(Vector2 direction){ //check for collisions in both x and y directions

        if (direction != Vector2.zero) //if there is any movement 
        {
            int count = rb.Cast(direction, movementFilter, castCollisions, moveSpeed * Time.fixedDeltaTime + collisionOffset);

            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            }
            else
            {
                return false;
            }
        }
        else // if the character can't make any movement in x or y
        {
            return false;
        }
    }

    void OnMove(InputValue movementValue){
        movementInput = movementValue.Get<Vector2>();
    }

    void OnFire()
    {
        animator.SetTrigger("weaponAttack");

    }

    public void LockMovement()
    {
        canMove = false;
    }

    public void UnlockMovement()
    {
        canMove = true;
    }
}
