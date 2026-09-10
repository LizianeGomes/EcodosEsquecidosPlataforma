using UnityEngine;
using UnityEngine.SceneManagement;

public class BotaoCreditos : MonoBehaviour
{
    public void IrParaCreditos()
    {
        SceneManager.LoadScene("Creditos");
    }
}