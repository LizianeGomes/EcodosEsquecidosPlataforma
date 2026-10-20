using System;
using UnityEngine;

public class MiniInimigo2 : MonoBehaviour
{
    [SerializeField] private Transform player;
    [Range(0, 10)]
    [SerializeField] private float distanciaDetecção;
    
    [Header("Pulo")]
    [SerializeField] private float forçaHorizontal;
    [SerializeField] private float forçaVertical;
    [SerializeField] private float intervalo;
    
    private Rigidbody2D rb;
    private float cronometroPulo;
    private bool estaNoChao = true;
    private Animator anim;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
    }

    
    void Update()
    {
        if (player == null) return;
        
        
        float distancia = Vector2.Distance(player.position, transform.position);

        if (estaNoChao && distancia <= distanciaDetecção)
        {
            anim.Play("Caminhada");
            cronometroPulo += Time.deltaTime;

            if (cronometroPulo >= intervalo)
            {
                PularNoPlayer();
                cronometroPulo = 0f;
            }
        }
    }

    void PularNoPlayer()
    {
        float direcaoHorizontal = Mathf.Sign(
            player.position.x - transform.position.x
        );

        Vector2 forcaPulo = new Vector2(
            direcaoHorizontal * forçaHorizontal,
            forçaVertical
        );

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        rb.AddForce(forcaPulo, ForceMode2D.Impulse);

        estaNoChao = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Chao"))
            estaNoChao = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawWireSphere(transform.position, distanciaDetecção);



    }
}
