using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AnimationHandler : MonoBehaviour {

	public GameObject myCharacter;
	public AudioSource audioPlayingSource; 

	public static AnimationHandler _instance;

	// Use this for initialization
	void Awake ()
	{
		_instance = this;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
