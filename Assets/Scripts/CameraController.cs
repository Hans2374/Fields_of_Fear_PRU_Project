using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform target;
    Vector3 velocity = Vector3.zero;

    [Range(0, 1)]
    public float smoothTime;

    public Vector3 positionOffset;

    [Header("Main Area Limits")]
    public Vector2 mainAreaXLimit;
    public Vector2 mainAreaYLimit;

    [Header("Shop Area Limits")]
    public Vector2 shopAreaXLimit;
    public Vector2 shopAreaYLimit;

    [Header("House Area Limits")]
    public Vector2 houseAreaXLimit;
    public Vector2 houseAreaYLimit;

    // Current active limits
    private Vector2 currentXLimit;
    private Vector2 currentYLimit;

    // Reference to the TransitionManager to detect area changes
    private TransitionManager transitionManager;
    private string currentArea = "Main"; // Default area

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        // Find the TransitionManager
        transitionManager = FindObjectOfType<TransitionManager>();
        if (transitionManager == null)
        {
            Debug.LogError("[CameraController] TransitionManager not found in the scene!");
        }

        // Initialize with Main area limits
        SetAreaLimits("Main");
    }

    private void OnEnable()
    {
        // Subscribe to area change events if needed
        // This is an alternative approach if you implement an event system
    }

    private void Start()
    {
        Debug.Log($"[CameraController] Initialized with target: {target.name}");

        // If the camera is instantiated after the transition, check the current area
        if (transitionManager != null)
        {
            // Get current area from TransitionManager
            SetAreaLimits(transitionManager.GetCurrentArea());
        }
    }

    private void LateUpdate()
    {
        // Update area if changed
        if (transitionManager != null && transitionManager.GetCurrentArea() != currentArea)
        {
            SetAreaLimits(transitionManager.GetCurrentArea());
        }

        // Follow target with current area limits
        if (target != null)
        {
            Vector3 targetPosition = target.position + positionOffset;
            targetPosition = new Vector3(
                Mathf.Clamp(targetPosition.x, currentXLimit.x, currentXLimit.y),
                Mathf.Clamp(targetPosition.y, currentYLimit.x, currentYLimit.y),
                -10
            );
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
        else
        {
            // Try to find the player if target was lost
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("[CameraController] Reacquired player target");
            }
        }
    }

    // Set camera limits based on the current area
    public void SetAreaLimits(string areaName)
    {
        currentArea = areaName;

        switch (areaName)
        {
            case "Main":
                currentXLimit = mainAreaXLimit;
                currentYLimit = mainAreaYLimit;
                Debug.Log("[CameraController] Using Main area camera limits");
                break;

            case "Shop":
                currentXLimit = shopAreaXLimit;
                currentYLimit = shopAreaYLimit;
                Debug.Log("[CameraController] Using Shop area camera limits");
                break;

            case "House":
                currentXLimit = houseAreaXLimit;
                currentYLimit = houseAreaYLimit;
                Debug.Log("[CameraController] Using House area camera limits");
                break;

            default:
                Debug.LogWarning($"[CameraController] Unknown area: {areaName}, using Main area limits as default");
                currentXLimit = mainAreaXLimit;
                currentYLimit = mainAreaYLimit;
                break;
        }

        // Immediately snap to new limits if we're outside of them
        if (target != null)
        {
            // Check if camera is outside new limits and snap it inside if needed
            Vector3 currentPos = transform.position;
            bool needsSnap = false;

            if (currentPos.x < currentXLimit.x || currentPos.x > currentXLimit.y ||
                currentPos.y < currentYLimit.x || currentPos.y > currentYLimit.y)
            {
                needsSnap = true;
            }

            if (needsSnap)
            {
                Vector3 snappedPos = new Vector3(
                    Mathf.Clamp(target.position.x + positionOffset.x, currentXLimit.x, currentXLimit.y),
                    Mathf.Clamp(target.position.y + positionOffset.y, currentYLimit.x, currentYLimit.y),
                    currentPos.z
                );
                transform.position = snappedPos;
                Debug.Log("[CameraController] Camera snapped to new area limits");
            }
        }
    }

    // For debugging - draws the camera bounds in the scene view
    private void OnDrawGizmosSelected()
    {
        // Draw all camera bounds with different colors

        // Main area bounds (yellow)
        Gizmos.color = Color.yellow;
        DrawAreaBounds(mainAreaXLimit, mainAreaYLimit);

        // Shop area bounds (blue)
        Gizmos.color = Color.blue;
        DrawAreaBounds(shopAreaXLimit, shopAreaYLimit);

        // House area bounds (green)
        Gizmos.color = Color.green;
        DrawAreaBounds(houseAreaXLimit, houseAreaYLimit);

        // Current active bounds (white)
        Gizmos.color = Color.white;
        DrawAreaBounds(currentXLimit, currentYLimit);
    }

    private void DrawAreaBounds(Vector2 xLimit, Vector2 yLimit)
    {
        Vector2 center = new Vector2(
            (xLimit.x + xLimit.y) / 2f,
            (yLimit.x + yLimit.y) / 2f
        );
        Vector2 size = new Vector2(
            xLimit.y - xLimit.x,
            yLimit.y - yLimit.x
        );
        Gizmos.DrawWireCube(new Vector3(center.x, center.y, 0), new Vector3(size.x, size.y, 1));
    }
}