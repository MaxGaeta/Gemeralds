using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverController : MonoBehaviour {

    public LevelData level;
    public InventoryController invC;
    // Color for bomb shape not including the center color at a free space (currently BLUE)
    private Color freeShape = new Color(255 / 255f, 255 / 255f, 255 / 255f, 0.5f);
    // Color for bomb shape not including the center color at an occupied space (currently LIGHT RED)
    private Color occupiedShape = new Color(190 / 255f, 30 / 255f, 46 / 255f, 0.55f);
    // Color for center piece at a free space (currently GREEN)
    private Color freeMiddle = new Color(255 / 255f, 255 / 255f, 255 / 255f, 1f);
    // Color for center piece at an occupied space (currently DARK RED)
    private Color occupiedMiddle = new Color(190 / 255f, 30 / 255f, 46 / 255f, 1f);

    public Image boardResetImage;
    public Image boardLevelSelectImage;
    public Image boardSoundImage;
    public Image boardMusicImage;
    public Image resetImage;
    public Image continueImage;
    public Image prevImage;
    public ButtonClicker interlevel;
    private Color defaultColor = new Color(0, 0, 0, 1f);
    private Color highlightColor = Color.white;

    // Use this for initialization
    public void InstantiateHoverGrid () 
    {

        level.HoverData = new GameObject[level.squaresX, level.squaresY];
        for (int i = 0; i < level.squaresX; i++)
        {
            for (int j = 0; j < level.squaresY; j++)
            {
                level.HoverData[i, j] = level.HoverSquare;
            }
        }
        
    }

    public void bombShapeHighlightFree(List<Position> affectedArea)
    {
        foreach (Position space in affectedArea)
        {
            if (!isTileShape(level.HoverObjects[space.x, space.y].GetComponent<Image>().sprite)) level.HoverObjects[space.x, space.y].GetComponent<Image>().sprite = level.RandomHover();
            level.HoverObjects[space.x, space.y].GetComponent<Image>().color = freeShape;
        }
    }

    public void bombShapeHighlight(List<Position> affectedArea)
    {
        foreach (Position space in affectedArea)
        {
            if (!isTileShape(level.HoverObjects[space.x, space.y].GetComponent<Image>().sprite)) level.HoverObjects[space.x, space.y].GetComponent<Image>().sprite = level.RandomHover();
            level.HoverObjects[space.x, space.y].GetComponent<Image>().color = occupiedShape;
        }
    }

    public void resetBombShapeHighlight(List<Position> affectedArea)
    {
        foreach (Position space in affectedArea)
        {
            resetSpace(space);
        }
    }

    public void resetSpace(Position pos)
    {
        level.HoverObjects[pos.x, pos.y].GetComponent<Image>().color = Color.clear;
    }

    public void resetAllHover()
    {
        foreach (GameObject g in level.HoverObjects) g.GetComponent<Image>().color = Color.clear;
    }

    // Color of middle block that is a free space
    public void setMiddleBlockFree(Position pos, Bomb bomb)
    {
        level.HoverObjects[pos.x, pos.y].GetComponent<Image>().sprite = invC.getBombImage(bomb);
        level.HoverObjects[pos.x, pos.y].GetComponent<Image>().color = freeMiddle;
    }

    // Color of middle block that is not a free space
    public void setMiddleBlock(Position pos, Bomb bomb)
    {
        level.HoverObjects[pos.x, pos.y].GetComponent<Image>().sprite = invC.getBombImage(bomb);
        level.HoverObjects[pos.x, pos.y].GetComponent<Image>().color = occupiedMiddle;
    }

    public bool isTileShape(Sprite sprite) 
    {
        return (sprite == level.hoverSprite1 || sprite == level.hoverSprite2 ||
                sprite == level.hoverSprite3 || sprite == level.hoverSprite4 ||
                sprite == level.hoverSprite5 || sprite == level.hoverSprite6 ||
                sprite == level.hoverSprite7 || sprite == level.hoverSprite8);
    }

    // Color of game board's reset button highlight
    public void highlightBoardReset()
    {
        boardResetImage.color = highlightColor;
    }

    // Color of game board's reset button
    public void originalBoardReset()
    {
        boardResetImage.color = defaultColor;
    }

    // Color of game board's level select button highlight
    public void highlightBoardLevelSelect()
    {
        boardLevelSelectImage.color = highlightColor;
    }

    // Color of game board's level select button
    public void originalBoardLevelSelect()
    {
        boardLevelSelectImage.color = defaultColor;
    }

    // Color of game board's level select button highlight
    public void highlightBoardMusic()
    {
        boardMusicImage.color = highlightColor;
    }

    // Color of game board's level select button
    public void originalBoardMusic()
    {
        boardMusicImage.color = defaultColor;
    }

    // Color of game board's level select button highlight
    public void highlightBoardSound()
    {
        boardSoundImage.color = highlightColor;
    }

    // Color of game board's level select button
    public void originalBoardSound()
    {
        boardSoundImage.color = defaultColor;
    }

    // Highlights inter-level buttons
    public void highlightInterlevelButton(int value)
    {
        switch (value)
        {
            case 0:
                prevImage.color = highlightColor;
                break;
            case 1:
                resetImage.color = highlightColor;
                break;
            case 2:
                continueImage.color = highlightColor;
                break;
        }
    }

    // Highlights inter-level buttons
    public void originalInterlevelButton(int value)
    {
        switch (value)
        {
            case 0:
                prevImage.color = defaultColor;
                break;
            case 1:
                resetImage.color = defaultColor;
                break;
            case 2:
                continueImage.color = defaultColor;
                break;
        }
    }
}
