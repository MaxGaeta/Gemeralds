using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {

    public Timer Waiter;

    public Image Arrow;
    public Image Arrow2;
    public Text Message;

    public int lvl;
    public bool active = false;
    public int lvlprog = 0;

    public int waitingOnBomb = -1;
    
    public int waitTime = 0;

    //tutorials can only be hard coded
    public void StartTutorial(int level)
    {
        active = true;
        lvl = level;
        waitTime = 1;

        Arrow.enabled = false;
        Arrow2.enabled = false;
        Message.enabled = false;
    }

    private void Tutorial1()
    {
        // starting lvl
        if (lvlprog == 0)
        {
            Arrow.enabled = true;
            Message.enabled = true;
            //wait for user to click on bomb 0;
            Message.text = "Pick up the bomb!";
        }
        else if (lvlprog == 1)
        {
            // user has clicked on bomb 0;
            waitingOnBomb = -1;
            Message.enabled = true;
            Arrow.enabled = false;
            Arrow2.enabled = true;
            Message.text = "Place the bomb on the empty tile!";
        }
        else if (lvlprog == 2)
        {
            EndTutorial();
        }
    }

    private void Tutorial3()
    {
        // starting lvl
        if (lvlprog == 0)
        {
            Arrow.enabled = true;
            Message.enabled = true;
            //wait for user to click on bomb 0;
            Message.text = "Pick up the bomb!";
        }
        else if (lvlprog == 1)
        {
            // user has clicked on bomb 0;
            waitingOnBomb = -1;
            Message.enabled = true;
            Arrow.enabled = false;
            //Arrow2.transform.position.Set(345.5f, 259f, 0);
            //Arrow2.enabled = true;
            Message.text = "Place the bomb on the empty tile!";
        }
        else if (lvlprog == 2)
        {
            Arrow.enabled = true;
            Arrow2.enabled = false;
            //wait for user to click on bomb 0 again;
            Message.text = "Pick up the bomb again!";
            waitingOnBomb = 0;
        }
        else if (lvlprog == 3)
        {
            // user has clicked on bomb 0again;
            waitingOnBomb = -1;
            Arrow.enabled = false;
            //Arrow2.enabled = true;
            Message.text = "Place the bomb on the newly cleared tile!";
        }
        else if (lvlprog == 4)
        {
            EndTutorial();
        }
    }

    public void EndTutorial()
    {
        active = false;
        Arrow.enabled = false;
        Arrow2.enabled = false;
        Message.enabled = false;
        lvlprog = 0;
        waitingOnBomb = -1;
    }
	
	// Update is called once per frame
	void Update () {
		if (active)
        {
            if ((lvl == 1 || lvl == 3) && (lvlprog == 0 || lvlprog == 2)) waitingOnBomb = 0;

            if (waitTime > 0)
            {
                Waiter.WaitFor(waitTime);
                waitTime = 0;
            }
            if (Waiter.ready)
            {
                waitTime = 0;

                if (lvl == 1) Tutorial1();
                if (lvl == 3) Tutorial3();
            }
        }
	}
}
