using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using Random=UnityEngine.Random;
using Object=UnityEngine.Object;


[System.Serializable]
public class charAScript : MonoBehaviour
{
    public string charNameText;
    public string charLongText;
    public int timeNowMonth;
    public int timeNowDay;
    public int timeNowYear;
    public string timeNowTime;
    public string charParagraphText;
    public string dateAndTimeText;
    public bool shinyCharacter;
    public bool characterFirstSpawn;
    public float characterChoiceTime;
    public int firstTierint;
    public int secondTierint;

    private Rigidbody2D thisCharRB;
    private SpriteRenderer thisCharSR;
    private Animator thisCharAnim;

    private int charStatus;

    private bool charFacingRight = true;
    private bool charDanceAnim1Bool = false;
    private bool charDanceAnim2Bool = false;
    private bool charSpecialAnim1Bool = false;



    void Awake()
    {
        saveSystemScript.characterList.Add(this);

    }

    // Start is called before the first frame update
    void Start()
    {
        thisCharRB = GetComponent<Rigidbody2D>();
        thisCharSR = GetComponent<SpriteRenderer>();
        thisCharAnim = GetComponent<Animator>();

        characterChoiceTime = Random.Range(10, 21);

        if (!characterFirstSpawn)
        {
            AutoGenStatsCharacter();
        }

        SetCharacterController();
    }


    void OnDestroy()
    {
        saveSystemScript.characterList.Remove(this);
    }

    void FixedUpdate()
    {
        if(thisCharRB.velocity.x > 0 && !charFacingRight)
        {
            FlipSprite();
        }
        else if(thisCharRB.velocity.x < 0 && charFacingRight)
        {
            FlipSprite();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // characters descision counter
        if (characterChoiceTime > 0)
        {
            characterChoiceTime -= Time.deltaTime;
        }
        else
        {
            characterChoiceTime = Random.Range(5, 10);
            ChooseMoveDirection();
            CharAnimate();
        }
        
    }

    void ChooseMoveDirection()
    {
        // Choose whether to move sideways or up/down
        float charMoveDistance = Random.Range(0f, 1.6f); //0, 1, 1.5
        charStatus = Random.Range(0, 10); // 0~9
        charDanceAnim1Bool = false;
        charDanceAnim2Bool = false;
        charSpecialAnim1Bool = false;

        // MOVE
        if(charStatus >= 0 && charStatus <= 2)
        {
            int charStatusMovement = Random.Range(0, 8);
            switch(charStatusMovement)
            {
                case 0:
                // character goes RIGHT
                thisCharRB.velocity = new Vector2(transform.localScale.x * charMoveDistance, 0);
                break;         
                case 1:
                    // character goes LEFT  
                    thisCharRB.velocity = new Vector2(transform.localScale.x * charMoveDistance * -1, 0);
                    break;
                case 2:
                    // character goes UP   
                    thisCharRB.velocity = new Vector2(0, transform.localScale.y * charMoveDistance);
                    break;
                case 3:
                    // character goes DOWN
                    thisCharRB.velocity = new Vector2(0, transform.localScale.y * -charMoveDistance);
                    break;
                case 4:
                    // character goes UP-RIGHT
                    thisCharRB.velocity = new Vector2(transform.localScale.x * charMoveDistance, transform.localScale.y * charMoveDistance);
                    break;
                case 5:
                    // character goes UP-LEFT
                    thisCharRB.velocity = new Vector2(transform.localScale.x * charMoveDistance * -1, transform.localScale.y * charMoveDistance);
                    break;  
                case 6:
                    // character goes DOWN-RIGHT
                    thisCharRB.velocity = new Vector2(transform.localScale.x * charMoveDistance, transform.localScale.y * -charMoveDistance);
                    break;  
                case 7:
                    // character goes DOWN-LEFT
                    thisCharRB.velocity = new Vector2(transform.localScale.x * charMoveDistance * -1, transform.localScale.y * -charMoveDistance);
                    break;
                default:
                    break;
            }
        }
        // DONT DO ANYTHING
        else if(charStatus >= 3 && charStatus <= 8)
        {
            thisCharRB.velocity = new Vector2(0, 0);
        }
        // SPECIAL ACTION
        else if(charStatus >= 9 && charStatus <= 9)
        {
            int charStatusSpecial;
            if (firstTierint == 100)
            {
                charStatusSpecial = Random.Range(0, 3);
            }
            else
            {
                charStatusSpecial = Random.Range(0, 2);
            }
            switch (charStatusSpecial)
            {
                case 0:
                    thisCharRB.velocity = new Vector2(0, 0);
                    charDanceAnim1Bool = true;
                    break;
                case 1:
                    thisCharRB.velocity = new Vector2(0, 0);
                    charDanceAnim2Bool = true;
                    break;
                case 2:
                    thisCharRB.velocity = new Vector2(0, 0);
                    charSpecialAnim1Bool = true;
                    break;
                default:
                    break;
            }
        }

        //Debug.Log("x: " + thisCharRB.velocity.x + " || y: " + thisCharRB.velocity.y);

    }

    public void DeleteCharacterA()
    {
        gameManagerScript.Instance.totalCharList.Remove(this);
        Object.Destroy(this.gameObject);
    }

    void CharAnimate()
    {
        thisCharAnim.SetFloat("charMoveMagnitude", thisCharRB.velocity.magnitude);
        thisCharAnim.SetBool("charDanceAnim1", charDanceAnim1Bool);
        thisCharAnim.SetBool("charDanceAnim2", charDanceAnim2Bool);
        // legendary anim
        if (firstTierint == 100)
        {
            thisCharAnim.SetBool("charSpecialAnim1", charSpecialAnim1Bool);
        }
    }

    void FlipSprite()
    {
        charFacingRight = !charFacingRight;
        Vector3 tempScale = transform.localScale;
        tempScale.x *= -1;
        transform.localScale = tempScale;
    }

    void SetCharacterController()
    {
        if (firstTierint <= 95)
        {
            //Debug.Log("Common Tier - HIT - 2");
            thisCharSR.color = gameManagerScript.Instance.randomColorListArray[secondTierint];
        }
        //uncommon
        // else if (firstTierint > 75 && firstTierint <= 78)
        // {

        // }
        //epic
        else if (firstTierint > 95 && firstTierint <= 99)
        {
            //Debug.Log("Epic Tier - HIT - 2 " + secondTierint);
            thisCharAnim.runtimeAnimatorController = gameManagerScript.Instance.animControllerListEpic[secondTierint];

        }
        //legendary
        else if (firstTierint == 100)
        {
            //Debug.Log("Legendary Tier - HIT - 2 " + secondTierint);
            thisCharAnim.runtimeAnimatorController = gameManagerScript.Instance.animControllerListLegendary[secondTierint];
        }
    }

    void AutoGenStatsCharacter()
    {
        if (gameManagerScript.Instance == null)
        {
            Debug.LogWarning("GameManager is not ready yet.");
            return;
        }

        // character name
        int tempNumInt = Random.Range(0, 1001);
        charNameText = "character" + tempNumInt.ToString();

        // character time-made
        timeNowMonth = DateTime.Now.Month;
        timeNowDay = DateTime.Now.Day;
        timeNowYear = DateTime.Now.Year;
        timeNowTime = (DateTime.Now.Hour + ":" + DateTime.Now.Minute).ToString();
        
        charLongText = "this character was made at " + timeNowTime;

        // firstTier Roll - TIER
        // Legendary 1% Epic 4% Uncommon 20% Normal 75%
        firstTierint = Random.Range(0, 101);
        //common (at 95 right now but after implementing uncommon change it to 75)
        if (firstTierint <= 75)
        {
            secondTierint = Random.Range(0, gameManagerScript.Instance.randomColorListArray.Length);
            Debug.Log("Common Tier - HIT");
        }
        //uncommon
        // else if (firstTierint > 75 && firstTierint <= 78)
        // {

        // }
        //epic
        else if (firstTierint > 95 && firstTierint <= 99)
        {
            secondTierint = Random.Range(0, gameManagerScript.Instance.animControllerListEpic.Count);
            Debug.Log("Epic Tier - HIT " + secondTierint);
        }
        //legendary
        else if (firstTierint == 100)
        {
            secondTierint = Random.Range(0, gameManagerScript.Instance.animControllerListLegendary.Count);
            Debug.Log("Legendary Tier - HIT " + secondTierint);
        }

        characterFirstSpawn = true;
    }
}
