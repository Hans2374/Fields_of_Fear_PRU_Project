using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

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

    [SerializeField] private InventoryUI inventoryUI;
    private Inventory inventory;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Awake()
    {

        inventory = new Inventory();
        inventoryUI.SetInventory(inventory);
        inventoryUI.SetPlayer(this);
        //ItemWorld.SpawnItemWorld(new Vector3(-32, -11), new Item { itemType = Item.ItemType.BerrySeed, amount = 1 });
        //ItemWorld.SpawnItemWorld(new Vector3(-29, -14), new Item { itemType = Item.ItemType.CarrotSeed, amount = 1 });
        //ItemWorld.SpawnItemWorld(new Vector3(-31, -10), new Item { itemType = Item.ItemType.GrapeSeed, amount = 1 });
        //ItemWorld.SpawnItemWorld(new Vector3(-28, -13), new Item { itemType = Item.ItemType.CabbageSeed, amount = 1 });
        //ItemWorld.SpawnItemWorld(new Vector3(-30, -12), new Item { itemType = Item.ItemType.PotatoSeed, amount = 1 });
        //ItemWorld.SpawnItemWorld(new Vector3(-27, -15), new Item { itemType = Item.ItemType.RadishSeed, amount = 1 });
        //ItemWorld.SpawnItemWorld(new Vector3(-26, -16), new Item { itemType = Item.ItemType.TomatoSeed, amount = 1 });

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        ItemWorld itemWorld = collider.GetComponent<ItemWorld>();
        if (itemWorld != null)
        {
            inventory.AddItem(itemWorld.GetItem());
            itemWorld.DestroySelf();
        }
    }
    public Vector3 GetPosition()
    {
        return transform.position;
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
        if (Input.GetKeyDown(KeyCode.LeftShift) && movement != Vector2.zero)
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
