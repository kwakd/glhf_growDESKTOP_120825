using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class closeMenuScript : MonoBehaviour
{
    public static closeMenuScript Instance;

    public GameObject detailMenuPageClose;
    public GameObject calendarMenuPageClose;
    public GameObject exitMenuButton;
    public Image exitImage;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Button exitMenuButtonStart = exitMenuButton.GetComponent<Button>();
        exitMenuButtonStart.onClick.AddListener(ExitPage);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CloseBothWindows()
    {
        
    }

    public void ExitPage()
    {
        Debug.Log("Exit Page button Pressed");
        detailMenuPageClose.gameObject.SetActive(false);
        calendarMenuPageClose.gameObject.SetActive(false);
        gameManagerScript.Instance.detailMenuToggle = false;
        gameManagerScript.Instance.calendarMenuToggle = false;
        DeactivateExitButton();
    }

    public void ActivateExitButton()
    {
        exitMenuButton.gameObject.SetActive(true);
    }
    public void ActivateExitImage()
    {
        exitImage.enabled = true;
    }
    public void DeactivateExitButton()
    {
        exitImage.enabled = false;
    }
}
