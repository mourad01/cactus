using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bloc : MonoBehaviour
{
    [Header("GameObject")]
    public GameObject FX_Bloc;

    [Header("String")]
    private string Direct;

    [Header("Float")]
    public float Speed;

    [Header("Vector")]
    private Vector3 Direction;

    [Header("Bool")]
    private bool Stop;

    [Header("Rigidbody")]
    private Rigidbody RB;

    [Header("Script")]
    private MainCamera MainCamera;

    private void Awake()
    {
        RB = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (!Stop)
        {
            transform.position = Vector3.MoveTowards(transform.position, Direction, Speed * Time.deltaTime); 
            Speed += Time.deltaTime;
        }
    }
    public void SetDistination(string Distination, float Speed, Color Color, Vector3 Scale)
    {
        this.Speed += Speed;
        Direction = new Vector3(0, transform.position.y, transform.position.z);
        GetComponent<MeshRenderer>().material.color = Color;
        transform.localScale = new Vector3(Scale.x, transform.localScale.y, transform.localScale.z);
        Direct = Distination;
    }
    public void ManageDetecter(string Value, Character Player)
    {
        switch (Value)
        {
            case ("Win"):

                if (transform.position.x == Mathf.Clamp(transform.position.x, -0.02f, 0.02f))
                {
                    GameObject Object = Instantiate(FX_Bloc, transform.position, Quaternion.identity);
                    Object.transform.localScale = new Vector3(transform.localScale.x * 1.375f, 1.05f, transform.localScale.z * 1.375f);
                    MiniGame.Instance.AddBloc(this);
                }
                else
                {
                    MiniGame.Instance.CleahBloc();
                }
                MiniGame.Instance.AddScore();
                break;

            case ("Lose"):

                Player.Death(Direct);
                break;
        }
        Stop = true;
        Destroy(RB);
        gameObject.layer = 0;
    }
    public void Flash()
    {
        Color DefaultColor = GetComponent<MeshRenderer>().material.color;
        GetComponent<MeshRenderer>().material.color = Color.white;
        GetComponent<MeshRenderer>().material.DOColor(DefaultColor, 0.5f);
    }
}
