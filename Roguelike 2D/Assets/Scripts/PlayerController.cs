using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;

    public int movementSpeed;

    Vector2 movement;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement.x != 0)
        {
            transform.localScale = new Vector3(movement.x, 1, 1);
        }
        ChangeAnimation();
    }

    private void FixedUpdate()
    {
        // rb.transform is the subset of rb.position?
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);
    }

    void ChangeAnimation()
    {
        anim.SetFloat("speed", movement.magnitude);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // collide with interactive
        if (collision.CompareTag("Interactive"))
        {
            Debug.Log("Player is collided with [" + collision.name + "].");
            transform.position.Set(0, 0, 0);
            if (collision.name.Equals("Ladder(Clone)"))
            {
                // BoardController.instance.NewLevel();
            }
        }

        if (collision.CompareTag("Enemy"))
        {
            // Debug.Log("Player is collided with [" + collision.name + "].");
        }
        
    }
}
