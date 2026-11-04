using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SacoDePancadas : MonoBehaviour, IDanificavel
{
    [Header("Vida")]
    [SerializeField] private int golpesParaDestruir = 3;
    [SerializeField] private float intervaloMinimoEntreGolpes = 0.1f; // evita contar 2x o mesmo golpe

    [Header("Tremor")]
    [SerializeField] private Transform visual;          // o sprite que treme
    [SerializeField] private float duracaoTremor = 0.25f;
    [SerializeField] private float intensidadeTremor = 0.12f;
    [SerializeField] private float frequenciaTremor = 40f;

    [Header("Sumir")]
    [SerializeField] private float duracaoFade = 0.3f;

    [Header("Eventos")]
    public UnityEvent aoDestruir; // opcional: abrir porta, mostrar próximo texto do tutorial...

    private int golpesRecebidos;
    private bool destruido;
    private float ultimoGolpe = -999f;
    private Vector3 posicaoOriginal;
    private Coroutine tremorAtual;
    private SpriteRenderer[] sprites;

    void Awake()
    {
        if (visual == null)
            visual = transform;

        posicaoOriginal = visual.localPosition;
        sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    public void ReceberDano(int dano)
    {
        if (destruido) return;
        if (Time.time - ultimoGolpe < intervaloMinimoEntreGolpes) return;

        ultimoGolpe = Time.time;
        golpesRecebidos++;

        // reinicia o tremor a cada golpe
        if (tremorAtual != null)
            StopCoroutine(tremorAtual);

        visual.localPosition = posicaoOriginal;
        tremorAtual = StartCoroutine(Tremer());

        if (golpesRecebidos >= golpesParaDestruir)
            StartCoroutine(Destruir());
    }

    private IEnumerator Tremer()
    {
        float t = 0f;

        while (t < duracaoTremor)
        {
            t += Time.deltaTime;

            // oscila e vai diminuindo até parar
            float decaimento = 1f - Mathf.Clamp01(t / duracaoTremor);
            float x = Mathf.Sin(t * frequenciaTremor) * intensidadeTremor * decaimento;

            visual.localPosition = posicaoOriginal + new Vector3(x, 0f, 0f);
            yield return null;
        }

        visual.localPosition = posicaoOriginal;
    }

    private IEnumerator Destruir()
    {
        destruido = true;

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        // deixa o último tremor terminar antes de sumir
        yield return new WaitForSeconds(duracaoTremor);

        float t = 0f;

        while (t < duracaoFade)
        {
            t += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(t / duracaoFade);

            foreach (SpriteRenderer sr in sprites)
            {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }

            yield return null;
        }

        aoDestruir?.Invoke();
        Destroy(gameObject);
    }
}