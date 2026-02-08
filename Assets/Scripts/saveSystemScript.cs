using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System.IO;

[System.Serializable]
public class saveSystemScript : MonoBehaviour
{
    public gameManagerScript gameManager;

    [SerializeField] charAScript charPrefab;
    public static List<charAScript> characterList = new List<charAScript>();
    
    const string CHAR_SUB = "/char";
    const string CHAR_COUNT_SUB = "/char.count";

    void Awake()
    {
        LoadCharacter();
    }

    void OnApplicationQuit()
    {
        SaveCharacter();
    }
    void SaveCharacter()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + CHAR_SUB + SceneManager.GetActiveScene().buildIndex;
        string countPath = Application.persistentDataPath + CHAR_COUNT_SUB + SceneManager.GetActiveScene().buildIndex;

        FileStream countStream = new FileStream(countPath, FileMode.Create);

        formatter.Serialize(countStream, characterList.Count);
        countStream.Close();

        for (int i = 0; i < characterList.Count; i++)
        {
            FileStream stream = new FileStream(path + i, FileMode.Create);
            charAData data = new charAData(characterList[i]);

            // DEBUG: Check if image data exists
            if (data.hasCustomImage && data.customImageData != null)
            {
                Debug.Log($"Saving character {i} WITH custom image. Size: {data.customImageData.Length} bytes");
            }
            else
            {
                Debug.Log($"Saving character {i} WITHOUT custom image");
            }

            formatter.Serialize(stream, data);
            stream.Close();
        }
        
    }

    void LoadCharacter()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + CHAR_SUB + SceneManager.GetActiveScene().buildIndex;
        string countPath = Application.persistentDataPath + CHAR_COUNT_SUB + SceneManager.GetActiveScene().buildIndex;
        int characterCount = 0;

        if (File.Exists(countPath))
        {
            FileStream countStream = new FileStream(countPath, FileMode.Open);

            characterCount = (int)formatter.Deserialize(countStream);
            countStream.Close();
        }
        else
        {
            Debug.LogError("Path not found in " + countPath);
        }

        for (int i = 0; i < characterCount; i++)
        {
            if (File.Exists(path + i))
            {
                FileStream stream = new FileStream(path + i, FileMode.Open);
                charAData data = formatter.Deserialize(stream) as charAData;

                stream.Close();

                // spawns then randomly.
                int rangeX = 15;
                int rangeY = 7;
                Vector3 randomPosition = new Vector3(
                Random.Range(-rangeX, rangeX),
                Random.Range(-rangeY, rangeY),
                0
                );

                charAScript characterA = Instantiate(charPrefab, randomPosition, Quaternion.identity);

                //adds character proper data
                characterA.charNameText = data.charNameText;
                characterA.charLongText = data.charLongText;
                characterA.characterFirstSpawn = data.characterFirstSpawn;
                characterA.firstTierint = data.firstTierint;
                characterA.secondTierint = data.secondTierint;
                characterA.timeNowMonth = data.timeNowMonth;
                characterA.timeNowDay = data.timeNowDay;
                characterA.timeNowYear = data.timeNowYear;
                characterA.timeNowTime = data.timeNowTime;

                // Load custom image data
                characterA.customImageData = data.customImageData;
                characterA.hasCustomImage = data.hasCustomImage;


                //adds character to the in-game list
                gameManager.totalCharList.Add(characterA);
            }
            else
            {
                Debug.LogError("Path not found in " + path + i);
            }
        }
        Debug.Log("Characters loaded! Total: " + characterCount);
    }
}
