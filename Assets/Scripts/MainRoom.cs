using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRoom : MonoBehaviour
{
    [Header("GameObject")]
    public GameObject Dance_Button;

    void Start()
    {
        SetData();
    }
    private void SetData()
    {
        Dance_Button.SetActive(true);
        VoiceRecognitionRepeating.Instance.CanRecord = true;
        VoiceRecognitionRepeating.Instance.Record();
    }
}
