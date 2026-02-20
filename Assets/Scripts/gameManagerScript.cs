using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class gameManagerScript : MonoBehaviour
{
    public static gameManagerScript Instance;
    public List<charAScript> totalCharList = new List<charAScript>();
    public List<RuntimeAnimatorController> animControllerListEpic = new List<RuntimeAnimatorController>();
    public List<RuntimeAnimatorController> animControllerListLegendary = new List<RuntimeAnimatorController>();
    public Color[] randomColorListArray = {Color.red, Color.blue, Color.cyan, Color.gray, Color.green, Color.grey, Color.magenta, Color.red, Color.white, Color.yellow};
    public charAScript testSpawn;

    public bool calendarMenuToggle;
    public bool detailMenuToggle;
    public GameObject calendarMenu;
    public GameObject DeletePage;
    public GameObject detailPageMenu;
    public charAScript selectedCharacter;
    public int globalIndex;

    public Sprite baseSprite;
    public List<Sprite> imageListEpic = new List<Sprite>();
    public List<Sprite> imageListLegendary = new List<Sprite>();

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;

        calendarMenu.SetActive(false);
        calendarMenuToggle = false;
        detailMenuToggle = false;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.E))
        // {
        //     int rangeX = 15;
        //     int rangeY = 7;
        //     Vector2 randomPosition = new Vector2(
        //     Random.Range(-rangeX, rangeX),
        //     Random.Range(-rangeY, rangeY)
        //     );
        //     charAScript tempChar;
        //     tempChar = Instantiate(testSpawn, randomPosition, Quaternion.identity);
        //     totalCharList.Add(tempChar);
        // }

        // if (Input.GetKeyDown(KeyCode.O) && totalCharList.Count != 0)
        // {
        //     //Debug.Log(totalCharList.Count);
        //     totalCharList[totalCharList.Count - 1].GetComponent<charAScript>().DeleteCharacterA();
        // }

        // if (Input.GetKeyDown(KeyCode.W) && !detailMenuToggle)
        // {
        //     ToggleCalendarMenu();
        // }

    }

    public void ToggleCalendarMenu()
    {
        if(!calendarMenuToggle)
        {
            calendarMenu.SetActive(true);
            //normalMenuText.enabled = false;
            //sellMenuText.enabled = true;
            calendarMenuToggle = true;
        }
        else
        {
            calendarMenu.SetActive(false);
            //normalMenuText.enabled = true;
            //sellMenuText.enabled = false;
            calendarMenuToggle = false;
            closeMenuScript.Instance.DeactivateExitButton();
        }
    }

    public void OpenMenuButton()
    {
        calendarMenu.SetActive(true);
        calendarMenuToggle = true;
    }

    public void OpenDeleteCharacterPageButton()
    {
        DeletePage.SetActive(true);
    }
    public void DeleteCharacterButton()
    {
        totalCharList[globalIndex].GetComponent<charAScript>().DeleteCharacterA();
        closeMenuScript.Instance.ExitPage();
    }
}
