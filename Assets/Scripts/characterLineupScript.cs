using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLineupScript : MonoBehaviour
{
    [Header("Lineup Settings")]
    public int maxCharactersToDisplay = 55;
    public int charactersPerRow = 11; // 55 / 5 rows = 11 per row
    public float spacingX = 1.5f;
    public float spacingY = 1.5f;
    public Vector2 startPosition = new Vector2(-8f, 3f);
    
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float arrivalThreshold = 0.1f;
    
    private bool isLinedUp = false;
    private bool isMoving = false;
    private Dictionary<charAScript, Vector3> originalPositions = new Dictionary<charAScript, Vector3>();
    private Dictionary<charAScript, Vector3> targetPositions = new Dictionary<charAScript, Vector3>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && !isMoving)
        {
            ToggleLineup();
        }
        
        // Smoothly move characters to their target positions
        if (isMoving)
        {
            MoveCharactersToTargets();
        }
    }

    void ToggleLineup()
    {
        if (!isLinedUp)
        {
            LineUpCharacters();
        }
        else
        {
            RestorePositions();
        }
        
        isLinedUp = !isLinedUp;
    }

    void LineUpCharacters()
    {
        gameManagerScript gmScript = gameManagerScript.Instance;
        
        if (gmScript == null || gmScript.totalCharList == null)
        {
            Debug.LogWarning("GameManager or character list not found!");
            return;
        }

        // Clear previous positions
        originalPositions.Clear();
        targetPositions.Clear();
        
        // Get the characters (reversed order - newest first)
        List<charAScript> charactersToLine = new List<charAScript>();
        int totalChars = Mathf.Min(maxCharactersToDisplay, gmScript.totalCharList.Count);
        
        for (int i = 0; i < totalChars; i++)
        {
            // Reverse order: start from the end of the list (newest characters)
            int index = gmScript.totalCharList.Count - 1 - i;
            charactersToLine.Add(gmScript.totalCharList[index]);
        }
        
        // Set up target positions for grid
        for (int i = 0; i < charactersToLine.Count; i++)
        {
            charAScript character = charactersToLine[i];
            
            // Save original position
            originalPositions[character] = character.transform.position;
            
            // Calculate grid position
            int row = i / charactersPerRow;
            int col = i % charactersPerRow;
            
            Vector3 targetPosition = new Vector3(
                startPosition.x + (col * spacingX),
                startPosition.y - (row * spacingY),
                0
            );
            
            // Store target position
            targetPositions[character] = targetPosition;
            
            // Disable collisions while moving
            Collider2D collider = character.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            
            // Stop character AI movement
            Rigidbody2D rb = character.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            }
        }
        
        isMoving = true;
        Debug.Log($"Moving {charactersToLine.Count} characters to lineup (reversed order - newest first)");
    }

    void RestorePositions()
    {
        // Set original positions as targets
        targetPositions.Clear();
        
        foreach (var kvp in originalPositions)
        {
            charAScript character = kvp.Key;
            Vector3 originalPos = kvp.Value;
            
            if (character != null)
            {
                targetPositions[character] = originalPos;
                
                // Disable collisions while moving back
                Collider2D collider = character.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = false;
                }
            }
        }
        
        isMoving = true;
        Debug.Log("Moving characters back to original positions");
    }

    void MoveCharactersToTargets()
    {
        bool allArrived = true;
        
        foreach (var kvp in targetPositions)
        {
            charAScript character = kvp.Key;
            Vector3 target = kvp.Value;
            
            if (character == null) continue;
            
            // Calculate distance to target
            float distance = Vector3.Distance(character.transform.position, target);
            
            if (distance > arrivalThreshold)
            {
                // Move towards target
                character.transform.position = Vector3.MoveTowards(
                    character.transform.position,
                    target,
                    moveSpeed * Time.deltaTime
                );
                
                allArrived = false;
            }
            else
            {
                // Snap to exact position when close enough
                character.transform.position = target;
            }
        }
        
        // When all characters have arrived
        if (allArrived)
        {
            isMoving = false;
            
            // Re-enable collisions after movement is complete
            foreach (var kvp in targetPositions)
            {
                charAScript character = kvp.Key;
                if (character != null)
                {
                    Collider2D collider = character.GetComponent<Collider2D>();
                    if (collider != null)
                    {
                        collider.enabled = true;
                    }
                }
            }
            
            if (!isLinedUp)
            {
                // Characters returned to original positions
                originalPositions.Clear();
            }
            
            targetPositions.Clear();
            Debug.Log("All characters arrived at their positions");
        }
    }
}