using UnityEngine;

public class LentidaoGosma : MonoBehaviour
{
    public float Multiplicador { get; private set; } = 1f;

    private int gosmasSobrepostas = 0;

    public void EntrarNaGosma(float multiplicador)
    {
        gosmasSobrepostas++;
        Multiplicador = multiplicador;
    }

    public void SairDaGosma()
    {
        gosmasSobrepostas = Mathf.Max(0, gosmasSobrepostas - 1);

        if (gosmasSobrepostas == 0)
            Multiplicador = 1f;
    }
}