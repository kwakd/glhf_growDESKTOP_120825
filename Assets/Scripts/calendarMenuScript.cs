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


    // Start is called before the first frame update
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
        //UpdateSellMenuList();

        //userNumToTotalNum.enabled = true;
        //userNumToTotalNum.gameObject.SetActive(true);
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
        userNumToTotalNum.gameObject.SetActive(false);
        calendarLeftArrowButton.gameObject.SetActive(false);
        calendarRightArrowButton.gameObject.SetActive(false);
        //exitMenuButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // var leftButton = calendarLeftArrowButton.gameObject.button;
        // leftButton.clicked += SubNumPage;
    }

    void UpdateCalendarMenuList()
    {
        //Debug.Log("Hello");
        
        // Clear existing items first
        ClearCalendarItems();
        
        GameObject g;
        gameManagerScript gmScript = FindObjectOfType<gameManagerScript>();

        int totalCharacters = gmScript.totalCharList.Count;
        int startIndex = numPage * 28;
        int charactersToShow = Mathf.Min(28, totalCharacters - startIndex);
        //exitMenuButton.gameObject.SetActive(true);

        // Show/hide arrow buttons based on page count
        bool multiplePages = totalCharacters > 28;
        calendarLeftArrowButton.gameObject.SetActive(multiplePages);
        calendarRightArrowButton.gameObject.SetActive(multiplePages);
        userNumToTotalNum.gameObject.SetActive(multiplePages);

        // Only show characters that exist on this page
        for (int i = 0; i < charactersToShow; i++)
        {
            g = Instantiate(calendarTemplateCopy, transform);
            
            int charIndex = i + startIndex;
            
            string tempMonth = gmScript.totalCharList[charIndex].timeNowMonth.ToString();
            string tempDay = gmScript.totalCharList[charIndex].timeNowDay.ToString();
            string tempYear = gmScript.totalCharList[charIndex].timeNowYear.ToString();

            Image dateImage = g.transform.GetChild(0).GetComponent<Image>();
            
            // Set to preserve aspect ratio
            dateImage.preserveAspect = true;

            // Determine which sprite character is using
            if (gmScript.totalCharList[charIndex].firstTierint <= 95)
            {
                // g.transform.GetChild(0).GetComponent<Image>().sprite = baseSprite;
                g.transform.GetChild(0).GetComponent<Image>().color = gmScript.randomColorListArray[gmScript.totalCharList[charIndex].secondTierint];
            }
            //epic
            else if (gmScript.totalCharList[charIndex].firstTierint > 95 && gmScript.totalCharList[charIndex].firstTierint <= 99)
            {
                g.transform.GetChild(0).GetComponent<Image>().sprite = imageListEpic[gmScript.totalCharList[charIndex].secondTierint];
            }
            //legendary
            else if(gmScript.totalCharList[charIndex].firstTierint == 100)
            {
                g.transform.GetChild(0).GetComponent<Image>().sprite = imageListLegendary[gmScript.totalCharList[charIndex].secondTierint];
            }
            
            g.transform.GetChild(1).GetComponent<TMP_Text>().text = tempMonth + "/" + tempDay + "/" + tempYear;
            
            // Make DateImage clickable - it's the child at index 0
            GameObject dateImageObject = g.transform.GetChild(0).gameObject;
            Button itemButton = dateImageObject.GetComponent<Button>();
            
            if (itemButton != null)
            {
                // Capture the index for the closure
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
        // Start from the end and work backwards to avoid index shifting
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
        int maxPages = Mathf.CeilToInt(gmScript.totalCharList.Count / 28f) - 1;
        
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
        //Debug.Log("CLICKED!!! Character Index: " + characterIndex);
        gameManagerScript gmScript = FindObjectOfType<gameManagerScript>();
        globalIndex = charIndex;
        Debug.Log("Character Name: " + gmScript.totalCharList[charIndex].charNameText);
        Debug.Log("Date: " + gmScript.totalCharList[charIndex].timeNowMonth + "/" + 
                gmScript.totalCharList[charIndex].timeNowDay + "/" + 
                gmScript.totalCharList[charIndex].timeNowYear);
        

        // Here you can add code to show a detail panel, etc.
        detailPageMenu.SetActive(true);
        calendarLeftArrowButton.gameObject.SetActive(false);
        calendarRightArrowButton.gameObject.SetActive(false);

        Image detailImage = detailPageMenu.transform.GetChild(0).GetComponent<Image>();
        
        // Set to preserve aspect ratio for detail page
        detailImage.preserveAspect = true;

        // Determine which sprite character is using
        if(gmScript.totalCharList[charIndex].firstTierint <= 95)
        {
            detailPageMenu.transform.GetChild(0).GetComponent<Image>().color = gmScript.randomColorListArray[gmScript.totalCharList[charIndex].secondTierint];
            detailPageMenu.transform.GetChild(0).GetComponent<Image>().sprite = baseSprite;
        }
        //epic
        else if(gmScript.totalCharList[charIndex].firstTierint > 95 && gmScript.totalCharList[charIndex].firstTierint <= 99)
        {
            detailPageMenu.transform.GetChild(0).GetComponent<Image>().sprite = imageListEpic[gmScript.totalCharList[charIndex].secondTierint];
            detailPageMenu.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
        //legendary
        else if(gmScript.totalCharList[charIndex].firstTierint == 100)
        {
            detailPageMenu.transform.GetChild(0).GetComponent<Image>().sprite = imageListLegendary[gmScript.totalCharList[charIndex].secondTierint];
            detailPageMenu.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
        
        // set the date in the details page for character
        string tempMonth = gmScript.totalCharList[charIndex].timeNowMonth.ToString();
        string tempDay = gmScript.totalCharList[charIndex].timeNowDay.ToString();
        string tempYear = gmScript.totalCharList[charIndex].timeNowYear.ToString();
        detailPageMenu.transform.GetChild(1).GetComponent<TMP_Text>().text = tempMonth + "/" + tempDay + "/" + tempYear;

        // have to set up the image one eventually

        // description
        string tempDesc = gmScript.totalCharList[charIndex].charLongText;
        //Debug.Log(tempDesc);
        userInputLongText.text = tempDesc;

        gameManagerScript.Instance.detailMenuToggle = true;

        this.gameObject.SetActive(false);
    }

    public void ReadStringInput(string s)
    {
        userInput = s;
        gameManagerScript.Instance.totalCharList[globalIndex].charLongText = userInput;
        //Debug.Log(globalIndex);
    }
}
