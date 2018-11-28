using UnityEngine;
using UnityEngine.UI;

/**
 * This class handles createing the GameObjects on the GameBoard and Inventory using information from LevelData
 * 
 * This class should be used to interact with GameObjects during level creation and destruction
 * This class should not handle any Game Logic
 * */
public class PopulateLevel : MonoBehaviour
{
    public BombController bombC;
    public LevelData level;
    public GameLogic logic;
    public InventoryController inventoryC;
    public GameObject hoverCanvas;

    private GameObject inv;
    private GameObject hov;

    private void Start()
    {
        //Create the GameBoard and Inventory Parent GameObjects for better organization
        level.gb = new GameObject("GameBoard");
        inv = new GameObject("Inventory");
        hov = new GameObject("HoverBoard");
    }

    /**
     * Creates all the objects specified by the level data
     * Makes all objects created children of the Parent GameObject
     * @params BoardData the matrix of objects to place on the GameBoard
     * @params Parent the Parent GameObject
     * @returns the matrix of rocks and gems on the game board
     **/
    private GameObject[,] CreateGameBoard(GameObject[,] BoardData , GameObject Parent)
    {
        //find width and height of the board
        float height = level.botX - level.topX;
        float width = level.rightZ - level.leftZ;

        //The array of Objects on the GameBoard
        GameObject[,] Objs = new GameObject[level.squaresX, level.squaresY];

        level.gemTotal = 0;

        for (int i = 0; i < level.squaresX; i++)
        {
            for (int j = 0; j < level.squaresY; j++)
            {
                //If not a free space, create the object (gem or rock) at the square [i, j]
                if (BoardData[i, j]) 
                {
                    Vector3 pos = new Vector3(level.topX + (height * i + height / 2f) / level.squaresX, 1f,
                                                level.leftZ + (width * j + width / 2f) / level.squaresY);

                    //If gem, create the gem based on hashed value of x and y.  Otherwise creates rock.
                    GameObject Obj;

                    if (BoardData[i, j] == level.Gem)
                    {
                        Obj = Instantiate(level.RandomGem(), pos, Quaternion.Euler(-90f, 0f, Random.Range(0, 3) * 90f)); //random gem colors
                    }
                    else if (BoardData[i, j] == level.Rock)
                    {
                        Obj = Instantiate(BoardData[i, j], pos, Quaternion.Euler(-90f, 0f, Random.Range(0, 3) * 90f));
                    }
                    else if (BoardData[i, j] == level.DeathRock)
                    {
                        Obj = Instantiate(BoardData[i, j], pos, Quaternion.Euler(-90f, 0f, 180f));
                    }
                    else 
                    {
                        //sinkhole
                        Obj = Instantiate(BoardData[i, j], pos, Quaternion.Euler(180f, 0f, 180f));
                    }
                    Obj.name = Obj.name.Substring(0, Obj.name.Length - 7) + "[" + i + ", " + j + "]";
                    Vector3 scale = Obj.transform.localScale;
                    Obj.transform.localScale = new Vector3(scale.x * 6.0f / level.squaresX, scale.y * 7.0f / level.squaresY,  scale.x * 6.0f / level.squaresX);
                    Obj.transform.SetParent(Parent.transform);
                    if (BoardData[i, j] == level.Gem)
                    {
                        level.gemTotal = level.gemTotal + 1;
                    }
                    Objs[i, j] = Obj;
                }
            }
        }

        return Objs;

    }

    /* *
     * Creates the Bomb GameObjects specified by the level data
     * Makes all objects created children of the Parent GameObject
     * @params InventoryData the list of Bombs to place in the inventory
     * @params Parent the Parent GameObject
     * @returns the list of Bomb GameObjects in the inventory
     **/
    private GameObject[] createInvBombs(Bomb[] InventoryData, GameObject Parent)
    {
        GameObject[] invObjects = new GameObject[level.invSize];

        for (int i = 0; i < level.invSize; i++)
        {
            invObjects[i] = bombC.createBomb(Parent, level.bombStartMidpoint + i * level.inventoryOffset);
        }

        inventoryC.setBombUI();

        return invObjects;

    }

    /**
     * Creates all the objects specified by the level data
     * Makes all objects created children of the Parent GameObject
     * @params BoardData the matrix of objects to place on the GameBoard
     * @params Parent the Parent GameObject
     * @returns the matrix of rocks and gems on the game board
     **/
    private GameObject[,] CreateHoverBoard(GameObject[,] BoardData, GameObject Parent)
    {
        //find width and height of the board
        float height = level.botX - level.topX;
        float width = level.rightZ - level.leftZ;

        //The array of Objects on the GameBoard
        GameObject[,] Objs = new GameObject[level.squaresX, level.squaresY];

        for (int i = 0; i < level.squaresX; i++)
        {
            for (int j = 0; j < level.squaresY; j++)
            {
                Vector3 pos = new Vector3(level.topX + (height * i + height / 2f) / level.squaresX, 3.5f,
                                                level.leftZ + (width * j + width / 2f) / level.squaresY);
                GameObject Obj = Instantiate(BoardData[i,j], pos, Quaternion.identity);
                Image img = Obj.GetComponent<Image>();
                img.sprite = level.RandomHover();
                img.preserveAspect = true;
                img.color = Color.clear;
                Obj.name = Obj.name.Substring(0, Obj.name.Length - 7) + "[" + i + ", " + j + "]";
                Obj.transform.rotation = Quaternion.Euler(90, 0, 90);
                Vector3 scale = Obj.transform.localScale;
                Obj.transform.localScale = new Vector3(scale.x * 0.6f / level.squaresX, scale.y * 0.75f / level.squaresY, scale.x * 6.0f / level.squaresX);
                Obj.transform.SetParent(Parent.transform);

                Objs[i, j] = Obj;
            }
        }

        Parent.transform.SetParent(hoverCanvas.transform);

        return Objs;

    }

    /* *
     * Reset the level by destroying all GameObjects in this level and associated parameters
     * */
    public void Reset()
    {
        //Reset GameBoard
        for (int i = 0; i < level.squaresX; i++)
        {
            for (int j = 0; j < level.squaresY; j++)
            {
                Destroy(level.Objects[i, j]);
                Destroy(level.HoverObjects[i, j]);
            }
        }
        level.Objects = null;
        level.HoverObjects = null;

        //Reset Inventory
        foreach (GameObject i in level.Inventory)
        {
            Destroy(i);
        }
        level.Inventory = null;

        //Reset Visuals
        inventoryC.setColorOriginal(inventoryC.bomb1Image, inventoryC.bomb1Count, inventoryC.bomb1X, inventoryC.bomb1Shape);
        inventoryC.setColorOriginal(inventoryC.bomb2Image, inventoryC.bomb2Count, inventoryC.bomb2X, inventoryC.bomb2Shape);
        inventoryC.setColorOriginal(inventoryC.bomb3Image, inventoryC.bomb3Count, inventoryC.bomb3X, inventoryC.bomb3Shape);

        //Reset Gems
        GemAnimation.ResetAnimations();

        //Reset Texts
        level.winText.text = "";
        level.brokeGemText.text = "";
        level.gemCollected = 0;
        level.outOfBombText.text = "";
        
    }

    /**
     * Creates all the objects in the level using LevelData
     * */
    public void Populate()
    {
        //Create each object using coordiates at width and height
        level.Objects = CreateGameBoard(level.BoardData, level.gb);
        level.Inventory = createInvBombs(level.InventoryArray, inv);
        level.HoverObjects = CreateHoverBoard(level.HoverData, hov);
    }
}

