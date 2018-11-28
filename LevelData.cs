using UnityEngine;
using UnityEngine.UI;

/**
 * All the relevant data in the current level
 * */
public class LevelData : MonoBehaviour {

    //The matrix of prefabs to spawn at each square at the beginning of each level
    //THIS SHOULD ONLY BE CHANGED WHEN LOADING LEVELS IN LEVEL_CONTROLLER.  CONSIDER THIS READONLY
    //OTHERWISE UNDO WILL BE MORE DIFFICULT TO IMPLEMENT
    //Useful for checking whether an object at a square is level.Rock, etc.
    public GameObject[,] BoardData;

    //The matrix of objects at each position of the game board
    //null refers to free spaces
    public GameObject[,] Objects;

    //The list of objects in the inventory
    public GameObject[] Inventory;

    //TODO: remove this and use GameObject[] Inventory for all bomb data
    public Bomb[] InventoryArray;

    public GameObject[,] HoverObjects;
    public GameObject[,] HoverData;

    //parent gameobject
    public GameObject gb;

    //Prefabs Start Here
    public GameObject Rock;
    public GameObject Gem;
    public GameObject RedGem;
    public GameObject YellowGem;
    public GameObject GreenGem;
    public GameObject Explosion;
    public GameObject ExplosionPoison;
    public GameObject HoverSquare;
    public GameObject DeathRock;
    public GameObject Sinkhole;

    public Sprite hoverSprite1;
    public Sprite hoverSprite2;
    public Sprite hoverSprite3;
    public Sprite hoverSprite4;
    public Sprite hoverSprite5;
    public Sprite hoverSprite6;
    public Sprite hoverSprite7;
    public Sprite hoverSprite8;

    public Text levelTitle;
    public Text brokeGemText;
    public Text deathRockText;
    public Text winText;
    public Text outOfBombText;

    //Level parameters start here
    public int squaresX = 7;
    public int squaresY = 9;

    public int invSize = 2;
    public int bombArrX = 5;
    public int bombArrY = 5;

    public int gemTotal = 0;
    public int gemCollected = 0;

    public float topX = -9.9f;
    public float botX = 9.9f;
    public float leftZ = -16.9f;
    public float rightZ = 2.9f;

    public float inventoryTopX = -12.5f;
    public float inventoryBotX = 12.5f;
    public float inventoryLeftZ = 9.7f;
    public float inventoryRightZ = 23.5f;
    public float inventoryOffset = 6f;
    private float bombStartX = -7f;
    private float bombXScale = 5f;
    public float bombStartMidpoint = -4.5f;

    public int id;

    //Sets the size of the game board
    public void setSquare(int x, int y)
    {
        squaresX = x;
        squaresY = y;
    }

    public Vector4 getBoardDimensions()
    {

        return new Vector4(topX, botX, rightZ, leftZ);

    }

    public Vector4 getInventoryDimensions()
    {

        return new Vector4(inventoryTopX, inventoryBotX, inventoryRightZ, inventoryLeftZ);

    }

    public Vector4 getBombDimensions()
    {

        return new Vector4(bombStartX, bombXScale + bombStartX, inventoryRightZ, inventoryLeftZ);

    }

    public float getInventoryMidpoint()
    {

        return (inventoryRightZ + inventoryLeftZ) / 2;

    }

    public float getBombOffset()
    {

        return inventoryOffset;

    }

    // Gives the Vector 3 location for the space [i,j] on the Board
    public Vector3 getBoardPosition(int i, int j) {
        float height = botX - topX;
        float width = rightZ - leftZ;
        return new Vector3(topX + (height * i + height / 2f) / squaresX, 1f,
                                            leftZ + (width * j + width / 2f) / squaresY);
    }

    public bool inBounds(int x, int y)
    {
        return x >= 0 && x < squaresX &&
              y >= 0 && y < squaresY;
    }

    private int Hash(int x)
    {
        x = ((x >> 16) ^ x) * 0x45d9f3b;
        x = ((x >> 16) ^ x) * 0x45d9f3b;
        x = (x >> 16) ^ x;
        return x;
    }

    //Returns a different colored gem based on the hashed position of the board
    public GameObject GemHash(int x, int y)
    {
        switch (Hash(squaresY * x + y) % 4)
        {
            case 1:
                return GreenGem;
            case 2:
                return RedGem;
            case 3:
                return YellowGem;
        }

        return Gem;
    }

    public GameObject RandomGem()
    {
        switch (Random.Range(0,4) % 4)
        {
            case 1:
                return GreenGem;
            case 2:
                return RedGem;
            case 3:
                return YellowGem;
        }

        return Gem;
    }

    public Sprite RandomHover()
    {
        switch (Random.Range(0, 8) % 8)
        {
            case 1:
                return hoverSprite1;
            case 2:
                return hoverSprite2;
            case 3:
                return hoverSprite3;
            case 4:
                return hoverSprite4;
            case 5:
                return hoverSprite5;
            case 6:
                return hoverSprite6;
            case 7:
                return hoverSprite7;
        }

        return hoverSprite8;
    }
}
