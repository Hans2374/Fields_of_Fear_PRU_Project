using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Tốc độ di chuyển của nhân vật
    private Animator animator;
    private Vector2 movement;
 
    void Start()
    {
        animator = GetComponent<Animator>();  // Lấy Animator của nhân vật
    }

    void Update()
    {
        // Lấy input từ người dùng để xác định hướng di chuyển
       
            movement.x = Input.GetAxisRaw("Horizontal");  // A/D hoặc phím mũi tên trái/phải
            movement.y = Input.GetAxisRaw("Vertical");    // W/S hoặc phím mũi tên lên/xuống
   
        // Kiểm tra nếu có di chuyển thì bật trạng thái walking
        if (movement != Vector2.zero)
        {
            animator.SetBool("isWalking", true);  // Chuyển sang trạng thái walking
            animator.SetFloat("MoveX", movement.x);  // Xác định hướng đi trên trục X
            animator.SetFloat("MoveY", movement.y);  // Xác định hướng đi trên trục Y
        }
        else
        {
            animator.SetBool("isWalking", false);  // Nếu không di chuyển thì idle
        }
    }

    void FixedUpdate()
    {
     transform.Translate(moveSpeed * Time.fixedDeltaTime * movement.normalized, Space.World);
    }
}
