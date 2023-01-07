using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedRoom : MonoBehaviour
{
    [Header("GameObject")]
    public GameObject Light;
    public GameObject FX_Zzz;
    public GameObject LightON_Button;
    public GameObject LightOFF_Button;

    [Header("Transform")]
    public Transform CactusPoint_ON;
    public Transform CactusPoint_OFF;

    [Header("Colors")]
    public Color LightON_Color;
    public Color LightOFF_Color;

    [Header("Sprite")]
    public SpriteRenderer[] BedBackground;

    [Header("Audio")]
    public AudioClip Clip_Sleep;
    private AudioSource AS;

    [Header("Script")]
    private Character Character;

    private void Awake()
    {
        AS = GetComponent<AudioSource>();
    }
    private void Start()
    {
        SetData();
    }
    private void SetData()
    {
        Character = FindObjectOfType<Character>();

        for (int i = 0; i < BedBackground.Length; i++)
        {
            BedBackground[i].color = LightOFF_Color;
        }
        VoiceRecognitionRepeating.Instance.StopIt();
        Character.PlayAnimation("Sleep");
        LightOFF_Button.SetActive(true);
        AS.clip = Clip_Sleep;
        AS.Play();
    }
    public void LightON()
    {
        AdManager.Instance.ShowInterstitial();
        for (int i = 0; i < BedBackground.Length; i++)
        {
            BedBackground[i].color = LightON_Color;
        }
        LightOFF_Button.SetActive(false);
        LightON_Button.SetActive(true);
        Character.PlayAnimation("Idle");
        Light.SetActive(true);
        Character.transform.localEulerAngles = CactusPoint_ON.localEulerAngles;
        FX_Zzz.SetActive(false);
        AS.Stop();
    }
    public void LightOFF()
    {
        AdManager.Instance.ShowInterstitial();
        for (int i = 0; i < BedBackground.Length; i++)
        {
            BedBackground[i].color = LightOFF_Color;
        }
        LightON_Button.SetActive(false);
        LightOFF_Button.SetActive(true);
        Character.PlayAnimation("Sleep");
        Light.SetActive(false);
        Character.transform.localEulerAngles = CactusPoint_OFF.localEulerAngles;
        FX_Zzz.SetActive(true);
        AS.clip = Clip_Sleep;
        AS.Play();
    }
}
