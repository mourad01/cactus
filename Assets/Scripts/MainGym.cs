using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class MainGym : MonoBehaviour
{
    [Header("GameObject")]
    public GameObject Dance_Button;
    public GameObject Walk_Button;
    public GameObject Run_Button;

    [Header("Transform")]
    public Transform Treadmill;
    public Transform TreadmillPoint_Start;
    public Transform TreadmillPoint_End;

    [Header("Int")]
    private int Count;

    [Header("Audio")]
    public AudioClip Clip_Gym;
    private AudioSource AS;

    [Header("Text")]
    public TextMeshProUGUI TM_Count;

    [Header("Script")]
    private Character Character;
    private MainGame MainGame;

    private void Awake()
    {
        AS = GetComponent<AudioSource>();
        MainGame = FindObjectOfType<MainGame>();
    }
    // Start is called before the first frame update
    void Start()
    {
        SetData();
    }
    private void SetData()
    {
        Character = FindObjectOfType<Character>();
        VoiceRecognitionRepeating.Instance.CanRecord = true;
        VoiceRecognitionRepeating.Instance.Record();
        Walk_Button.SetActive(true);
        Run_Button.SetActive(true);
        Dance_Button.SetActive(true);
    }
    public void Dance()
    {
        AS.Stop();
        Walk_Button.SetActive(true);
        Run_Button.SetActive(true);
        CancelInvoke(nameof(StopingTreadmill));
        CancelInvoke(nameof(Counting));
        Count = 0;
        TM_Count.text = Count.ToString();
        Treadmill.DOMove(TreadmillPoint_Start.position, 1f);
    }
    public void Walk()
    {
        MainGame.StopDance();
        Walk_Button.SetActive(false);
        Run_Button.SetActive(false);
        VoiceRecognitionRepeating.Instance.StopIt();
        if (Treadmill.position == TreadmillPoint_End.position)
        {
            Character.PlayAnimation("Walk");
            Walk_Button.SetActive(false);
            Run_Button.SetActive(true);
        }
        else
        {
            Treadmill.DOMove(TreadmillPoint_End.position, 1f).OnComplete(() =>
            {
                Character.PlayAnimation("Walk"); AS.PlayOneShot(Clip_Gym); Invoke(nameof(StopingTreadmill), Clip_Gym.length); Walk_Button.SetActive(false);
                Run_Button.SetActive(true);
                Invoke(nameof(Counting), 1f);
            });
        }
    }
    public void Run()
    {
        MainGame.StopDance();
        Walk_Button.SetActive(false);
        Run_Button.SetActive(false);
        VoiceRecognitionRepeating.Instance.StopIt();
        if (Treadmill.position == TreadmillPoint_End.position) 
        {
            Character.PlayAnimation("Run");
            Walk_Button.SetActive(true);
            Run_Button.SetActive(false);
        }
        else
        {
            Treadmill.DOMove(TreadmillPoint_End.position, 1f).OnComplete(() =>
            {
                Character.PlayAnimation("Run"); AS.PlayOneShot(Clip_Gym); Invoke(nameof(StopingTreadmill), Clip_Gym.length); Walk_Button.SetActive(true);
                Run_Button.SetActive(false);
                Invoke(nameof(Counting), 1f);
            });
        }
    }
    private void StopingTreadmill()
    {
        Walk_Button.SetActive(false);
        Run_Button.SetActive(false);
        Character.PlayAnimation("Idle");
        CancelInvoke(nameof(Counting));
        Treadmill.DOMove(TreadmillPoint_Start.position, 1f).OnComplete(() =>
        {
            Count = 0;
            TM_Count.text = Count.ToString();
            VoiceRecognitionRepeating.Instance.CanRecord = true;
            VoiceRecognitionRepeating.Instance.Record(); Walk_Button.SetActive(true);
            Run_Button.SetActive(true);
        });
    }
    private void Counting()
    {
        Count++;
        TM_Count.text = Count.ToString();
        Invoke(nameof(Counting), 1f);
    }
}
