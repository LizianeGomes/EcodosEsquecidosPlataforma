using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    [Header("Partículas")]
    [Tooltip("Particle System que toca quando o checkpoint é ativado. Se vazio, busca automaticamente nos filhos.")]
    [SerializeField] private ParticleSystem checkpointParticles;

    private bool isActivated = false;

    private void Awake()
    {
        if (checkpointParticles == null)
        {
            checkpointParticles = GetComponentInChildren<ParticleSystem>();
        }

        if (checkpointParticles == null)
        {
            Debug.LogWarning($"[Checkpoint] Nenhum Particle System encontrado em '{gameObject.name}'. Atribua um no Inspector ou adicione como filho.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated) return;

        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                player.SetCheckpoint(transform);
                Debug.Log("Checkpoint salvo!");

                isActivated = true;

                if (checkpointParticles != null)
                {
                    checkpointParticles.Play();
                }
            }
        }
    }
}