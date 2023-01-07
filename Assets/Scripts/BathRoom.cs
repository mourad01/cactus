using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BathRoom : MonoBehaviour
{
    [Header("GameObject")]
    public GameObject Fade;
    public GameObject[] HideElementInLoo;

    [Header("Transform")]
    public Transform Door;
    public Transform Shower;
    public Transform Sponge;
    public Transform DoorOpen_Point;
    public Transform DoorClosed_Point;
    public Transform Sponge_Point;
    public Transform Shower_Point;
    public Transform Cactus_Point;
    public Transform CactusInLoo_Point;

    [Header("String")]
    private string Element;

    [Header("Float")]
    public float MinClampDoor;
    public float MaxClampDoor;
    public float MinClampShower;
    public float MaxClampShower;
    private float Offset;

    [Header("Int")]
    [HideInInspector]
    public int CountSoapBubbles;

    [Header("Vector")]
    private Vector3 MousePosition;

    [Header("Bool")]
    private bool IsDragDoor;
    private bool IsDragShower;
    private bool IsDragSponge;
    private bool Stop;

    [Header("Raycast")]
    private Ray Ray;
    private RaycastHit Hit;

    [Header("LayerMask")]
    public LayerMask SoapBubbleON_Mask;
    public LayerMask SoapBubbleOFF_Mask;
    public LayerMask Default_Mask;

    [Header("Audio")]
    public AudioClip Clip_Loo;
    public AudioClip Clip_ToiletFlush;
    private AudioSource AS;

    [Header("Camera")]
    private Camera Camera;

    [Header("Script")]
    public BathRoomElement BathRoomElement_Door;
    public BathRoomElement BathRoomElement_Shower;
    public BathRoomElement BathRoomElement_Sponge;
    private Character Character;

    private void Awake()
    {
        Camera = Camera.main;
        AS = GetComponent<AudioSource>();
    }
    private void Start()
    {
        Character = FindObjectOfType<Character>();
        SetData();
    }
    private void SetData()
    {
        Character.ShowSoapBubbles();
        VoiceRecognitionRepeating.Instance.CanRecord = true;
        VoiceRecognitionRepeating.Instance.Record();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!Stop)
            {
                Ray = Camera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

                if (Physics.Raycast(Ray, out Hit, 50, Default_Mask))
                {
                    Element = Hit.collider.GetComponent<BathRoomElement>().Type;
                    MousePosition = Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

                    switch (Element)
                    {
                        case ("Door"):

                            IsDragDoor = true;
                            Offset = MousePosition.x - Door.position.x;
                            BathRoomElement_Door.Manage(true);
                            BathRoomElement_Door.ManageOutline(false);
                            break;

                        case ("Shower"):

                            IsDragShower = true;
                            BathRoomElement_Shower.Manage(true);
                            BathRoomElement_Shower.ManageOutline(false);
                            Offset = MousePosition.y - Shower.position.y;
                            break;

                        case ("Sponge"):

                            IsDragSponge = true;
                            BathRoomElement_Sponge.ManageOutline(false);
                            break;
                    }
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (!Stop)
            {
                switch (Element)
                {
                    case ("Door"):

                        IsDragDoor = false;
                        Door.transform.position = DoorOpen_Point.position;
                        break;

                    case ("Shower"):

                        IsDragShower = false;
                        Shower.transform.position = Shower_Point.position;
                        if (CountSoapBubbles > 0)
                        {
                            BathRoomElement_Shower.ManageOutline(true);
                        }
                        BathRoomElement_Shower.Manage(false);
                        break;

                    case ("Sponge"):

                        IsDragSponge = false;
                        Sponge.transform.position = Sponge_Point.position;
                        if (CountSoapBubbles > 0)
                        {
                            BathRoomElement_Shower.ManageOutline(true);
                        }
                        break;
                }
            }
            Element = null;
        }
        if (IsDragDoor)
        {
            MousePosition = Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            Door.position = new Vector3(Mathf.Clamp(MousePosition.x - Offset, MinClampDoor, MaxClampDoor), Door.position.y, Door.position.z);
            if (Door.position.x - MinClampDoor <= 3f)
            {
                InLoo();
            }
        }
        else if (IsDragSponge)
        {
            MousePosition = Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            Sponge.position = new Vector3(MousePosition.x, MousePosition.y, Sponge.position.z);
        }
        else if (IsDragShower)
        {
            MousePosition = Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            Shower.position = new Vector3(MousePosition.x, Mathf.Clamp(MousePosition.y - Offset, MinClampShower, MaxClampShower), Shower.position.z);
        }
    }
    private void FixedUpdate()
    {
        if (IsDragSponge)
        {
            if (Physics.Raycast(MousePosition - new Vector3(0, 1, 0), Vector3.forward, out Hit, 50, SoapBubbleOFF_Mask))
            {
                Collider[] CC = Physics.OverlapSphere(Hit.point, 0.5f, SoapBubbleOFF_Mask);
                
                for (int i = 0; i < CC.Length; i++)
                {
                    CC[i].GetComponent<SoapBubble>().ManageSoap(true, this);
                }
            }
        }
        else if (IsDragShower)
        {
            /*
            if (Physics.Raycast(new Vector3(Shower.position.x, Shower.position.y, 2) - new Vector3(0, 2.5f, 0), Vector3.down, out Hit, 50, SoapBubbleON_Mask))
            {
                Collider[] CC = Physics.OverlapSphere(Hit.point, 0.5f, SoapBubbleON_Mask);

                for (int i = 0; i < CC.Length; i++)
                {
                    CC[i].GetComponent<SoapBubble>().ManageSoap(false, this);
                }
            }
            */

            Collider[] CC = Physics.OverlapBox(new Vector3(Shower.position.x, Shower.position.y, 2) - new Vector3(0, 6f, 0), new Vector3(0.5f, 2.5f, 5), Quaternion.identity, SoapBubbleON_Mask);

            for (int i = 0; i < CC.Length; i++)
            {
                CC[i].GetComponent<SoapBubble>().ManageSoap(false, this);
            }
        }
    }
    private void InLoo()
    {
        Stop = true;
        IsDragDoor = false;
        Element = null;
        Fade.SetActive(true);
        VoiceRecognitionRepeating.Instance.StopIt();
        Fade.transform.DOScale(new Vector3(2, 2, 2), 1f).OnComplete(() =>
        {
            for (int i = 0; i < HideElementInLoo.Length; i++)
            {
                HideElementInLoo[i].SetActive(false);
            }
            AS.PlayOneShot(Clip_Loo);
            Character.PlayAnimation("Loo");
            Character.transform.position = CactusInLoo_Point.position;
            Door.position = DoorClosed_Point.position;
            Door.localScale = DoorClosed_Point.localScale;
            Fade.transform.DOScale(Vector3.zero, 1f).OnComplete(() => { Invoke(nameof(OutLoo), Clip_Loo.length); Fade.SetActive(false); });
        });
    }
    private void OutLoo()
    {
        Fade.SetActive(true);
        AS.PlayOneShot(Clip_ToiletFlush);
        Fade.transform.DOScale(new Vector3(2, 2, 2), 1f).OnComplete(() =>
        {
            for (int i = 0; i < HideElementInLoo.Length; i++)
            {
                HideElementInLoo[i].SetActive(true);
            }
            Character.PlayAnimation("Idle");
            Character.transform.position = Cactus_Point.position;
            Door.position = DoorOpen_Point.position;
            Door.localScale = DoorOpen_Point.localScale;
            BathRoomElement_Sponge.ManageOutline(true);
            Fade.transform.DOScale(Vector3.zero, 2f).OnComplete(() =>
            {
                Stop = false; VoiceRecognitionRepeating.Instance.CanRecord = true;
                VoiceRecognitionRepeating.Instance.Record(); Fade.SetActive(false);
            });
        });
    }
}
