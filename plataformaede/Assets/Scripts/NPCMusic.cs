using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NPCMusic : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;           // Jogador
    public AudioClip musicaNPC;        // Música que o NPC vai tocar

    [Header("Configurações de áudio")]
    public float distanciaMax = 5f;    // Distância máxima para ouvir a música
    public float fadeSpeed = 1f;       // Velocidade de fade in/out

    private AudioSource audioSource;

    void Start()
    {
        // Pega o AudioSource no NPC
        audioSource = GetComponent<AudioSource>();

        // Configura o AudioSource
        audioSource.clip = musicaNPC;
        audioSource.loop = true;
        audioSource.spatialBlend = 1f;  // Som 3D
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = distanciaMax;
        audioSource.volume = 0f;        // Começa silencioso

        audioSource.Play();             // Começa tocando (volume 0)
    }

    void Update()
    {
        if (player == null) return;

        float distancia = Vector3.Distance(transform.position, player.position);

        // Faz fade in/out baseado na distância
        if (distancia <= distanciaMax)
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, 1f, fadeSpeed * Time.deltaTime);
        }
        else
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0f, fadeSpeed * Time.deltaTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distanciaMax);
    }
}