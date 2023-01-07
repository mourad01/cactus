using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class MiniGame : MonoBehaviour
{
    [Header("GameObject")]
    public GameObject Prefab_Bloc;
    public GameObject Score;
    public GameObject Button_Dance;
    public GameObject Button_StackJump;
   // public GameObject Button_Trampoline;
    public GameObject Panel_GameOver;
  //  public GameObject Trampoline;
    public GameObject Football;
    public GameObject Guide;

    [Header("Transform")]
    public Transform Bloc_Point;

    [Header("Float")]
    private float Speed;

    [Header("Int")]
    private int CountDrag;
    private int CountScore;

    [Header("Vector")]
    private Vector3 Offset;
    private Vector3 Scale = Vector3.one * 1.5f;

    [Header("Colors")]
    public Color[] Colors;
    private Color DefaultColor;

    [Header("Bool")]
    private bool YouLose;
    private bool Play;
    private bool IsDrag;

    [Header("Text")]
    public TextMeshProUGUI TM_Score;
    public TextMeshProUGUI TM_BestScore;

    [Header("Audio")]
    public AudioClip Clip_Jump;
    private AudioSource AS;

    [Header("Touch")]
    private Touch Touch;

    [Header("Script")]
    private Character Character;
    private MainCamera MainCamera;
    private MainGame MainGame;
    private List<Bloc> ListBloc = new List<Bloc>();
    public static MiniGame Instance;

    private void Awake()
    {
        Instance = this;
        AS = GetComponent<AudioSource>();
    }
    private void Start()
    {
        Character = FindObjectOfType<Character>();
        MainCamera = FindObjectOfType<MainCamera>();
        MainGame = FindObjectOfType<MainGame>();
        SetData();
    }
    private void SetData()
    {
        Button_Dance.SetActive(true);
        Button_StackJump.SetActive(true);
     //   Button_Trampoline.SetActive(true);
        VoiceRecognitionRepeating.Instance.CanRecord = true;
        VoiceRecognitionRepeating.Instance.Record();
    }
    private void Update()
    {
#if UNITY_EDITOR

        if (Input.GetMouseButtonDown(0))
        {
            Inputs();
        }
#else 
        if (Input.touchCount > 0)
        {
            Touch = Input.GetTouch(0);
            if (Touch.phase == TouchPhase.Began)
            {
                Inputs();
            }
        }
#endif
    }
    private void FixedUpdate()
    {
        if (IsDrag)
        {
            Character.GetComponent<Rigidbody>().AddForce(Vector3.up * 12, ForceMode.Impulse);
            IsDrag = false;
        }
    }
    private void Inputs()
    {
        if (Play)
        {
            if (!YouLose)
            {
                IsDrag = true;
                Character.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                Character.PlayAnimation("Jump");
                if (Character.transform.position.y > 0f)
                {
                    if (CountDrag < 80)
                    {
                        MainCamera.OffSet -= new Vector3(0, 0, 0.25f);
                        CountDrag++;
                    }
                }
                Play = false;
                if (!PlayerPrefs.GetString("Guide").Equals("Done"))
                {
                    Guide.SetActive(false);
                    Time.timeScale = 1f;
                    PlayerPrefs.SetString("Guide", "Done");
                }
            }
        }
    }
    public void Dance()
    {
        
    }
    public void PlayStackJump()
    {

        Play = true;
        MainGame.StopDance();
        Score.SetActive(true);
        Football.SetActive(false);
        Button_Dance.SetActive(false);
        Button_StackJump.SetActive(false);
    //    Button_Trampoline.SetActive(false);
        MainCamera.SetTarget(Character.transform);
        Character.PlayAnimation("Idle");
        VoiceRecognitionRepeating.Instance.StopIt();
        DefaultColor = Colors[Random.Range(0, Colors.Length)];
        InvokeRepeating(nameof(SpawnBloc), 1.5f, 1);
        Character.GetComponent<Rigidbody>().isKinematic = false;

        if (!PlayerPrefs.GetString("Guide").Equals("Done"))
        {
            StartCoroutine(ShowGuide(2.25f));

            IEnumerator ShowGuide(float Timer)
            {
                yield return new WaitForSeconds(Timer);

                Guide.SetActive(true);
                Time.timeScale = 0.25f;
            }
        }
    }
 /*   public void PlayTrampoline()
    {
        MainGame.StopDance();
        Football.SetActive(false);
        Button_Dance.SetActive(false);
        Button_StackJump.SetActive(false);
        Button_Trampoline.SetActive(false);
        VoiceRecognitionRepeating.Instance.StopIt();
        Trampoline.GetComponent<Trampoline>().ManageTrampoline(true);
    }
 */
    public void ResetAfterTrampoline()
    {
        Football.SetActive(true);
        Button_Dance.SetActive(true);
        Button_StackJump.SetActive(true);
    //    Button_Trampoline.SetActive(true);
        VoiceRecognitionRepeating.Instance.CanRecord = true;
        VoiceRecognitionRepeating.Instance.Record();
    }
    public void AddBloc(Bloc bloc)
    {
        ListBloc.Add(bloc);

        for (int i = 0; i < ListBloc.Count; i++)
        {
            ListBloc[i].Flash();
        }
    }
    public void CleahBloc()
    {
        ListBloc.Clear();
    }
    public void ManageControlle(bool Value)
    {
        Play = Value;
    }
    public void AddScore()
    {
        CountScore++;
        Score.GetComponent<TextMeshProUGUI>().text = CountScore.ToString();
    }
    public void Fail()
    {
        YouLose = true;
        TM_Score.text = "SCORE : " + CountScore.ToString();
        if (!PlayerPrefs.GetString("Guide").Equals("Done"))
        {
            Guide.SetActive(false);
            Time.timeScale = 1f;
            PlayerPrefs.SetString("Guide", "Done");
        }
        if (CountScore > PlayerPrefs.GetInt("BestScore"))
        {
            TM_BestScore.text = "BEST SCORE : " + CountScore.ToString();
            PlayerPrefs.SetInt("BestScore", CountScore);
        }
        else
        {
            TM_BestScore.text = "BEST SCORE : " + PlayerPrefs.GetInt("BestScore").ToString();
        }
        StartCoroutine(ManagePanel(1f, Panel_GameOver, true));
        CancelInvoke(nameof(SpawnBloc));
    }
    public void TryAgain()

    {
        AdManager.Instance.ShowInterstitial();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void SpawnBloc()
    {
        GameObject Object = Instantiate(Prefab_Bloc, Bloc_Point.position + Offset, Quaternion.identity);

        switch (Random.Range(0,2))
        {
            case (0):

                Object.transform.position += new Vector3(15, 0, 0);
                Object.GetComponent<Bloc>().SetDistination("Left", Speed, DefaultColor, Scale);
                break;

            case (1):

                Object.transform.position -= new Vector3(15, 0, 0);
                Object.GetComponent<Bloc>().SetDistination("Right", Speed, DefaultColor, Scale);
                break;
        }

        if (DefaultColor.g < 0.75f)
        {
            DefaultColor.g += 0.02f;
        }
        if (Scale.x > 1f)
        {
            Scale -= new Vector3(0.01f, 0f, 0f);
        }
        Offset += new Vector3(0, 0.95f, 0);
        if (Speed < 2)
        {
            Speed += 0.1f;
        }
    }
    private IEnumerator ManagePanel(float Timer, GameObject Panel, bool Value)
    {
        yield return new WaitForSeconds(Timer);

        Panel.SetActive(Value);
    }
}
