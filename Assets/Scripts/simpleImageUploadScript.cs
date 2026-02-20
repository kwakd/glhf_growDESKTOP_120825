using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SimpleFileBrowser;

public class simpleImageUploadScript : MonoBehaviour
{
    public static simpleImageUploadScript Instance;
    
    public Image userImage; // Reference to DPuserImage
    
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
        // Make the image clickable
        if (userImage != null)
        {
            // Turn off preserve aspect to fill RectTransform
            userImage.preserveAspect = false;
            
            Button imageButton = userImage.GetComponent<Button>();
            if (imageButton == null)
            {
                imageButton = userImage.gameObject.AddComponent<Button>();
            }
            imageButton.onClick.AddListener(OpenFileBrowser);
        }
        
        // Optional: Configure the file browser appearance
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".png", ".jpg", ".jpeg"));
        FileBrowser.SetDefaultFilter(".png");
    }

    public void OpenFileBrowser()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show file browser
        yield return FileBrowser.WaitForLoadDialog(
            FileBrowser.PickMode.Files,
            false,
            null,
            null,
            "Select Image",
            "Load"
        );

        // Check if user selected a file
        if (FileBrowser.Success)
        {
            // Get the selected file path
            string path = FileBrowser.Result[0];
            
            // Load the image
            yield return StartCoroutine(LoadImageFromPath(path));
        }
        else
        {
            Debug.Log("Image selection cancelled.");
        }
    }

    IEnumerator LoadImageFromPath(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("File not found: " + path);
            yield break;
        }

        // Check if it's an image file
        string extension = Path.GetExtension(path).ToLower();
        if (extension != ".png" && extension != ".jpg" && extension != ".jpeg")
        {
            Debug.LogError("Invalid file type. Please use PNG or JPG files.");
            yield break;
        }

        // Read the image file
        byte[] imageData = File.ReadAllBytes(path);
        
        // Create texture from the image data
        Texture2D texture = new Texture2D(2, 2);
        bool loaded = texture.LoadImage(imageData);
        
        if (!loaded)
        {
            Debug.LogError("Failed to load image data");
            yield break;
        }
        
        ApplyTexture(texture);
        SaveImageForCharacter(imageData);
        
        Debug.Log("✓ Image uploaded successfully from: " + path);
        
        yield return null;
    }

    void ApplyTexture(Texture2D texture)
    {
        // Create sprite from texture
        Sprite newSprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );
        
        // Apply to the image
        if (userImage != null)
        {
            userImage.sprite = newSprite;
            userImage.color = Color.white;
            
            // Set Image Type to Filled for maximum coverage
            userImage.type = Image.Type.Simple;
            
            // Turn OFF preserve aspect so it fills the entire RectTransform
            userImage.preserveAspect = false;
            
            // Remove any existing AspectRatioFitter to prevent conflicts
            AspectRatioFitter existingFitter = userImage.GetComponent<AspectRatioFitter>();
            if (existingFitter != null)
            {
                Destroy(existingFitter);
            }
            
            Debug.Log($"Image loaded and filling RectTransform: {texture.width}x{texture.height}");
        }
    }

    void SaveImageForCharacter(byte[] imageData)
    {
        if (calendarMenuScript.Instance == null)
        {
            Debug.LogWarning("Calendar menu script not found!");
            return;
        }
        
        int charIndex = gameManagerScript.Instance.globalIndex;
        
        // Save image data directly to character
        if (charIndex >= 0 && charIndex < gameManagerScript.Instance.totalCharList.Count)
        {
            charAScript character = gameManagerScript.Instance.totalCharList[charIndex];
            character.SetCustomImage(imageData);
            Debug.Log("Image saved to character " + charIndex);
        }
        else
        {
            Debug.LogError("Invalid character index: " + charIndex);
        }
    }

    public void LoadImageForCharacter(int charIndex)
    {
        if (charIndex < 0 || charIndex >= gameManagerScript.Instance.totalCharList.Count)
        {
            Debug.LogError("Invalid character index: " + charIndex);
            return;
        }
        
        charAScript character = gameManagerScript.Instance.totalCharList[charIndex];
        
        if (character.hasCustomImage && character.customImageData != null && character.customImageData.Length > 0)
        {
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(character.customImageData);
            
            ApplyTexture(texture);
            
            Debug.Log("Custom image loaded for character " + charIndex);
        }
        else
        {
            // No custom image saved - show placeholder or leave empty
            if (userImage != null)
            {
                userImage.sprite = null;
                userImage.color = new Color(0.8f, 0.8f, 0.8f, 0.3f); // Light gray placeholder
            }
            Debug.Log("No custom image for character " + charIndex);
        }
    }

    public bool HasSavedImage(int charIndex)
    {
        if (charIndex < 0 || charIndex >= gameManagerScript.Instance.totalCharList.Count)
        {
            return false;
        }
        
        charAScript character = gameManagerScript.Instance.totalCharList[charIndex];
        return character.hasCustomImage;
    }

    public void ClearCurrentImage()
    {
        if (calendarMenuScript.Instance == null) return;
        
        int charIndex = gameManagerScript.Instance.globalIndex;
        
        if (charIndex >= 0 && charIndex < gameManagerScript.Instance.totalCharList.Count)
        {
            charAScript character = gameManagerScript.Instance.totalCharList[charIndex];
            character.SetCustomImage(null);
            
            Debug.Log("Image deleted for character " + charIndex);
        }
        
        if (userImage != null)
        {
            userImage.sprite = null;
            userImage.color = new Color(0.8f, 0.8f, 0.8f, 0.3f);
        }
    }
}