using UnityEngine;
using TMPro;

public class MoedaManager : MonoBehaviour
{
    public static MoedaManager Instance;

    public int totalMoedas = 0;
    public TextMeshProUGUI textoMoedas;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AtualizarUI();
    }

    public void AdicionarMoeda()
    {
        totalMoedas++;
        AtualizarUI();
    }

    void AtualizarUI()
    {
        if (textoMoedas != null)
            textoMoedas.text = totalMoedas.ToString(); // só o número, sem "Moedas: "
    }
}