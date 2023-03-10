using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGame : MonoBehaviour
{
    [Header("GameObject")]
    public GameObject Prefab_Cactus;

    [Header("Transform")]
    public Transform[] CactusPoint;

    [Header("Audio")]
    public AudioClip Clip_Dance;
    private AudioSource AS;

    [Header("Script")]
    private Character Character;
    public static MainGame Instance;

    [Header("UISetting")]
    public UI UISettings;

    private void Awake()
    {
        Instance = this;
        AS = GetComponent<AudioSource>();
        ManageCharacter();
    }
    private void Start()
    {
        AdManager.Instance.Request();
    }
    private void ManageCharacter()
    {
        GameObject Object = Instantiate(Prefab_Cactus, CactusPoint[0].position, Quaternion.Euler(CactusPoint[0].localEulerAngles));
        Character = Object.GetComponent<Character>();
    }
    public void Dance()
    {
        AdManager.Instance.ShowInterstitial();
        AS.PlayOneShot(Clip_Dance);
        UISettings.Dance_Button.SetActive(false);
        VoiceRecognitionRepeating.Instance.StopIt();
        Character.PlayAnimation("Dance");
        Invoke(nameof(AfterDance), Clip_Dance.length + 1f);
        MainGym MainGym = FindObjectOfType<MainGym>();
        MiniGame MiniGame = FindObjectOfType<MiniGame>();

        if (MainGym)
        {
            MainGym.Dance();
        }
        if (MiniGame)
        {
            MiniGame.Dance();
        }
    }
    public void StopDance()
    {
        AS.Stop();
        Character.PlayAnimation("Idle");
        UISettings.Dance_Button.SetActive(true);
    }
    private void AfterDance()
    {
        Character.PlayAnimation("Idle");
        UISettings.Dance_Button.SetActive(true);
        VoiceRecognitionRepeating.Instance.CanRecord = true;
        VoiceRecognitionRepeating.Instance.Record();
    }
    public void LoadScene(string Name)
    {
        AdManager.Instance.ShowInterstitial();
        UISettings.Panel_LoadingScreen.SetActive(true);
        StartCoroutine(LoadYourAsyncScene(0.25f, Name));
    }
    private IEnumerator LoadYourAsyncScene(float Timer, string Name)
    {
        yield return new WaitForSeconds(Timer);

        AsyncOperation AsyncLoad = SceneManager.LoadSceneAsync(Name);

        // Wait until the asynchronous scene fully loads
        while (!AsyncLoad.isDone)
        {
            UISettings.Loading.fillAmount = AsyncLoad.progress;
            yield return null;
        }
    }
    [System.Serializable]
    public class UI
    {
        [Header("GameObject")]
        public GameObject Dance_Button;
        public GameObject Panel_LoadingScreen;

        [Header("Image")]
        public Image Loading;
    }
}
