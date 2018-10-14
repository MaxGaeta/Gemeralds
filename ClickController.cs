using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickController : MonoBehaviour, IPointerClickHandler
{
    public LevelController levelC;
    public InventoryController invC;
    public GameLogic logic;

    public enum ButtonType
    {
        BOMB0,
        BOMB1,
        BOMB2,
        RESET
    };

    public ButtonType Type;

    public void OnPointerClick(PointerEventData eventData)
    {

        switch (Type)
        {
            case ButtonType.RESET:
                levelC.ResetLevel();
                break;
            case ButtonType.BOMB0:

                logic.setBombIndex(0);
                invC.selectBomb(0);
                break;
            case ButtonType.BOMB1:

                logic.setBombIndex(1);
                invC.selectBomb(1);
                break;
            case ButtonType.BOMB2:

                logic.setBombIndex(2);
                invC.selectBomb(2);
                break;
        }
        Debug.Log(name + " Game Object Left Clicked!");
        
    }

}
