using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Tốc độ di chuyển của nhân vật
    private Animator animator;
    private Vector2 movement;
    private bool isRunning = false;

    public Image StaminaBar;

    public float Stamina, MaxStamina;

    public float AttackCost;
    public float RunCost;
    public float ChargeRate;

    private Coroutine recharge;


    void Start()
    {
        animator = GetComponent<Animator>();  // Lấy Animator của nhân vật
    }

    void Update()
    {
        // Lấy input từ người dùng để xác định hướng di chuyển
       
            movement.x = Input.GetAxisRaw("Horizontal");  // A/D hoặc phím mũi tên trái/phải
            movement.y = Input.GetAxisRaw("Vertical");    // W/S hoặc phím mũi tên lên/xuống

        // Chuẩn hóa vector để đảm bảo tốc độ di chuyển đồng nhất
        movement = movement.normalized;

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
        // Kiểm tra nếu nhấn hoặc thả phím "left shift" để bật/tắt chế độ chạy
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
        }

        if (Input.GetKeyDown(KeyCode.F) && Stamina >= AttackCost)
        {
            Debug.Log("Attack!");  // In ra console khi tấn công
            Stamina -= AttackCost; // Trừ stamina khi tấn công

            if (Stamina < 0) Stamina = 0; // Đảm bảo Stamina không xuống dưới 0

            StaminaBar.fillAmount = Stamina / MaxStamina; // Cập nhật thanh thể lực

            if(recharge != null) StopCoroutine(recharge);
            recharge = StartCoroutine(RechargeStamina());
        }
    }

    void FixedUpdate()
    {
        // Nếu đang chạy, tăng tốc độ di chuyển lên 1.5 lần
        float currentSpeed;
        if (isRunning)
        {
            currentSpeed = moveSpeed * 1.5f; // Nếu đang chạy, tăng tốc độ lên 1.5 lần

            Stamina -= RunCost * Time.deltaTime;
            if (Stamina < 0) Stamina = 0; // Đảm bảo Stamina không xuống dưới 0
            if (Stamina == 0) isRunning = false;
            StaminaBar.fillAmount = Stamina / MaxStamina; // Cập nhật thanh thể lực
            if (recharge != null) StopCoroutine(recharge);
            recharge = StartCoroutine(RechargeStamina());
        }
        else
        {
            currentSpeed = moveSpeed; // Nếu không chạy, giữ nguyên tốc độ
        }


        transform.Translate(currentSpeed * Time.fixedDeltaTime * movement.normalized, Space.World);
    }

    private IEnumerator RechargeStamina()
    {
        yield return new WaitForSeconds(1f);

        while (Stamina < MaxStamina)
        {
            Stamina += ChargeRate / 10f;
            if (Stamina > MaxStamina) Stamina = MaxStamina;
            StaminaBar.fillAmount = Stamina / MaxStamina;
            yield return new WaitForSeconds(.1f);
        }
    }
}
