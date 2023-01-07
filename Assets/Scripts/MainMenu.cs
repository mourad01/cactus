using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Microphone.GetPosition("");
        }
    }
    public void Play()
    {
        SceneManager.LoadScene(1);
    }
}
