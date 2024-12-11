using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController cameraController;

    public float speed;
    public float defSpeed;
    private Vector3 defPos;
    private void Awake()
    {
        cameraController = this;
        defPos = transform.position;
        defSpeed = speed;
    }

    void Update()
    {
        Vector3 dirHorizontal = new Vector3(GameManager.gameManager.inputActions.Player.Move.ReadValue<Vector2>().x, 0, 0);
        Vector3 dirVertical = new Vector3(GameManager.gameManager.inputActions.Player.Move.ReadValue<Vector2>().y, 0, GameManager.gameManager.inputActions.Player.Move.ReadValue<Vector2>().y);
        dirHorizontal.Normalize();
        dirVertical.Normalize();

        if ((dirHorizontal.x <0 && transform.position.x > -30) || (dirHorizontal.x >0 && transform.position.x < 30))
        {
            transform.localPosition +=  transform.TransformDirection(dirHorizontal * Time.deltaTime * speed);    
        }

        if ((dirVertical.x <0 && transform.position.x > -30) || (dirVertical.x >0 && transform.position.x < 30))
        {
            transform.localPosition +=  dirVertical * Time.deltaTime * speed;
        }
        
        if (GameManager.gameManager.inputActions.Player.Tab.IsPressed())
        {
            transform.localPosition = defPos;
        }
    }
}
