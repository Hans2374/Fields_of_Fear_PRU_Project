using UnityEngine;
using System;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    // Singleton instance
    public static CurrencyManager Instance { get; private set; }

    // Event triggered when money changes
    public static event Action<int> OnMoneyChanged;

    // Currency display
    [SerializeField] private TextMeshProUGUI moneyText;

    // Sound effects
    [SerializeField] private AudioManager audioManager;

    // Starting money (adjust based on game balance)
    [SerializeField] private int startingMoney = 50;

    // Current money
    private int _currentMoney;
    public int CurrentMoney
    {
        get => _currentMoney;
        private set
        {
            // Only update if value changed
            if (_currentMoney == value) return;

            int oldValue = _currentMoney;
            _currentMoney = value;

            // Update UI
            UpdateMoneyDisplay();

            // Trigger event
            OnMoneyChanged?.Invoke(_currentMoney);

            // Play sound effect (if money increased)
            if (_currentMoney > oldValue && audioManager != null)
            {
                audioManager.PlaySFX(audioManager.menuButton); // You might want to add a specific money sound
            }
        }
    }

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadMoney(); // Load saved money
        }
        else
        {
            Destroy(gameObject);
        }

        // Find references if not set
        if (audioManager == null)
        {
            audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
        }
    }

    private void Start()
    {
        UpdateMoneyDisplay();
    }

    // Update the UI text
    private void UpdateMoneyDisplay()
    {
        if (moneyText != null)
        {
            moneyText.text = CurrentMoney.ToString();
        }
    }

    // Add money (from harvesting crops)
    public void AddMoney(int amount)
    {
        if (amount <= 0) return;

        CurrentMoney += amount;
        Debug.Log($"Added {amount} money. New total: {CurrentMoney}");
        SaveMoney();
    }

    // Try to spend money (returns true if successful)
    public bool TrySpendMoney(int amount)
    {
        if (amount <= 0) return false;

        if (CurrentMoney >= amount)
        {
            CurrentMoney -= amount;
            Debug.Log($"Spent {amount} money. Remaining: {CurrentMoney}");
            SaveMoney();
            return true;
        }
        else
        {
            Debug.Log("Not enough money!");
            return false;
        }
    }

    // Check if player can afford something
    public bool CanAfford(int amount)
    {
        return CurrentMoney >= amount;
    }

    // Save money to PlayerPrefs
    private void SaveMoney()
    {
        PlayerPrefs.SetInt("PlayerMoney", CurrentMoney);
        PlayerPrefs.Save();
    }

    // Load money from PlayerPrefs
    private void LoadMoney()
    {
        if (PlayerPrefs.HasKey("PlayerMoney"))
        {
            CurrentMoney = PlayerPrefs.GetInt("PlayerMoney");
        }
        else
        {
            CurrentMoney = startingMoney;
        }
    }

    // Reset money (for testing or new game)
    public void ResetMoney()
    {
        CurrentMoney = startingMoney;
        SaveMoney();
    }
}