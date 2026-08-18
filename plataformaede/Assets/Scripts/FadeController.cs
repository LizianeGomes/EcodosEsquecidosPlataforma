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

    private IEnumerator FadeOut(string nomeCena)
    {
        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;

            float alpha = Mathf.Lerp(0f, 1f, tempo / duracao);

            Color cor = fadeImage.color;
            cor.a = alpha;
            fadeImage.color = cor;

            yield return null;
        }

        SceneManager.LoadScene(nomeCena);
    }
}