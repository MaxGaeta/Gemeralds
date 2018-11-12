using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemAnimation : MonoBehaviour {

    public static List<GemAnimation> gemsToReset;
    public enum AnimationOccuring { none, collecting, destroying };

    float timeSinceRun;

    private const float targetCollectSize = 0.5f;
    private const float targetDestroySize = 3f;
    private const float collectSpeedMod = 1.5f;

    private AnimationOccuring animationType = AnimationOccuring.none;

    private Transform gem;
    private Vector3 targetPos;          //Ending position for the animation
    private Vector3 targetPosRelative;  //Vector from start position to ending
    private Vector3 perpVector;         //Perpendicular vector used for parabolic motion
        //along the targetPosRelative vector

    private Vector3 origPos;
    private Vector3 origScale;
    private float timeStartRunning; //Time that animationType was set to != none

    public AnimationCurve collectionSpeed;  //x-axis time, y-axis amount moved along collection parabola

	// Use this for initialization
	void Start () {
        //Initialize list if not initialized
        if (gemsToReset == null)
        {
            gemsToReset = new List<GemAnimation>();
        }

        //Add to array
        gemsToReset.Add(this);

        gem = gameObject.GetComponent<Transform>();
	}

    public void CollectGem(int collectedGems)
    {
        animationType = AnimationOccuring.collecting;

        //Make the gem transparent
        matColor = GetComponentInChildren<MeshRenderer>().material.color;
        matColor.a = 0.3f;
        GetComponentInChildren<MeshRenderer>().material.color = matColor;

        //Get the eventual position that this gem will settle into
        targetPos =
            new Vector3(16.2f, 5f, -21.7f)+
                  Camera.main.transform.right * 1.5f * collectedGems +
                              Camera.main.transform.up * 0.5f;
           
        //Calculate the two vectors to travel - linear and polynomial curve direction
        targetPosRelative = targetPos - gem.position;
        perpVector = Vector3.Normalize(Vector3.Cross(targetPosRelative, Vector3.up));
        perpVector *= (targetPosRelative.magnitude) / 5f;

        StartAnim();
    }

    public void DestroyGem()
    {

        animationType = AnimationOccuring.destroying;

        //Get the eventual position that this gem will settle into
        targetPos =
            transform.position + 
            Camera.main.transform.forward * -3f;

        StartAnim();
    }

    void StartAnim()
    {
        timeSinceRun = 0f;
        origPos = gem.position;
        origScale = gem.localScale;
    }

    Color matColor;
    void Update () {
        timeSinceRun += Time.deltaTime;
        float amountMoved = collectionSpeed.Evaluate((timeSinceRun) * collectSpeedMod);

        switch (animationType)
        {
            case AnimationOccuring.collecting:
                gem.position = Vector3.Lerp(origPos, targetPos, amountMoved);
                gem.position += perpVector * (1 - Mathf.Pow((2f*amountMoved - 1), 2f));

                gem.localScale = Vector3.Lerp(origScale, 
                                              new Vector3(targetCollectSize, targetCollectSize, targetCollectSize),
                                              amountMoved);

                if (amountMoved.Equals(1f))
                {
                    screenShake.shakeTime = 0.7f;
                    GetComponent<ParticleSystem>().Play();
                    animationType = AnimationOccuring.none;
                      
                    //Make opaque again
                    matColor = GetComponentInChildren<MeshRenderer>().material.color;
                    matColor.a = 1f;
                    GetComponentInChildren<MeshRenderer>().material.color = matColor;
                }
                break;
            case AnimationOccuring.destroying:

                //Move gem
                gem.position = Vector3.Lerp(origPos, targetPos, amountMoved);
                gem.position += Random.insideUnitSphere * amountMoved;
                gem.localScale = Vector3.Lerp(origScale,
                                              new Vector3(targetDestroySize, targetDestroySize, targetDestroySize),
                                              amountMoved);

                //Update color
                matColor = GetComponentInChildren<MeshRenderer>().material.color;
                matColor = Color.Lerp(matColor, Color.black, amountMoved * 1.5f);
                GetComponentInChildren<MeshRenderer>().material.color = matColor;

                //Slow down time
                Time.timeScale = (1 - amountMoved);
                if (amountMoved.Equals(1f))
                {
                    animationType = AnimationOccuring.none;
                }

                break;
            case AnimationOccuring.none:

                break;
        }
	}

    /// <summary>
    /// Removes the animated gems, resets Time,timescale
    /// </summary>
    public static void ResetAnimations()
    {
        Time.timeScale = 1f;
        // print("no");
        if (gemsToReset != null)
        {
            foreach (GemAnimation gem in gemsToReset)
            {
                Destroy(gem.gameObject);
            }
            gemsToReset = new List<GemAnimation>();
        }
    }
}
