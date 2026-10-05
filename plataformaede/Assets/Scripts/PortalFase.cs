using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PortalFase : MonoBehaviour
{
    [Header("Destino")]
    public string cenaDestino = "Fase2";

    [Header("Fade")]
    public CanvasGroup fadeCanvasGroup;
    public float duracaoFade = 1f;

    private bool ativado = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ativado) return;
        if (!other.CompareTag("Player")) return;

        ativado = true;
        StartCoroutine(TransicaoFase());
    }

    IEnumerator TransicaoFase()
    {
        yield return StartCoroutine(Fade(0f, 1f));
        SceneManager.LoadScene(cenaDestino);
    }

    IEnumerator Fade(float de, float para)
    {
        float tempo = 0f;
        fadeCanvasGroup.blocksRaycasts = true;

        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(de, para, tempo / duracaoFade);
            yield return null;
        }

        fadeCanvasGroup.alpha = para;
    }
}