using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class InventoryController : MonoBehaviour
{

    public LevelData level;
    public GameLogic logic;

    //Stand-in images
    public Image bomb1Image;
    public Text bomb1Count;
    public Text bomb1X;
    public Image bomb1Shape;

    public Image bomb2Image;
    public Text bomb2Count;
    public Text bomb2X;
    public Image bomb2Shape;

    public Image bomb3Image;
    public Text bomb3Count;
    public Text bomb3X;
    public Image bomb3Shape;

    private Color defaultColor = new Color(0,0,0,1f);
    private Color unusableColor = new Color(0, 0, 0, 0.1f);
    private Color selectColor = Color.white;

    //Illustrations for the bombs
    public Sprite p1by5;
    public Sprite p5by1;
    public Sprite p1by3;
    public Sprite p3by1;
    public Sprite p1byinf;
    public Sprite pinfby1;
    public Sprite p5by5cross;
    public Sprite pinfbyinfcross;
    public Sprite pfireworksup;
    public Sprite pfireworksdown;
    public Sprite pfireworksleft;
    public Sprite pfireworksright;

    // Shapes for bombs
    public Sprite s1by5;
    public Sprite s5by1;
    public Sprite s1by3;
    public Sprite s3by1;
    public Sprite s1byinf;
    public Sprite sinfby1;
    public Sprite s5by5cross;
    public Sprite sinfbyinfcross;
    public Sprite sfireworksup;
    public Sprite sfireworksdown;
    public Sprite sfireworksleft;
    public Sprite sfireworksright;

    public void setBombUI()
    {
        bomb1Image.enabled = false;
        bomb1Count.enabled = false;
        bomb1X.enabled = false;
        bomb1Shape.enabled = false;
        bomb2Image.enabled = false;
        bomb2Count.enabled = false;
        bomb2X.enabled = false;
        bomb2Shape.enabled = false;
        bomb3Image.enabled = false;
        bomb3Count.enabled = false;
        bomb3X.enabled = false;
        bomb3Shape.enabled = false;

        if (level.invSize > 0)
        {
            bomb1Image.enabled = true;
            bomb1Image.GetComponent<Image>().sprite = getBombImage(level.InventoryArray[0]);
            setColorOriginal(bomb1Image, bomb1Count, bomb1X, bomb1Shape);
            bomb1Count.enabled = true;
            bomb1X.enabled = true;
            bomb1Shape.enabled = true;
            bomb1Shape.GetComponent<Image>().sprite = getBombShape(level.InventoryArray[0]);
        }
        if (level.invSize > 1)
        {
            bomb2Image.enabled = true;
            bomb2Image.GetComponent<Image>().sprite = getBombImage(level.InventoryArray[1]);
            setColorOriginal(bomb2Image, bomb2Count, bomb2X, bomb2Shape);
            bomb2Count.enabled = true;
            bomb2X.enabled = true;
            bomb2Shape.enabled = true;
            bomb2Shape.GetComponent<Image>().sprite = getBombShape(level.InventoryArray[1]);
        }
        if (level.invSize > 2)
        {
            bomb3Image.enabled = true;
            bomb3Image.GetComponent<Image>().sprite = getBombImage(level.InventoryArray[2]);
            setColorOriginal(bomb3Image, bomb3Count, bomb3X, bomb3Shape);
            bomb3Count.enabled = true;
            bomb3X.enabled = true;
            bomb3Shape.enabled = true;
            bomb3Shape.GetComponent<Image>().sprite = getBombShape(level.InventoryArray[2]);
        }
    }

    public Sprite getBombImage(Bomb bomb)
    {
        switch (bomb.GetShape())
        {
            case "1x5":
                return p1by5;
            case "5x1":
                return p5by1;
            case "1x3":
                return p1by3;
            case "3x1":
                return p3by1;
            case "5x5_cross":
                return p5by5cross;
            case "infinite_cross":
                return pinfbyinfcross;
            case "infinite_horizontal":
                return p1byinf;
            case "infinite_vertical":
                return pinfby1;
            case "up_fireworks":
                return pfireworksup;
            case "down_fireworks":
                return pfireworksdown;
            case "left_fireworks":
                return pfireworksleft;
            case "right_fireworks":
                return pfireworksright;
        }
        throw new IOException("Bomb Type Does Not Exist");
    }

    private Sprite getBombShape(Bomb bomb)
    {
        switch (bomb.GetShape())
        {
            case "1x5":
                return s1by5;
            case "5x1":
                return s5by1;
            case "1x3":
                return s1by3;
            case "3x1":
                return s3by1;
            case "5x5_cross":
                return s5by5cross;
            case "infinite_cross":
                return sinfbyinfcross;
            case "infinite_horizontal":
                return s1byinf;
            case "infinite_vertical":
                return sinfby1;
            case "up_fireworks":
                return sfireworksup;
            case "down_fireworks":
                return sfireworksdown;
            case "left_fireworks":
                return sfireworksleft;
            case "right_fireworks":
                return sfireworksright;
        }
        throw new IOException("Bomb Type Does Not Exist");
    }

    public void updateBombTextCount()
    {
        if (level.invSize > 0) bomb1Count.text = level.InventoryArray[0].GetQuant().ToString();
        if (level.InventoryArray.Length > 1) bomb2Count.text = level.InventoryArray[1].GetQuant().ToString();
        if (level.InventoryArray.Length > 2) bomb3Count.text = level.InventoryArray[2].GetQuant().ToString();
    }

    public void updateBombColor(){
        if (level.InventoryArray[0].GetQuant() == 0) setColorUnusable(bomb1Image, bomb1Count, bomb1X, bomb1Shape);
        if (level.invSize >= 2 && level.InventoryArray[1].GetQuant() == 0) setColorUnusable(bomb2Image, bomb2Count, bomb2X, bomb2Shape);
        if (level.invSize == 3 && level.InventoryArray[2].GetQuant() == 0) setColorUnusable(bomb3Image, bomb3Count, bomb3X, bomb3Shape);
    }

    // Selects bomb and shows highlight color
    public void selectBomb(int bombIndex)
    {
        if (bombIndex == 0 && level.InventoryArray[bombIndex].GetQuant() > 0)
        {
            setColorSelect(bomb1Image, bomb1Count, bomb1X, bomb1Shape);
            if (level.invSize >= 2 && level.InventoryArray[1].GetQuant() != 0) setColorOriginal(bomb2Image, bomb2Count, bomb2X, bomb2Shape);
            if (level.invSize == 3 && level.InventoryArray[2].GetQuant() != 0) setColorOriginal(bomb3Image, bomb3Count, bomb3X, bomb3Shape);
        }
        if (bombIndex == 1 && level.InventoryArray[bombIndex].GetQuant() > 0)
        {
            setColorSelect(bomb2Image, bomb2Count, bomb2X, bomb2Shape);
            if (level.invSize >= 2 && level.InventoryArray[0].GetQuant() != 0) setColorOriginal(bomb1Image, bomb1Count, bomb1X, bomb1Shape);
            if (level.invSize == 3 && level.InventoryArray[2].GetQuant() != 0) setColorOriginal(bomb3Image, bomb3Count, bomb3X, bomb3Shape);
        }
        if (bombIndex == 2 && level.InventoryArray[bombIndex].GetQuant() > 0)
        {
            setColorSelect(bomb3Image, bomb3Count, bomb3X, bomb3Shape);
            if (level.InventoryArray[0].GetQuant() != 0) setColorOriginal(bomb1Image, bomb1Count, bomb1X, bomb1Shape);
            if (level.InventoryArray[1].GetQuant() != 0) setColorOriginal(bomb2Image, bomb2Count, bomb2X, bomb2Shape);
        }
    }

    public void deselectBomb()
    {
        logic.setBombIndex(-1);
        setColorOriginal(bomb1Image, bomb1Count, bomb1X, bomb1Shape);
        setColorOriginal(bomb2Image, bomb2Count, bomb2X, bomb2Shape);
        setColorOriginal(bomb3Image, bomb3Count, bomb3X, bomb3Shape);
    }

    private void setColorUnusable(Image bombImg, Text bombCt, Text bombX, Image bombShape)
    {
        bombImg.color = unusableColor;
        bombCt.color = unusableColor;
        bombX.color = unusableColor;
        bombShape.color = unusableColor;
    }

    public void setColorOriginal(Image bombImg, Text bombCt, Text bombX, Image bombShape)
    {
        bombImg.color = defaultColor;
        bombCt.color = defaultColor;
        bombX.color = defaultColor;
        bombShape.color = defaultColor;
    }

    public void setColorSelect(Image bombImg, Text bombCt, Text bombX, Image bombShape)
    {
        bombImg.color = selectColor;
        bombCt.color = selectColor;
        bombX.color = selectColor;
        bombShape.color = selectColor;
    }

}
