using UnityEngine;
using TMPro; 

public class ProgressoFase : MonoBehaviour
{
    public static ProgressoFase Instance;

    public int bossesNecessarios = 1;
    public TextMeshProUGUI textoContador;

    private int bossesDerrotados = 0;

    public bool Completo => bossesDerrotados >= bossesNecessarios;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        AtualizarUI();
    }

    public void RegistrarBossDerrotado()
    {
        bossesDerrotados++;
        AtualizarUI();
    }

    void AtualizarUI()
    {
        if (textoContador != null)
            textoContador.text = bossesDerrotados + "/" + bossesNecessarios;
    }
}