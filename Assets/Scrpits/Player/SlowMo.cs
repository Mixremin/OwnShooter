using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMo : MonoBehaviour
{
    public float HowSlow;
    [SerializeField]private bool Slowed;

    // Update is called once per frame

    private void Start()
    {
        Slowed = false;
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Z) && !(Slowed))
        {
            SlowOn();
        }
        else if (Input.GetKey(KeyCode.Z) && Slowed)
        {
            SlowOff();
        }
    }
    private void SlowOn()
    {
        Slowed = true;
        Time.timeScale = HowSlow;
        Time.fixedDeltaTime = Time.time + 0.015f;
    }

    private void SlowOff()
    {
        Time.timeScale = 1;
        Slowed = false;
    }
}

 
