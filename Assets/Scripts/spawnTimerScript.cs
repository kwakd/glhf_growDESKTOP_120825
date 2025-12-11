using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpawnTimerScript : MonoBehaviour
{
    public static SpawnTimerScript Instance;
    
    public Button spawnButton;
    public TextMeshProUGUI buttonText; // Text inside the button
    
    private const string LAST_SPAWN_TIME_KEY = "LastSpawnTime";
    //private const float SPAWN_COOLDOWN_HOURS = 24f;
    // 30 seconds for quick testing
    private const float SPAWN_COOLDOWN_HOURS = 0.00833f;
    private DateTime lastSpawnTime;
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
        LoadLastSpawnTime();
        
        if (spawnButton != null)
        {
            spawnButton.onClick.AddListener(TrySpawnCharacter);
        }
        
        UpdateButtonDisplay();
    }

    void Update()
    {
        UpdateButtonDisplay();
    }

    void LoadLastSpawnTime()
    {
        if (PlayerPrefs.HasKey(LAST_SPAWN_TIME_KEY))
        {
            string lastSpawnString = PlayerPrefs.GetString(LAST_SPAWN_TIME_KEY);
            lastSpawnTime = DateTime.Parse(lastSpawnString);
            
            // Check if 24 hours have passed
            TimeSpan timeSinceLastSpawn = DateTime.Now - lastSpawnTime;
            canSpawn = timeSinceLastSpawn.TotalHours >= SPAWN_COOLDOWN_HOURS;
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
                buttonText.color = Color.white; // or any color you prefer
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
            // Calculate remaining time
            TimeSpan timeSinceLastSpawn = DateTime.Now - lastSpawnTime;
            TimeSpan timeRemaining = TimeSpan.FromHours(SPAWN_COOLDOWN_HOURS) - timeSinceLastSpawn;
            
            if (timeRemaining.TotalSeconds <= 0)
            {
                // Time's up!
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
            Debug.Log("Cannot spawn yet! Wait for timer to finish.");
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
        
        // Save the spawn time
        lastSpawnTime = DateTime.Now;
        PlayerPrefs.SetString(LAST_SPAWN_TIME_KEY, lastSpawnTime.ToString());
        PlayerPrefs.Save();
        
        // Update state
        canSpawn = false;
        UpdateButtonDisplay();
        
        Debug.Log("Character spawned! Next spawn available at: " + lastSpawnTime.AddHours(SPAWN_COOLDOWN_HOURS));
    }

    public bool CanSpawn()
    {
        return canSpawn;
    }

    public TimeSpan GetTimeRemaining()
    {
        if (canSpawn) return TimeSpan.Zero;
        
        TimeSpan timeSinceLastSpawn = DateTime.Now - lastSpawnTime;
        TimeSpan timeRemaining = TimeSpan.FromHours(SPAWN_COOLDOWN_HOURS) - timeSinceLastSpawn;
        
        return timeRemaining.TotalSeconds > 0 ? timeRemaining : TimeSpan.Zero;
    }

    // Optional: Reset timer for testing
    public void ResetTimer()
    {
        PlayerPrefs.DeleteKey(LAST_SPAWN_TIME_KEY);
        PlayerPrefs.Save();
        canSpawn = true;
        UpdateButtonDisplay();
        Debug.Log("Timer reset! You can spawn again.");
    }
}