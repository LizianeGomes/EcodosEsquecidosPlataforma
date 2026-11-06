using UnityEngine;

public class Moeda : MonoBehaviour
{
    [Header("Som da coleta")]
    public AudioClip somColeta;

    [Range(0f, 1f)]
    public float volumeColeta = 0.7f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Moeda coletada!");

            MoedaManager.Instance.AdicionarMoeda();

            AudioSource audioPlayer =
                other.GetComponentInParent<AudioSource>();

            if (audioPlayer != null && somColeta != null)
            {
                audioPlayer.PlayOneShot(
                    somColeta,
                    volumeColeta
                );
            }
            else
            {
                Debug.LogWarning("Audio Source ou som da moeda não encontrado!");
            }

            Destroy(gameObject);
        }
    }
}