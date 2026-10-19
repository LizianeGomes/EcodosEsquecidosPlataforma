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
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        if (player == null) return;
        
        float distancia = Vector2.Distance(player.position, transform.position);

        if (estaNoChao && distancia <= distanciaDetecção)
        {
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
        float direcaoHorizontal = Mathf.Sign(player.position.x - transform.position.x);
        Vector3 forcaPulo = new Vector3(direcaoHorizontal * forçaHorizontal, forçaVertical, 0f);
        
        
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
