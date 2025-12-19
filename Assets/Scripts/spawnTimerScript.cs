using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpawnTimerScript : MonoBehaviour
{
    public static SpawnTimerScript Instance;
    
    public Button spawnButton;
    public TextMeshProUGUI buttonText; // Text inside the button
    
    private const string LAST_SPAWN_DATE_KEY = "LastSpawnDate";
    private DateTime lastSpawnDate;
    private bool canSpawn = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadLastSpawnDate();
        
        if (spawnButton != null)
        {
            spawnButton.onClick.AddListener(TrySpawnCharacter);
        }
        
        UpdateButtonDisplay();
    }

    void Update()
    {
        UpdateButtonDisplay();
        
        // Press Q to reset timer (for testing/debugging)
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ResetTimer();
        }
    }

    void LoadLastSpawnDate()
    {
        if (PlayerPrefs.HasKey(LAST_SPAWN_DATE_KEY))
        {
            string lastSpawnString = PlayerPrefs.GetString(LAST_SPAWN_DATE_KEY);
            lastSpawnDate = DateTime.Parse(lastSpawnString);
            
            // Check if it's a new day (past midnight)
            DateTime today = DateTime.Today; // Gets today's date at 00:00:00
            canSpawn = lastSpawnDate.Date < today;
        }
        else
        {
            // First time playing - allow spawn
            canSpawn = true;
        }
    }

    void UpdateButtonDisplay()
    {
        if (canSpawn)
        {
            // Can spawn - show "Spawn Character" text
            if (buttonText != null)
            {
                buttonText.text = "Spawn a 'moment'";
                buttonText.color = Color.white;
            }
            
            if (spawnButton != null)
            {
                spawnButton.interactable = true;
                // Optional: Change button color to green when ready
                ColorBlock colors = spawnButton.colors;
                colors.normalColor = Color.green;
                colors.highlightedColor = new Color(0.6f, 1f, 0.6f); // lighter green
                colors.pressedColor = new Color(0.3f, 0.8f, 0.3f); // darker green
                spawnButton.colors = colors;
            }
        }
        else
        {
            // Calculate time until midnight
            DateTime now = DateTime.Now;
            DateTime nextMidnight = DateTime.Today.AddDays(1); // Tomorrow at 00:00:00
            TimeSpan timeRemaining = nextMidnight - now;
            
            if (timeRemaining.TotalSeconds <= 0)
            {
                // It's past midnight!
                canSpawn = true;
                UpdateButtonDisplay();
                return;
            }
            
            // Display countdown on button
            if (buttonText != null)
            {
                string timeString = FormatTimeRemaining(timeRemaining);
                buttonText.text = timeString;
                buttonText.color = Color.white;
            }
            
            if (spawnButton != null)
            {
                spawnButton.interactable = false;
                // Optional: Change button color to gray when disabled
                ColorBlock colors = spawnButton.colors;
                colors.disabledColor = Color.gray;
                spawnButton.colors = colors;
            }
        }
    }

    string FormatTimeRemaining(TimeSpan timeRemaining)
    {
        // Format: 23h 59m 59s
        if (timeRemaining.TotalHours >= 1)
        {
            return string.Format("{0:D2}h {1:D2}m {2:D2}s", 
                timeRemaining.Hours, 
                timeRemaining.Minutes, 
                timeRemaining.Seconds);
        }
        // Format: 59m 59s (when less than 1 hour)
        else if (timeRemaining.TotalMinutes >= 1)
        {
            return string.Format("{0:D2}m {1:D2}s", 
                timeRemaining.Minutes, 
                timeRemaining.Seconds);
        }
        // Format: 59s (when less than 1 minute)
        else
        {
            return string.Format("{0:D2}s", timeRemaining.Seconds);
        }
    }

    public void TrySpawnCharacter()
    {
        if (canSpawn)
        {
            SpawnCharacter();
        }
        else
        {
            Debug.Log("Cannot spawn yet! Wait until midnight.");
        }
    }

    void SpawnCharacter()
    {
        Debug.Log("Spawning character!");
        
        // Spawn the character (your existing spawn code)
        gameManagerScript gmScript = FindObjectOfType<gameManagerScript>();
        if (gmScript != null)
        {
            int rangeX = 2;
            int rangeY = 5;
            Vector2 randomPosition = new Vector2(
                UnityEngine.Random.Range(-rangeX, rangeX),
                UnityEngine.Random.Range(-rangeY, rangeY)
            );
            
            charAScript tempChar = Instantiate(gmScript.testSpawn, randomPosition, Quaternion.identity);
            gmScript.totalCharList.Add(tempChar);
        }
        
        // Save the spawn date
        lastSpawnDate = DateTime.Now;
        PlayerPrefs.SetString(LAST_SPAWN_DATE_KEY, lastSpawnDate.ToString());
        PlayerPrefs.Save();
        
        // Update state
        canSpawn = false;
        UpdateButtonDisplay();
        
        DateTime nextMidnight = DateTime.Today.AddDays(1);
        Debug.Log("Character spawned! Next spawn available at midnight: " + nextMidnight);
    }

    public bool CanSpawn()
    {
        return canSpawn;
    }

    public TimeSpan GetTimeRemaining()
    {
        if (canSpawn) return TimeSpan.Zero;
        
        DateTime now = DateTime.Now;
        DateTime nextMidnight = DateTime.Today.AddDays(1);
        TimeSpan timeRemaining = nextMidnight - now;
        
        return timeRemaining.TotalSeconds > 0 ? timeRemaining : TimeSpan.Zero;
    }

    // Optional: Reset timer for testing
    public void ResetTimer()
    {
        PlayerPrefs.DeleteKey(LAST_SPAWN_DATE_KEY);
        PlayerPrefs.Save();
        canSpawn = true;
        UpdateButtonDisplay();
        Debug.Log("Timer reset! You can spawn again.");
    }
}