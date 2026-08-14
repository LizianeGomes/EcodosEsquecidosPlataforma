using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public Image fadeImage;
    public float duracao = 1f;

    public void CarregarCena(string nomeCena)
    {
        StartCoroutine(FadeOut(nomeCena));
    }

    IEnumerator FadeOut(string nomeCena)
    {
        float tempo = 0f;
        Color cor = fadeImage.color;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;

            float alpha = Mathf.Lerp(0f, 1f, tempo / duracao);

            fadeImage.color = new Color(
                cor.r,
                cor.g,
                cor.b,
                alpha
            );

            yield return null;
        }

        SceneManager.LoadScene(nomeCena);
    }
}