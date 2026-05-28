using UnityEngine;
using UnityEngine.UI;

public class VidaUI : MonoBehaviour
{
    public Image[] coracoes;

    public void AtualizarVida(int vidaAtual)
    {
        for (int i = 0; i < coracoes.Length; i++)
        {
            coracoes[i].enabled = i < vidaAtual;
        }
    }
}