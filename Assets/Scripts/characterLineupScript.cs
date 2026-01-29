using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterLineupScript : MonoBehaviour
{
    [Header("Lineup Settings")]
    public int maxCharactersToDisplay = 55;
    public int charactersPerRow = 11; // 55 / 5 rows = 11 per row
    public float spacingX = 1.5f;
    public float spacingY = 1.5f;
    public Vector2 startPosition = new Vector2(-8f, 3f);
    
    private bool isLinedUp = false;
    private Dictionary<charAScript, Vector3> originalPositions = new Dictionary<charAScript, Vector3>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleLineup();
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
        
        // Get the characters (reversed order - newest first)
        List<charAScript> charactersToLine = new List<charAScript>();
        int totalChars = Mathf.Min(maxCharactersToDisplay, gmScript.totalCharList.Count);
        
        for (int i = 0; i < totalChars; i++)
        {
            // Reverse order: start from the end of the list (newest characters)
            int index = gmScript.totalCharList.Count - 1 - i;
            charactersToLine.Add(gmScript.totalCharList[index]);
        }
        
        // Line them up in a grid
        for (int i = 0; i < charactersToLine.Count; i++)
        {
            charAScript character = charactersToLine[i];
            
            // Save original position
            originalPositions[character] = character.transform.position;
            
            // Calculate grid position
            int row = i / charactersPerRow;
            int col = i % charactersPerRow;
            
            Vector3 newPosition = new Vector3(
                startPosition.x + (col * spacingX),
                startPosition.y - (row * spacingY),
                0
            );
            
            // Move character to new position
            character.transform.position = newPosition;
            
            // Stop character movement
            Rigidbody2D rb = character.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            }
        }
        
        Debug.Log($"Lined up {charactersToLine.Count} characters (reversed order - newest first)");
    }

    void RestorePositions()
    {
        foreach (var kvp in originalPositions)
        {
            charAScript character = kvp.Key;
            Vector3 originalPos = kvp.Value;
            
            if (character != null)
            {
                character.transform.position = originalPos;
            }
        }
        
        originalPositions.Clear();
        Debug.Log("Restored character positions");
    }
}