using System.Collections;
using UnityEngine;

public class SmoothFade : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine fadeCoroutine;

    void Start()
    {
        // Get the parent's SpriteRenderer instead of the child's
        spriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartFading(0.3f); // Adjust transparency level here
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartFading(1f); // Restore full opacity
        }
    }

    void StartFading(float targetAlpha)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeTo(targetAlpha));
    }

    IEnumerator FadeTo(float targetAlpha)
    {
        float duration = 0.5f; // Fade speed
        float elapsedTime = 0f;
        Color color = spriteRenderer.color;

        while (elapsedTime < duration)
        {
            color.a = Mathf.Lerp(color.a, targetAlpha, elapsedTime / duration);
            spriteRenderer.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = targetAlpha;
        spriteRenderer.color = color;
    }
}
