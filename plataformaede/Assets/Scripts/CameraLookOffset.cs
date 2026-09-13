using UnityEngine;

public class CameraLookOffset : MonoBehaviour
{
    [Header("Configuração do Look Up/Down")]
    [Tooltip("Quanto tempo segurando pra câmera começar a se mover (estilo Sonic).")]
    [SerializeField] private float tempoParaOlhar = 0.5f;

    [Tooltip("Distância que a câmera se desloca verticalmente.")]
    [SerializeField] private float distanciaOffset = 3f;

    [Tooltip("Velocidade da transição do deslocamento.")]
    [SerializeField] private float velocidadeTransicao = 4f;

    private float tempoSegurando = 0f;
    private float offsetAtual = 0f;
    private float offsetAlvo = 0f;
    private Vector3 posicaoLocalBase;

    private void Awake()
    {
        posicaoLocalBase = transform.localPosition;
    }

    private void Update()
    {
        float inputVertical = Input.GetAxisRaw("Vertical");

        if (inputVertical != 0f)
        {
            tempoSegurando += Time.deltaTime;

            if (tempoSegurando >= tempoParaOlhar)
            {
                offsetAlvo = inputVertical > 0 ? distanciaOffset : -distanciaOffset;
            }
        }
        else
        {
            tempoSegurando = 0f;
            offsetAlvo = 0f;
        }

        offsetAtual = Mathf.Lerp(offsetAtual, offsetAlvo, Time.deltaTime * velocidadeTransicao);

        transform.localPosition = posicaoLocalBase + new Vector3(0f, offsetAtual, 0f);
    }
}