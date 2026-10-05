using UnityEngine;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup;
    public float duracaoFade = 1f;

    void Start()
    {
        fadeCanvasGroup.alpha = 1f; 
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        float tempo = 0f;

        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, tempo / duracaoFade);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
    }
}