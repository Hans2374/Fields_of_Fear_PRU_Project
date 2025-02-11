using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
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
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Nếu đang tưới nước, không cho phép di chuyển
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Watering"))
        {
            movement = Vector2.zero;
            return;
        }

        // Lấy input từ người dùng để xác định hướng di chuyển
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;

        if (movement != Vector2.zero)
        {
            animator.SetBool("isWalking", true);
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        // Chạy khi giữ shift
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
        }

        // Nhấn F để tưới nước nếu đủ Stamina
        if (Input.GetKeyDown(KeyCode.F) && Stamina >= AttackCost && movement == Vector2.zero)
        {
            animator.SetTrigger("Watering"); // Bật animation tưới nước
            Stamina -= AttackCost;

            if (Stamina < 0) Stamina = 0;
            StaminaBar.fillAmount = Stamina / MaxStamina;

            if (recharge != null) StopCoroutine(recharge);
            recharge = StartCoroutine(RechargeStamina());
        }
    }

    void FixedUpdate()
    {
        // Nếu đang tưới nước, không di chuyển
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Watering")) return;

        float currentSpeed;
        if (isRunning)
        {
            currentSpeed = moveSpeed * 1.5f;

            Stamina -= RunCost * Time.deltaTime;
            if (Stamina < 0) Stamina = 0;
            if (Stamina == 0) isRunning = false;
            StaminaBar.fillAmount = Stamina / MaxStamina;
            if (recharge != null) StopCoroutine(recharge);
            recharge = StartCoroutine(RechargeStamina());
        }
        else
        {
            currentSpeed = moveSpeed;
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
