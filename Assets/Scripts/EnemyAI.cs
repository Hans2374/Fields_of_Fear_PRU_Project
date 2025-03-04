using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class EnemyAI : MonoBehaviour
{
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
    private bool isFrozen = false; // Quái vật bị đóng băng sau lần tấn công đầu tiên
    private bool isNight = false; // Kiểm tra ban đêm

    public Image fadeImage; // Đặt một UI Image full màn hình màu đen, alpha = 0
    public float fadeDuration = 3f; // Thời gian hiệu ứng mờ dần

    [SerializeField] private WorldTime _worldTime;
    public Vector2 spawnMin = new Vector2(-45, -16);
    public Vector2 spawnMax = new Vector2(5, -17);


    
    void Awake()
    {

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        _worldTime.WorldTimeChanged += OnWorldTimeChanged;
        gameObject.SetActive(false); // Quái vật bị ẩn khi bắt đầu

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
        Debug.Log($"[Enemy] Time: {newTime}, Percent: {timePercent}, Night: {isNightTime}, Day: {isDaytimeSpawn}");

        if (isNightTime)
        {
            if (!isNight)
            {
                isNight = true;
                TrySpawn(0.8f); // 80% spawn vào ban đêm
            }
        }
        else if (isDaytimeSpawn)
        {
            TrySpawn(0.2f); // 20% spawn vào ban ngày
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


            gameObject.SetActive(true);
            audioManager.PlaySFX(audioManager.monsterRoar);
        }
    }

    void Update()
    {
        if (isFrozen) return; // 🔥 Nếu bị đóng băng, quái vật không di chuyển

        if (player != null && !isAttacking)
        {
            moveDirection = (player.position - transform.position).normalized;
            if (Vector2.Distance(transform.position, player.position) < 0.5f)
            {
                moveDirection = Vector2.zero;
                StartCoroutine(AttackPlayer());
            }
        }

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // 🔥 Cập nhật animation dựa trên trạng thái
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

            // 🔊 Phát nhạc rượt đuổi
            if (audioManager != null)
            {
                audioManager.PlaySFX(audioManager.monsterChase);
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

            // 🔇 Ngừng nhạc chase, có thể bật lại nhạc nền nếu muốn
            if (audioManager != null)
            {
                audioManager.StopSFX();
            }
        }
    }

    IEnumerator AttackPlayer()
    {
        if (gameObject.scene.name != SceneManager.GetActiveScene().name)
        {
            yield break;
        }


        isAttacking = true;
        animator.SetBool("attack", true);
        animator.SetBool("idle", false);
        animator.SetBool("walk", false);

        yield return new WaitForSeconds(1f);

        if (player != null)
        {
            if (hitCount < hearts.Length)
            {
                hearts[hitCount].sprite = emptyHeart;
                audioManager.PlaySFX(audioManager.getHit);
                hitCount++;
            }

            if (hitCount >= hearts.Length)
            {
                StopChaseMusic(); // 🔥 Dừng nhạc trước khi Load Scene
                SceneManager.LoadScene(3);
                yield break;
            }
        }

        animator.SetBool("attack", false);
        animator.SetBool("idle", true);
        animator.SetBool("walk", false);

        // 🔥 Nếu đây là lần tấn công đầu tiên, quái vật sẽ đứng yên 5 giây
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
            audioManager.StopSFX(); // 🔥 Dừng nhạc SFX chase
        }
    }


}
