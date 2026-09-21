using UnityEngine;
using System.Collections;

public class InimigoController : MonoBehaviour
{
    private bool morto = false;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void MorrerPisado()
    {
        if (morto) return;

        morto = true;

        // Desativa colisores e movimento
        GetComponent<EnemyAI>().enabled = false;

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
            col.enabled = false;

        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        GetComponent<Rigidbody2D>().simulated = false;

        anim.SetTrigger("morto");

        StartCoroutine(DestruirAposAnimacao());
    }

    IEnumerator DestruirAposAnimacao()
    {
        yield return new WaitForSeconds(0.6f); // ajuste pro tempo real da sua animação de morte
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