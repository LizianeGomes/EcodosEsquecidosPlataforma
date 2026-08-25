using UnityEngine;

public class MouseParticleTrail : MonoBehaviour
{
    public ParticleSystem particulas;

    [Header("Configuração")]
    public float intervalo = 0.08f;

    private float contador;

    void Update()
    {
        Vector3 mouse = Input.mousePosition;

        mouse.z = -Camera.main.transform.position.z;

        Vector3 posicaoMundo =
            Camera.main.ScreenToWorldPoint(mouse);

        transform.position = posicaoMundo;

        contador += Time.deltaTime;

        if (contador >= intervalo)
        {
            particulas.Emit(1);
            contador = 0f;
        }
    }
}