using UnityEngine;

public class Projetil : MonoBehaviour
{
    public int dano = 1;
    public float tempoVida = 5f;

    [Header("Crescimento")]
    public float escalaInicial = 0.2f;
    public float escalaFinal = 1.5f;
    public float duracaoCrescimento = 1.5f; // segundos até chegar no tamanho final

    private bool jaAtingiu = false;
    private float tempoCrescimento = 0f;
    private float sinalX = 1f;

    void Start()
    {
        // guarda pra qual lado o projétil está virado, caso o boss inverta o X
        sinalX = Mathf.Sign(transform.localScale.x);
        AplicarEscala(escalaInicial);

        Destroy(gameObject, tempoVida);
    }

    void Update()
    {
        if (tempoCrescimento >= duracaoCrescimento)
            return;

        tempoCrescimento += Time.deltaTime;
        float t = Mathf.Clamp01(tempoCrescimento / duracaoCrescimento);

        AplicarEscala(Mathf.Lerp(escalaInicial, escalaFinal, t));
    }

    void AplicarEscala(float escala)
    {
        transform.localScale = new Vector3(escala * sinalX, escala, 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (jaAtingiu)
            return;

        PlayerMovement player =
            other.GetComponentInParent<PlayerMovement>();

        if (player != null)
        {
            jaAtingiu = true;

            player.TomarDano(dano);

            Destroy(gameObject);
        }
    }
}