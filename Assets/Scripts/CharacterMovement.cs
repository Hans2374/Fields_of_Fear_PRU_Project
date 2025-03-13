using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class CharacterMovement : MonoBehaviour
{
    public bool isWatering = false;
    private float stepTimer = 0f;
    AudioManager audioManager;
    public float moveSpeed = 5f;
    private Animator animator;
    private Vector2 movement;
    private bool isRunning = false;

    public Image StaminaBar;

    public float Stamina, MaxStamina;
    public float AttackCost;
    public float RunCost;
    public float ChargeRate;
    private float lastMoveX = 0;
    private float lastMoveY = -1;

    private Coroutine recharge;

    [SerializeField] private InventoryUI inventoryUI;
    private Inventory inventory;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Log warning if StaminaBar is missing
        if (StaminaBar == null)
        {
            Debug.LogWarning("StaminaBar reference is missing in CharacterMovement!");
        }
    }

    private void Awake()
    {
        // Find AudioManager reference
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogWarning("AudioManager not found!");
        }

        // Initialize inventory
        inventory = new Inventory();

        // Check and set inventory UI
        if (inventoryUI != null)
        {
            inventoryUI.SetInventory(inventory);
            inventoryUI.SetPlayer(this);
        }
        else
        {
            Debug.LogWarning("InventoryUI reference is missing in CharacterMovement!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        ItemWorld itemWorld = collider.GetComponent<ItemWorld>();
        if (itemWorld != null && inventory != null)
        {
            inventory.AddItem(itemWorld.GetItem());
            itemWorld.DestroySelf();
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    // Safe method to update stamina bar
    private void UpdateStaminaBar()
    {
        if (StaminaBar != null)
        {
            StaminaBar.fillAmount = Stamina / MaxStamina;
        }
    }

    void Update()
    {
        // If currently watering, don't allow movement
        if (animator != null && animator.GetCurrentAnimatorStateInfo(0).IsTag("Watering"))
        {
            isWatering = true;
            movement = Vector2.zero;
            return;
        }
        else
        {
            isWatering = false;
        }

        // Handle movement input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;

        // Update animator parameters based on movement
        if (animator != null)
        {
            if (movement != Vector2.zero)
            {
                animator.SetBool("isWalking", true);
                animator.SetFloat("MoveX", movement.x);
                animator.SetFloat("MoveY", movement.y);

                // Store last movement direction
                lastMoveX = movement.x;
                lastMoveY = movement.y;

                // Play footstep sounds
                stepTimer += Time.deltaTime;
                if (stepTimer >= 0.5f && audioManager != null)
                {
                    audioManager.PlaySFX(audioManager.moveStep);
                    stepTimer = 0f;
                }
            }
            else
            {
                animator.SetBool("isWalking", false);
                stepTimer = 0f;
            }
        }

        // Handle running with shift
        if (Input.GetKeyDown(KeyCode.LeftShift) && movement != Vector2.zero)
        {
            isRunning = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
        }

        // Handle watering action
        if (Input.GetKeyDown(KeyCode.F) && Stamina >= AttackCost && movement == Vector2.zero)
        {
            if (animator != null)
            {
                animator.SetTrigger("Watering");
            }

            Stamina -= AttackCost;
            if (Stamina < 0) Stamina = 0;

            // Update stamina UI safely
            UpdateStaminaBar();

            // Play watering sound
            if (audioManager != null)
            {
                audioManager.PlaySFX(audioManager.water);
            }

            // Handle stamina recharge
            if (recharge != null) StopCoroutine(recharge);
            recharge = StartCoroutine(RechargeStamina());
        }
    }

    void FixedUpdate()
    {
        // Don't move if watering
        if (animator != null && animator.GetCurrentAnimatorStateInfo(0).IsTag("Watering")) return;

        float currentSpeed;
        if (isRunning)
        {
            currentSpeed = moveSpeed * 1.5f;

            Stamina -= RunCost * Time.deltaTime;
            if (Stamina < 0) Stamina = 0;
            if (Stamina == 0) isRunning = false;

            // Update stamina UI safely
            UpdateStaminaBar();

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

            // Update stamina UI safely
            UpdateStaminaBar();

            yield return new WaitForSeconds(.1f);
        }
    }

    // Return the inventory (for cases where external scripts need access)
    public Inventory GetInventory()
    {
        return inventory;
    }

    // Safe method to get the InventoryUI
    public InventoryUI GetInventoryUI()
    {
        return inventoryUI;
    }

    // Helper method for using items
    public void UseSelectedItem()
    {
        if (inventoryUI != null && inventoryUI.selectedItem != null)
        {
            // Logic for using the selected item
            Debug.Log($"Using item: {inventoryUI.selectedItem.itemType}");

            // Implement your item usage logic here
        }
    }
}