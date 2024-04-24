using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{

    [SerializeField] Text DeadText;

    private bool isRestart;

    private float alpha;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        alpha = 1;
        DeadText.gameObject.SetActive(false);
    }

    public IEnumerator Fade(bool toVisible)
    {
        float step = toVisible ? 0.1f : -0.1f;
        int endValue = toVisible ? 1 : 0;

        while (alpha != endValue)
        {
            alpha += step;
            canvasGroup.alpha = alpha;

            if (alpha < 0)
            {
                alpha = 0;
            }
            else if (alpha > 1)
            {
                alpha = 1;
            }

            yield return new WaitForSeconds(0.05f);
        }
        
    }
    public IEnumerator BlinkinText()
    {
        DeadText.gameObject.SetActive(true);
        while(!isRestart)
        {
            DeadText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.2f);

            DeadText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);
        }
    }
}
