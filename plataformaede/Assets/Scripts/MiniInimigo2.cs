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
    [SerializeField] private GameObject limiteEsquerda;
    [SerializeField] private GameObject limiteDireita;

    [Header("Animação")]
    [SerializeField] private string nomeEstadoPulo = "Caminhada"; 

    private Rigidbody2D rb;
    private Animator anim;
    private float cronometroPulo;
    private bool estaNoChao = true;
    private bool preparandoPulo = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        PoseParado();
    }

    void Update()
    {
        if (player == null) return;

        float distancia = Vector2.Distance(player.position, transform.position);
        

        if (estaNoChao && !preparandoPulo && distancia <= distanciaDetecção)
        {
            cronometroPulo += Time.deltaTime;

            if (cronometroPulo >= intervalo)
            {
                cronometroPulo = 0f;
                preparandoPulo = true;

                
                anim.speed = 1f;
                anim.Play(nomeEstadoPulo, 0, 0f);
            }
        }
    }

    
    public void ExecutarPulo()
    {
        if (!preparandoPulo) return;

        
        float direcaoHorizontal = Mathf.Sign(player.position.x - transform.position.x);

        Vector2 forcaPulo = new Vector2(
            direcaoHorizontal * forçaHorizontal,
            forçaVertical
        );

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        rb.AddForce(forcaPulo, ForceMode2D.Impulse);

        estaNoChao = false;
        preparandoPulo = false;
    }

    
    void PoseParado()
    {
        anim.Play(nomeEstadoPulo, 0, 0f);
        anim.Update(0f);
        anim.speed = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        PlayerMovement jogador = collision.collider.GetComponentInParent<PlayerMovement>();

        if (jogador != null)
        {
            jogador.PerderCoracaoNoLocal();
            return;
        }

        if (!collision.gameObject.CompareTag("Chao")) return;
        if (!estaNoChao)
        {
            estaNoChao = true;
            PoseParado();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player =
            other.GetComponentInParent<PlayerMovement>();

        if (other.CompareTag("Player"))
        {
            player.TomarDano(1);
            
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawWireSphere(transform.position, distanciaDetecção);
    }
}