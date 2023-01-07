using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Character : MonoBehaviour
{
    [Header("GameObject")]
    public GameObject[] SoapBubbles;

    [Header("Transform")]
    public Transform[] PointDetector;

    [Header("String")]
    public string CurrentAnimation;

    [Header("Animator")]
    private Animator AM;

    [Header("Rigidbody")]
    private Rigidbody RB;

    [Header("Layer")]
    public LayerMask Mask_Bloc;

    [Header("Raycast")]
    private RaycastHit Hit;

    [Header("Script")]
    private MainCamera MainCamera;

    private void Awake()
    {
        AM = GetComponent<Animator>();
        RB = GetComponent<Rigidbody>();
        MainCamera = FindObjectOfType<MainCamera>();
    }
    public void PlayAnimation(string Name)
    {
        if (!Name.Equals("Jump") && !Name.Equals("Jump Trampoline"))
        {
            if (!Name.Equals(CurrentAnimation))
            {
                AM.SetTrigger(Name);
                CurrentAnimation = Name;
            }
        }
        else
        {
            AM.SetTrigger(Name);
            CurrentAnimation = Name;
        }
    }
    public void Death(string Direction)
    {
        RB.constraints = RigidbodyConstraints.None;
        GetComponent<Collider>().isTrigger = true;

        switch (Direction)
        {
            case ("Right"):

                RB.AddForce(Vector3.right * 8, ForceMode.Impulse);
                transform.DORotate(new Vector3(-90, 180, 90), 0.5f);
                break;

            case ("Left"):

                RB.AddForce(Vector3.left * 8, ForceMode.Impulse);
                transform.DORotate(new Vector3(-90, 180, -90), 0.5f);
                break;
        }
        RB.AddForce(Vector3.up * 10, ForceMode.Impulse);
        MiniGame.Instance.Fail();
        MainCamera.Unfollow();
    }
    public void ShowSoapBubbles()
    {
        for (int i = 0; i < SoapBubbles.Length; i++)
        {
            SoapBubbles[i].SetActive(true);
        }
    }
    public void DetectBloc(GameObject Bloc)
    {
        for (int i = 0; i < PointDetector.Length; i++)
        {
            if (Physics.Raycast(PointDetector[i].position + new Vector3(0, 0.25f, 0), Vector3.down, out Hit, 10, Mask_Bloc))
            {
                if (Hit.collider.gameObject == Bloc)
                {
                    Bloc.GetComponent<Bloc>().ManageDetecter("Win", this);
                }
                else
                {
                    Bloc.GetComponent<Bloc>().ManageDetecter("Lose", this);
                }
                break;
            }
            if (i.Equals(PointDetector.Length - 1))
            {
                Bloc.GetComponent<Bloc>().ManageDetecter("Lose", this);
            }
        }
        Bloc.tag = "Untagged";
    }
    private void OnCollisionEnter(Collision Coll)
    {
        if (Coll.gameObject.CompareTag("Bloc"))
        {
            RB.constraints = RigidbodyConstraints.FreezeAll;
            DetectBloc(Coll.gameObject);
        }
        MiniGame.Instance.ManageControlle(true);
    }
    private void OnTriggerEnter(Collider Other)
    {
        if (Other.CompareTag("Line"))
        {
            Destroy(Other.gameObject);
        }
    }
}
