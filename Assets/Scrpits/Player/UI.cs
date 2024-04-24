using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI SpeedText;
    public Transform Player; 

    public float playerSpeed;

    private void Start()
    {
        
    }
    void Update()
    {
        playerSpeed = Player.GetComponent<Rigidbody>().velocity.magnitude;
        SpeedText.text ="Speed:"+ playerSpeed.ToString();

    }
}
