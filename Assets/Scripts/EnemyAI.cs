using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class EnemyAI : MonoBehaviour
{
    private float lastRoarTime = 0f;
    [SerializeField] private float roarCooldown = 10f; // Khoảng thời gian tối thiểu giữa 2 lần Roar

    AudioManager audioManager;
    public float moveSpeed = 2f;
    private Vector2 moveDirection;
    private Transform player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private int hitCount = 0;
    private Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    private bool isAttacking = false;
    private bool isFrozen = false;
    private bool isNight = false;
    public float timeBeforeMove = 1f;


    [SerializeField] private WorldTime _worldTime;
    public Vector2 spawnMin = new Vector2(-45, -16);
    public Vector2 spawnMax = new Vector2(5, -17);



    void Awake()
    {

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        _worldTime.WorldTimeChanged += OnWorldTimeChanged;
        gameObject.SetActive(false);

        Transform lifeFolder = GameObject.Find("Life").transform;
        hearts = new Image[2];
        hearts[0] = lifeFolder.Find("heart1").GetComponent<Image>();
        hearts[1] = lifeFolder.Find("heart2").GetComponent<Image>();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        _worldTime.WorldTimeChanged -= OnWorldTimeChanged;
    }

    private void OnWorldTimeChanged(object sender, System.TimeSpan newTime)
    {
        float timePercent = (float)newTime.TotalMinutes % WorldTimeConstants.MinutesInDay / WorldTimeConstants.MinutesInDay;

        bool isNightTime = (timePercent > 0.75f || timePercent < 0.125f); // 18:00 - 3:00
        bool isDaytimeSpawn = (timePercent > 0.375f && timePercent < 0.583f); // 9:00 - 14:00


        if (isNightTime)
        {
            if (!isNight)
            {
                isNight = true;
                TrySpawn(0.6f); // 60% spawn vào ban đêm
            }
        }
        else if (isDaytimeSpawn)
        {
            TrySpawn(0.1f); // 10% spawn vào ban ngày
        }
        else
        {
            isNight = false;
            gameObject.SetActive(false); // Ẩn quái vật ngoài khung giờ spawn
        }
    }


    private void TrySpawn(float spawnChance)
    {
        if (UnityEngine.Random.value < spawnChance) // Xác suất spawn theo tỷ lệ
        {
            float spawnX = UnityEngine.Random.Range(spawnMin.x, spawnMax.x);
            float spawnY = UnityEngine.Random.Range(spawnMin.y, spawnMax.y);
            // Kiểm tra nếu đủ cooldown thì mới Roar
            // if (Time.time - lastRoarTime >= roarCooldown)
            // {
            //     audioManager.PlaySFX(audioManager.monsterRoar);
            //     lastRoarTime = Time.time; // Cập nhật thời gian Roar mới nhất
            // }

            gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy || isFrozen) return;
        if (isFrozen) return; // Nếu bị đóng băng, quái vật không di chuyển

        if (player != null && !isAttacking)
        {
            moveDirection = (player.position - transform.position).normalized;
            if (Vector2.Distance(transform.position, player.position) < 0.5f)
            {
                moveDirection = Vector2.zero;
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(AttackPlayer());
                }

            }
        }

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // Cập nhật animation dựa trên trạng thái
        animator.SetBool("walk", moveDirection.magnitude > 0);
        animator.SetBool("attack", isAttacking);
        animator.SetBool("idle", moveDirection.magnitude == 0 && !isAttacking);

        if (moveDirection.x > 0) spriteRenderer.flipX = true;
        else if (moveDirection.x < 0) spriteRenderer.flipX = false;
    }

    void ChooseRandomDirection()
    {
        if (player == null && !isAttacking && !isFrozen)
        {
            moveDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;

            if (audioManager != null)
            {
                audioManager.StopMusic();
                audioManager.StopSFX();
                audioManager.PlayMusic(audioManager.monsterChase);
                audioManager.PlaySFX(audioManager.monsterRoar);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            animator.SetBool("attack", false);
            ChooseRandomDirection();

            if (audioManager != null)
            {
                audioManager.StopMusic();
                audioManager.StopSFX();
                audioManager.PlayMusic(audioManager.morningSound);
                audioManager.PlaySFX(audioManager.birdSound);
            }

        }
    }

    IEnumerator AttackPlayer()
    {
        if (gameObject.scene.name != SceneManager.GetActiveScene().name)
        {
            yield break;
        }

        if (!gameObject.activeInHierarchy) yield break;


        isAttacking = true;
        animator.SetBool("attack", true);
        animator.SetBool("idle", false);
        animator.SetBool("walk", false);

        yield return new WaitForSeconds(1f);

        if (player != null)
        {
            if (hitCount < hearts.Length)
            {
                audioManager.PlaySFX(audioManager.getHit);
                hearts[hitCount].sprite = emptyHeart;
                hitCount++;
            }

            if (hitCount >= hearts.Length)
            {
                StopChaseMusic();
                SceneManager.LoadScene(3);
                yield break;
            }
        }

        animator.SetBool("attack", false);
        animator.SetBool("idle", true);
        animator.SetBool("walk", false);

        // Nếu đây là lần tấn công đầu tiên, quái vật sẽ đứng yên 5 giây
        if (hitCount == 1)
        {
            isFrozen = true; // Đóng băng quái vật
            yield return new WaitForSeconds(5f);
            isFrozen = false; // Hết đóng băng, quái vật tiếp tục di chuyển
        }

        isAttacking = false;
    }

    void StopChaseMusic()
    {
        if (audioManager != null)
        {
            audioManager.StopSFX();
            audioManager.StopMusic();
        }
    }
}
