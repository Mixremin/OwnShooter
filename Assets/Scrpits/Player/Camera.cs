using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    //����������
    public float X, Y;

    public Transform orientation;

    float xRot , yRot;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //����� ������ � ������
        Cursor.visible = false; //������ ��� ���������

    }

    private void Update()
    {
        //�������� ������ �� ����
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * X;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * Y;

        yRot += mouseX;
        xRot -= mouseY;
        //����� ����� �� ����������� ���� ��� ���� 90 ��������
        xRot = Mathf.Clamp(xRot, -90, 90);
        //������������ �������� ��������
        transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        orientation.rotation = Quaternion.Euler(0, yRot, 0);
        
    }
}
