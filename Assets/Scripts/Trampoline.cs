using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Trampoline : MonoBehaviour
{
    [Header("Transform")]
    public Transform Joint;
    public Transform StartJoint;
    public Transform EndJoint;
    public Transform PositionCactus_1;
    public Transform PositionCactus_2;
    public Transform PositionCactus_3;

    [Header("Int")]
    private int CountJump;

    [Header("Vector")]
    public Vector3 StartPosition;
    public Vector3 EndPosition;

    [Header("Audio")]
    public AudioClip Clip_Trampoline;
    private AudioSource AS;

    [Header("Script")]
    private Character Character;

    private void Awake()
    {
        AS = GetComponent<AudioSource>();
    }
    private void Start()
    {
        Character = FindObjectOfType<Character>();
    }
    public void ManageTrampoline(bool Value)
    {
        switch (Value)
        {
            case true:

                transform.DOMove(EndPosition, 1f).OnComplete(() => { StartCoroutine(Step(0f, "1")); });
                break;

            case false:

                AS.Stop();
                transform.DOMove(StartPosition, 1f).OnComplete(() => { MiniGame.Instance.ResetAfterTrampoline(); });
                break;
        }
    }
    private IEnumerator Step(float Timer, string Value)
    {
        yield return new WaitForSeconds(Timer);

        switch (Value)
        {
            case ("1"):

                if (CountJump < 3)
                {
                    Character.transform.DOMove(PositionCactus_1.position, 0.25f);
                    Joint.DOLocalMove(StartJoint.localPosition, 0.25f).OnComplete(() => { StartCoroutine(Step(0f, "2")); });
                }
                else
                {
                    CountJump = 0;
                    ManageTrampoline(false);
                }
                break;

            case ("2"):

                AS.PlayOneShot(Clip_Trampoline);
                Character.PlayAnimation("Jump Trampoline");
                Joint.DOLocalMove(EndJoint.localPosition, 0.25f);
                Vector3[] Path = new Vector3[2];
                Path[0] = PositionCactus_2.position;
                Path[1] = PositionCactus_3.position;
                Character.transform.DOPath(Path, 1, PathType.Linear, PathMode.Full3D).OnComplete(() => { CountJump++; StartCoroutine(Step(0f, "1")); });
                break;
        }
    }
}
