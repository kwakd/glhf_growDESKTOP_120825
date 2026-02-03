using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class calendarMenuScript : MonoBehaviour
{
    public static calendarMenuScript Instance;

    public GameObject calendarTemplateCopy;
    public GameObject calendarLeftArrowButton;
    public GameObject calendarRightArrowButton;
    public GameObject detailPageMenu;
    public TextMeshProUGUI userNumToTotalNum;
    public TMP_InputField  userInputLongText;
    public Sprite baseSprite;
    public List<Sprite> imageListEpic = new List<Sprite>();
    public List<Sprite> imageListLegendary = new List<Sprite>();

    public int userNum = 1;
    public int numPage = 0;
    public int totalNum = 0;
    public int globalIndex;

    private string userInput;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        Button leftButton = calendarLeftArrowButton.GetComponent<Button>();
        Button rightButton = calendarRightArrowButton.GetComponent<Button>();
        
        leftButton.onClick.AddListener(SubNumPage);
        rightButton.onClick.AddListener(AddNumPage);
    }

    void OnEnable()
    {
        userNum = 1;
        numPage = 0;
        UpdateCalendarMenuList();
        closeMenuScript.Instance.ActivateExitImage();
    }

    void OnDisable()
    {
        if(transform.childCount > 1)
        {
            for(int i=1; i<transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        //userNumToTotalNum.gameObject.SetActive(false);
        calendarLeftArrowButton.gameObject.SetActive(false);
        calendarRightArrowButton.gameObject.SetActive(false);
    }

    void Update()
    {
    }

    void UpdateCalendarMenuList()
    {
        ClearCalendarItems();
        
        GameObject g;
        gameManagerScript gmScript = FindObjectOfType<gameManagerScript>();

        int totalCharacters = gmScript.totalCharList.Count;
        int startIndex = numPage * 55;
        int charactersToShow = Mathf.Min(55, totalCharacters - startIndex);

        bool multiplePages = totalCharacters > 55;
        calendarLeftArrowButton.gameObject.SetActive(multiplePages);
        calendarRightArrowButton.gameObject.SetActive(multiplePages);
        //userNumToTotalNum.gameObject.SetActive(multiplePages);

        for (int i = 0; i < charactersToShow; i++)
        {
            g = Instantiate(calendarTemplateCopy, transform);
            
            // Reverse the order - show newest first
            int charIndex = (totalCharacters - 1) - (i + startIndex);
            
            string tempMonth = gmScript.totalCharList[charIndex].timeNowMonth.ToString();
            string tempDay = gmScript.totalCharList[charIndex].timeNowDay.ToString();
            string tempYear = gmScript.totalCharList[charIndex].timeNowYear.ToString();

            Image dateImage = g.transform.GetChild(0).GetComponent<Image>();
            
            // Set to preserve aspect ratio
            dateImage.preserveAspect = true;

            // Determine which sprite character is using
            if (gmScript.totalCharList[charIndex].firstTierint <= 74)
            {
                dateImage.sprite = baseSprite;
                dateImage.color = gmScript.randomColorListArray[gmScript.totalCharList[charIndex].secondTierint];
            }
            //epic
            else if (gmScript.totalCharList[charIndex].firstTierint >= 75 && gmScript.totalCharList[charIndex].firstTierint <= 94)
            {
                dateImage.sprite = imageListEpic[gmScript.totalCharList[charIndex].secondTierint];
                dateImage.color = Color.white;
            }
            //legendary
            else if(gmScript.totalCharList[charIndex].firstTierint >= 95 && gmScript.totalCharList[charIndex].firstTierint <= 99)
            {
                dateImage.sprite = imageListLegendary[gmScript.totalCharList[charIndex].secondTierint];
                dateImage.color = Color.white;
            }
            
            g.transform.GetChild(1).GetComponent<TMP_Text>().text = tempMonth + "/" + tempDay + "/" + tempYear;
            
            GameObject dateImageObject = g.transform.GetChild(0).gameObject;
            Button itemButton = dateImageObject.GetComponent<Button>();
            
            if (itemButton != null)
            {
                int index = charIndex;
                itemButton.onClick.AddListener(() => OnCalendarItemClick(index));
            }
            else
            {
                Debug.LogWarning("No Button component found on DateImage!");
            }
            
            g.gameObject.SetActive(true);
        }
    }

    void ClearCalendarItems()
    {
        for(int i = transform.childCount - 1; i >= 1; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    void SubNumPage()
    {
        if(numPage <= 0)
        {
            Debug.Log("numPage IS AT 0");
        }
        else
        {
            numPage--;
            UpdateCalendarMenuList();
        }
        Debug.Log("numPage = " + numPage);
    }

    void AddNumPage()
    {
        gameManagerScript gmScript = FindObjectOfType<gameManagerScript>();
        int maxPages = Mathf.CeilToInt(gmScript.totalCharList.Count / 55f) - 1;
        
        if(numPage >= maxPages)
        {
            Debug.Log("numPage MAX IS AT " + maxPages);
        }
        else
        {
            numPage++;
            UpdateCalendarMenuList();
        }
        Debug.Log("numPage = " + numPage);
    }

    void OnCalendarItemClick(int charIndex)
    {
        gameManagerScript gmScript = FindObjectOfType<gameManagerScript>();
        globalIndex = charIndex;
        Debug.Log("Character Name: " + gmScript.totalCharList[charIndex].charNameText);
        Debug.Log("Date: " + gmScript.totalCharList[charIndex].timeNowMonth + "/" + 
                gmScript.totalCharList[charIndex].timeNowDay + "/" + 
                gmScript.totalCharList[charIndex].timeNowYear);
        
        detailPageMenu.SetActive(true);
        calendarLeftArrowButton.gameObject.SetActive(false);
        calendarRightArrowButton.gameObject.SetActive(false);

        Image detailImage = detailPageMenu.transform.GetChild(0).GetComponent<Image>();
        
        // Set to preserve aspect ratio for detail page
        detailImage.preserveAspect = true;

        // Determine which sprite character is using
        if(gmScript.totalCharList[charIndex].firstTierint <= 74)
        {
            detailImage.color = gmScript.randomColorListArray[gmScript.totalCharList[charIndex].secondTierint];
            detailImage.sprite = baseSprite;
        }
        //epic
        else if(gmScript.totalCharList[charIndex].firstTierint >= 75 && gmScript.totalCharList[charIndex].firstTierint <= 94)
        {
            detailImage.sprite = imageListEpic[gmScript.totalCharList[charIndex].secondTierint];
            detailImage.color = Color.white;
        }
        //legendary
        else if(gmScript.totalCharList[charIndex].firstTierint >= 95 && gmScript.totalCharList[charIndex].firstTierint <= 99)
        {
            detailImage.sprite = imageListLegendary[gmScript.totalCharList[charIndex].secondTierint];
            detailImage.color = Color.white;
        }
        
        string tempMonth = gmScript.totalCharList[charIndex].timeNowMonth.ToString();
        string tempDay = gmScript.totalCharList[charIndex].timeNowDay.ToString();
        string tempYear = gmScript.totalCharList[charIndex].timeNowYear.ToString();
        detailPageMenu.transform.GetChild(1).GetComponent<TMP_Text>().text = tempMonth + "/" + tempDay + "/" + tempYear;

        string tempDesc = gmScript.totalCharList[charIndex].charLongText;
        userInputLongText.text = tempDesc;

        // Load custom user image if it exists
        if (simpleImageUploadScript.Instance != null)
        {
            simpleImageUploadScript.Instance.LoadImageForCharacter(charIndex);
        }

        gameManagerScript.Instance.detailMenuToggle = true;

        this.gameObject.SetActive(false);
    }

    public void ReadStringInput(string s)
    {
        userInput = s;
        gameManagerScript.Instance.totalCharList[globalIndex].charLongText = userInput;
    }
}