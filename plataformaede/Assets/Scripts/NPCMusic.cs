using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NPCMusic : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;

    [Header("Configurações")]
    public float distanciaMax = 5f;
    public float fadeSpeed = 1f;

    private AudioSource audioSource;
    private bool tocando = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 🔥 IMPORTANTE: SOM 2D
        audioSource.volume = 0f;
    }

    void Update()
    {
        if (player == null) return;

        float distancia = Vector2.Distance(transform.position, player.position);

        if (distancia <= distanciaMax)
        {
            if (!tocando)
            {
                audioSource.Play();
                tocando = true;
            }

            audioSource.volume = Mathf.MoveTowards(
                audioSource.volume,
                1f,
                fadeSpeed * Time.deltaTime
            );
        }
        else
        {
            audioSource.volume = Mathf.MoveTowards(
                audioSource.volume,
                0f,
                fadeSpeed * Time.deltaTime
            );

            if (audioSource.volume <= 0.01f && tocando)
            {
                audioSource.Stop();
                tocando = false;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distanciaMax);
    }
}