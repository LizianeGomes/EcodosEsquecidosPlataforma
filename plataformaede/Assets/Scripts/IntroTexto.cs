using UnityEngine;

public class IntroTexto : MonoBehaviour
{
    public float tempoVisivel = 6f; // tempo que o texto fica na tela

    void Start()
    {
        Invoke(nameof(EsconderTexto), tempoVisivel);
    }

    void EsconderTexto()
    {
        gameObject.SetActive(false);
    }
}