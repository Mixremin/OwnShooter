using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    //Переменные
    public float X, Y;

    public Transform orientation;

    float xRot , yRot;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //Лочим курсор в центре
        Cursor.visible = false; //Делаем его невидимым

    }

    private void Update()
    {
        //Получаем данные от мыши
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * X;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * Y;

        yRot += mouseX;
        xRot -= mouseY;
        //Лочим чтобы не поднималось выше или ниже 90 градусов
        xRot = Mathf.Clamp(xRot, -90, 90);
        //Обеспечиваем вращение модельки
        transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        orientation.rotation = Quaternion.Euler(0, yRot, 0);
        
    }
}
