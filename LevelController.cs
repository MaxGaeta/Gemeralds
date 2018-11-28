using System.Collections;
using UnityEngine;
using System.IO;

/* *
 * This class handles loading levels from the level editor (or hardcoded levels), cycling through and resetting levels
 * */
public class LevelController : MonoBehaviour
{

    public LevelData level;
    public GameLogic logic;
    public PopulateLevel populater;
    public PopulateLevelSelect menupopulator;
    public HoverController hover;
    public Tutorial tutor;
    public AudioHandler aud;
    public GameObject[] scenePrefabs;

    private int time; //0 = noon, 1 = sunset, 2 = night
    
    public int currentLevel;
    public int numLevels;
    public bool [] clearedLevels;
    public string completedString;
    public int highestLevelCompleted;

    private TextAsset[] jsonTextAssets;

    /* *
     * Loads a level from the level editor json
     * @params filename the name of the file
     * */
    public void LoadLevel(int levelNum)
    {
        level.levelTitle.text = "Level " + levelNum;

        //string path = "Assets/Levels/" + filename + ".json";
        TextAsset jsonTextAsset = jsonTextAssets[levelNum - 1];

        //Check if file exists at filepath
        if (jsonTextAsset != null)
        {
            //Extracts file into LevelJson Object
            LevelJson jsonFile = JsonUtility.FromJson<LevelJson>(jsonTextAsset.text);

            level.setSquare(jsonFile.height, jsonFile.width);

            hover.InstantiateHoverGrid();

            // Game board population
            level.BoardData = new GameObject[level.squaresX, level.squaresY];
            int count = 0;

            for (int j = 0; j < level.squaresY; j++)
            {
                for (int i = 0; i < level.squaresX; i++)
                {
                    switch (jsonFile.gameBoard[count])
                    {
                        case 1:
                            level.BoardData[i, j] = null; //Empty Space
                            break;
                        case 2:
                            level.BoardData[i, j] = level.Gem; //Blue Gem
                            break;
                        case 3:
                            level.BoardData[i, j] = level.DeathRock; //Death Rock
                            break;
                        case 4:
                            level.BoardData[i, j] = level.Sinkhole; //Death Rock
                            break;
                        default:
                            level.BoardData[i, j] = level.Rock; //Default Rock
                            break;
                    }
                    count++;
                }
            }

            //Inventory population
            ArrayList bombs = new ArrayList();
            if (jsonFile.B1x3 > 0) bombs.Add(new Bomb("1x3", jsonFile.B1x3));
            if (jsonFile.B1x5 > 0) bombs.Add(new Bomb("1x5", jsonFile.B1x5));
            if (jsonFile.B1xINF > 0) bombs.Add(new Bomb("infinite_horizontal", jsonFile.B1xINF));
            if (jsonFile.B3x1 > 0) bombs.Add(new Bomb("3x1", jsonFile.B3x1));
            if (jsonFile.B3x3Cross > 0) bombs.Add(new Bomb("3x3_cross", jsonFile.B3x3Cross));
            if (jsonFile.B3x3Square > 0) bombs.Add(new Bomb("3x3_square", jsonFile.B3x3Square));
            if (jsonFile.B5x1 > 0) bombs.Add(new Bomb("5x1", jsonFile.B5x1));
            if (jsonFile.B5x5Cross > 0) bombs.Add(new Bomb("5x5_cross", jsonFile.B5x5Cross));
            if (jsonFile.B5x5Square > 0) bombs.Add(new Bomb("5x5_square", jsonFile.B5x5Square));
            if (jsonFile.BINFx1 > 0) bombs.Add(new Bomb("infinite_vertical", jsonFile.BINFx1));
            if (jsonFile.BINFxINFCross > 0) bombs.Add(new Bomb("infinite_cross", jsonFile.BINFxINFCross));
            if (jsonFile.BUpFirework > 0) bombs.Add(new Bomb("up_fireworks", jsonFile.BUpFirework));
            if (jsonFile.BDownFirework > 0) bombs.Add(new Bomb("down_fireworks", jsonFile.BDownFirework));
            if (jsonFile.BLeftFirework > 0) bombs.Add(new Bomb("left_fireworks", jsonFile.BLeftFirework));
            if (jsonFile.BRightFirework > 0) bombs.Add(new Bomb("right_fireworks", jsonFile.BRightFirework));

            level.InventoryArray = (Bomb[])bombs.ToArray(typeof(Bomb));
            level.invSize = level.InventoryArray.Length;

            level.id = jsonFile.id;

            /*  For level id
            int x = 0;
            int c = 0;

            foreach (int b in jsonFile.gameBoard)
            {
                x += (int) Mathf.Pow(c, b);
                c += 1;
            }
            
            x += jsonFile.width * 12735 + jsonFile.height * 23213;
            
            Debug.Log(levelNum + ": " + x);
            */

        }
        else
        {
            throw new IOException("File Not Found");
        }
    }

    /* *
     * Loads a level from the level editor json
     * @params filename the name of the file
     * */
    public void LoadLevels()
    {
        jsonTextAssets = Resources.LoadAll<TextAsset>("Levels/");
        numLevels = jsonTextAssets.Length;
        clearedLevels = new bool[numLevels + 1];
        highestLevelCompleted = 0;
        completedString = "";
    }


    /* *
     * Sets destroys the current level and loads a new one
     * @params levelNum the level to load
     * */
    public void SetLevel(int levelNum)
    {
        if (levelNum == 1 || levelNum == 2 || levelNum == 13)
        {
            tutor.StartTutorial(levelNum);
        }
        else if (tutor.active)
        {
            tutor.EndTutorial();
        }
        LoggingManager.instance.RecordLevelEnd();
        currentLevel = levelNum;
        populater.Reset();
        LoggingManager.instance.RecordLevelStart(currentLevel, "levelId: " + level.id);
        LoadLevel(currentLevel);
        level.gemCollected = 0;
        populater.Populate();
        logic.stateReset();


        if (currentLevel < 16) //noon
        {
            if (time != 0) updateTime(0);
        }
        else if (currentLevel < 31) //sunset
        {
            if (time != 1) updateTime(1);
        }
        else //night
        {
            if (time != 2) updateTime(2);
        }
    }

    private void updateTime(int newTime)
    {
        time = newTime;
        for (int i = 0; i < scenePrefabs.Length; i++)
            scenePrefabs[i].SetActive(i == time);

        for (int i = 0; i < aud.musicSources.Length; i++)
            if (i == time) aud.musicSources[i].Play();
            else aud.musicSources[i].Stop();
    }

    /* *
     * Loads the next Level
     * */
    public void NextLevel()
    {
        SetLevel(currentLevel = currentLevel == numLevels ? 1 : currentLevel + 1);
    }

    /* *
     * Loads the previous Level
     * */
    public void PrevLevel()
    {
        SetLevel(currentLevel = currentLevel == 1 ? numLevels : currentLevel - 1);
    }

    /* *
     * Resets the current level
     * */
    public void ResetLevel()
    {
        LoggingManager.instance.RecordEvent(2, "reset level " + level.id);
        if (tutor.active) tutor.EndTutorial();

        //Copied from SetLevel, but with logging removed
        if (currentLevel == 1 || currentLevel == 2 || currentLevel == 13)
        {
            tutor.StartTutorial(currentLevel);
        }
        else if (tutor.active)
        {
            tutor.EndTutorial();
        }
        populater.Reset();
        LoadLevel(currentLevel);
        level.gemCollected = 0;
        populater.Populate();
        logic.stateReset();

    }

    public void openLevelSelect(){
        //Debug.Log("Open Level Select");
        menupopulator.Populate();
    }

    private void Start()
    {
        LoadLevels();
        
        //Load level 1 when game starts
        currentLevel = 1;
        LoggingManager.instance.RecordLevelStart(currentLevel, "levelId: " + level.id);
        LoadLevel(currentLevel);
        populater.Populate();
        //logic.setCountText();
        tutor.StartTutorial(1);

        updateTime(0);
    }

}
