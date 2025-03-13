using System.Collections;
using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private GameObject moneyIcon;

    // Animation settings
    [SerializeField] private float animDuration = 0.5f;
    [SerializeField] private float bounceHeight = 20f;

    // Previous amount for change detection
    private int previousAmount = 0;
    private bool isInitialized = false;

    private void Start()
    {
        // Find currency manager and subscribe to events
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.OnMoneyChanged += OnMoneyChanged;

            // Update display with initial amount
            UpdateDisplay(CurrencyManager.Instance.CurrentMoney);
            previousAmount = CurrencyManager.Instance.CurrentMoney;
            isInitialized = true;
        }
        else
        {
            Debug.LogWarning("CurrencyUI: CurrencyManager instance not found!");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.OnMoneyChanged -= OnMoneyChanged;
        }
    }

    // Called when money amount changes
    private void OnMoneyChanged(int newAmount)
    {
        // Skip change animation on initialization
        if (!isInitialized)
        {
            previousAmount = newAmount;
            UpdateDisplay(newAmount);
            return;
        }

        // Get the difference
        int difference = newAmount - previousAmount;

        // Update display
        UpdateDisplay(newAmount);

        // Play animation if there's a significant change
        if (Mathf.Abs(difference) > 0)
        {
            PlayChangeAnimation(difference);
        }

        // Update previous amount
        previousAmount = newAmount;
    }

    // Update the currency display
    private void UpdateDisplay(int amount)
    {
        if (currencyText != null)
        {
            currencyText.text = amount.ToString();
        }
    }

    // Play animation when currency changes
    private void PlayChangeAnimation(int difference)
    {
        // Bounce the icon
        if (moneyIcon != null)
        {
            StartCoroutine(BounceAnimation(moneyIcon));
        }

        // Show floating text for the change
        SpawnFloatingText(difference);
    }

    // Spawn floating text showing the change amount
    private void SpawnFloatingText(int amount)
    {
        if (floatingTextPrefab == null || currencyText == null) return;

        // Create floating text
        GameObject floatingObj = Instantiate(floatingTextPrefab, currencyText.transform.position, Quaternion.identity, transform);
        TextMeshProUGUI floatingText = floatingObj.GetComponent<TextMeshProUGUI>();

        if (floatingText != null)
        {
            // Set text with sign
            string sign = amount > 0 ? "+" : "";
            floatingText.text = sign + amount.ToString();

            // Set color based on gain/loss
            floatingText.color = amount > 0 ? Color.green : Color.red;

            // Animate and destroy
            StartCoroutine(AnimateFloatingText(floatingObj, floatingText));
        }
        else
        {
            Destroy(floatingObj);
        }
    }

    // Animate the floating text
    private IEnumerator AnimateFloatingText(GameObject textObj, TextMeshProUGUI text)
    {
        // Initial position
        RectTransform rect = textObj.GetComponent<RectTransform>();
        if (rect == null)
        {
            Destroy(textObj);
            yield break;
        }

        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, 50f); // Float upward

        // Animation duration
        float duration = 1.5f;
        float elapsed = 0f;

        // Animate position and alpha
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Move up
            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            // Fade out near the end
            if (t > 0.5f)
            {
                text.alpha = 1f - ((t - 0.5f) * 2f);
            }

            yield return null;
        }

        // Destroy when done
        Destroy(textObj);
    }

    // Bounce animation for the money icon
    private IEnumerator BounceAnimation(GameObject target)
    {
        RectTransform rect = target.GetComponent<RectTransform>();
        if (rect == null) yield break;

        Vector2 startPos = rect.anchoredPosition;
        Vector2 topPos = startPos + new Vector2(0, bounceHeight);

        // Bounce up
        float halfDuration = animDuration / 2f;
        float elapsed = 0f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            rect.anchoredPosition = Vector2.Lerp(startPos, topPos, t);
            yield return null;
        }

        // Bounce down
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            rect.anchoredPosition = Vector2.Lerp(topPos, startPos, t);
            yield return null;
        }

        // Ensure we end exactly at the start position
        rect.anchoredPosition = startPos;
    }
}