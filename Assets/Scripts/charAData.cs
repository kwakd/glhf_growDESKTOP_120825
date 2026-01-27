using UnityEngine;

[System.Serializable]
public class charAData
{
    public string charNameText;
    public string charLongText;

    public bool characterFirstSpawn;
    public int firstTierint;
    public int secondTierint;

    public int timeNowMonth;
    public int timeNowDay;
    public int timeNowYear;
    public string timeNowTime;

    public byte[] customImageData;
    public bool hasCustomImage;

    public charAData(charAScript charA)
    {
        charNameText = charA.charNameText;
        charLongText = charA.charLongText;
        characterFirstSpawn = charA.characterFirstSpawn;
        firstTierint = charA.firstTierint;
        secondTierint = charA.secondTierint;
        timeNowMonth = charA.timeNowMonth;
        timeNowDay = charA.timeNowDay;
        timeNowYear = charA.timeNowYear;
        timeNowTime = charA.timeNowTime;
        customImageData = charA.customImageData;
        hasCustomImage = charA.hasCustomImage;
    }
}
