using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Events : MonoBehaviour
{

    public UnityEvent onLevelComplete;
    public UnityEvent hideLevelButtons;

    [System.Serializable]
    public class StringEvent : UnityEvent<string> { }

    // Use this for initialization
    private void Start () {
        
	}

    private void Update()
    {
        
    }

}
