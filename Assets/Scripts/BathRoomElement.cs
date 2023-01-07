using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BathRoomElement : MonoBehaviour
{
    [Header("GameObject")]
    public GameObject FX_Shower;

    [Header("String")]
    public string Type;

    [Header("Vector")]
    public Vector3 Offset;

    [Header("SpriteRenderer")]
    public SpriteRenderer Outline;

    [Header("Tweener")]
    private Tweener Tween;

    private void Start()
    {
        switch (Type)
        {
            case ("Door"):

                ManageOutline(true);
                Tween = transform.DOMove(Offset, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
                break;
        }
    }
    public void ManageOutline(bool Value)
    {
        switch (Value)
        {
            case true:

                Outline.DOColor(new Color(1, 1, 1, 0.25f), 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
                break;
        }
        Outline.enabled = Value;
    }
    public void Manage(bool Value)
    {
        switch (Value)
        {
            case true:

                switch (Type)
                {
                    case ("Door"):

                        Tween.Kill();
                        break;

                    case ("Shower"):

                        FX_Shower.SetActive(true);
                        break;
                }
                break;

            case false:

                switch (Type)
                {
                    case ("Door"):

                        Tween = transform.DOMove(Offset, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
                        break;

                    case ("Shower"):

                        FX_Shower.SetActive(false);
                        break;
                }
                break;
        }
    }
}
