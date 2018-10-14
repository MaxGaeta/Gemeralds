using System.Collections;
using UnityEngine;
using System.IO;

/* *
 * This class handles loading levels from the level editor (or hardcoded levels), cycling through and resetting levels
 * */
public class LevelController : MonoBehaviour {

    public LevelData level;
    public GameLogic logic;
    public PopulateLevel populater;
    public HoverController hover;
    public Tutorial tutor;
    public int currentLevel;

    private int numLevels;
    private string[] levels;

    /* *
     * Loads a level from the level editor json
     * @params filename the name of the file
     * */
    public void LoadLevel(string filename)
    {
        string path = "Assets/Levels/" + filename + ".json";

        //Check if file exists at filepath
        if (File.Exists(path))
        {
            //Extracts file into LevelJson Object
            LevelJson jsonFile = JsonUtility.FromJson<LevelJson>(File.ReadAllText(path));

            level.setSquare(jsonFile.height, jsonFile.width);

            // Game board population
            level.BoardData = new GameObject[level.squaresX, level.squaresY];
            int count = 0;

            for (int j = 0; j < level.squaresY; j++)
            {
                for (int i = 0; i < level.squaresX; i++)
                {
                    switch (jsonFile.gameBoard[count])
                    {
                        case 0:
                            level.BoardData[i, j] = level.Rock;
                            break;
                        case 1:
                            level.BoardData[i, j] = null; //Empty Space
                            break;
                        case 2:
                            level.BoardData[i, j] = level.Gem; //Blue Gem
                            break;
                        case 3:
                            level.BoardData[i, j] = level.Gem; //Green Gem
                            break;
                        case 4:
                            level.BoardData[i, j] = level.Gem; //Red Gem
                            break;
                        case 5:
                            level.BoardData[i, j] = level.Gem; //Yellow Gem
                            break;
                        default:
                            level.BoardData[i, j] = level.Rock;
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

            level.InventoryArray = (Bomb[])bombs.ToArray(typeof(Bomb));
            level.invSize = level.InventoryArray.Length;
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
        string dir = "Assets/Levels/";

        //Get all files in directory
        foreach (string file in System.IO.Directory.GetFiles(dir))
        {
            //Extracts file into LevelJson Object
            LevelJson jsonFile = JsonUtility.FromJson<LevelJson>(File.ReadAllText(dir + file));
            
            level.setSquare(jsonFile.height, jsonFile.width);

            // Game board population
            level.BoardData = new GameObject[level.squaresX, level.squaresY];
            int count = 0;

            for (int j = 0; j < level.squaresY; j++)
            {
                for (int i = 0; i < level.squaresX; i++)
                {
                    switch (jsonFile.gameBoard[count])
                    {
                        case 0:
                            level.BoardData[i, j] = level.Rock;
                            break;
                        case 1:
                            level.BoardData[i, j] = null; //Empty Space
                            break;
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                            level.BoardData[i, j] = level.Gem; //Gem
                            break;
                        default:
                            level.BoardData[i, j] = level.Rock;
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

            level.InventoryArray = (Bomb[]) bombs.ToArray(typeof(Bomb));
            level.invSize = level.InventoryArray.Length;
        }
    }

    /* *
     * Sets destroys the current level and loads a new one
     * @params levelNum the level to load
     * */
    public void SetLevel(int levelNum)
    {
        if (levelNum == 1 || levelNum == 3)
        {
            tutor.StartTutorial(levelNum);
        } 
        else if (tutor.active)
        {
            tutor.EndTutorial();
        }
        currentLevel = levelNum;
        populater.Reset();
        LoadLevel(levels[currentLevel - 1]);
        level.gemCollected = 0;
        populater.Populate();
        logic.setCountText();
        logic.stateReset();
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
        if (tutor.active) tutor.EndTutorial();
        SetLevel(currentLevel);
    }
    
    private void Start()
    {
        //Hardcoded array of level names
        levels = new string[] {"1_lvl", "2_lvl", "3_lvl", "4_lvl", "5_lvl",
        "6_lvl", "7_lvl", "8_lvl", "9_lvl", "10_lvl", "11_lvl"};
        numLevels = levels.Length;

        //Load level 1 when game starts
        currentLevel = 1;
        hover.InstantiateHoverGrid();
        LoadLevel(levels[currentLevel - 1]);
        populater.Populate();
        logic.setCountText();
        tutor.StartTutorial(1);
    }
   
}
