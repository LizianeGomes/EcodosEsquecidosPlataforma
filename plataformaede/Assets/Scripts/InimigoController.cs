using UnityEngine;
using System.Collections;

public class InimigoController : MonoBehaviour
{
    private bool morto = false;
    private Animator anim;

    [Header("Som de morte")]
    [SerializeField] private AudioClip somMorte;
    [Range(0f, 10f)]
    [SerializeField] private float volumeMorte = 1f;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();

        Debug.Log("Animator encontrado em: " + anim.gameObject.name +
                  " | controller: " + anim.runtimeAnimatorController);
    }

    public void MorrerPisado()
    {
        Debug.Log("MorrerPisado chamado");

        if (morto) return;

        morto = true;

        // Reproduz o som apenas uma vez, ao morrer.
        if (somMorte != null)
        {
            AudioSource.PlayClipAtPoint(
                somMorte,
                transform.position,
                volumeMorte
            );
        }

        // Desativa colisores e movimento.
        EnemyAI2 inimigo = GetComponent<EnemyAI2>();
        if (inimigo != null)
            inimigo.enabled = false;

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

        foreach (Collider2D col in colliders)
            col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        anim.SetTrigger("morto");

        StartCoroutine(DestruirAposAnimacao());
    }

    IEnumerator DestruirAposAnimacao()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (morto) return;

        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
                player.PerderVidaDireto();
        }
    }
}