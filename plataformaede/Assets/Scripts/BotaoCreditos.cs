using UnityEngine;
using UnityEngine.SceneManagement;

public class BotaoCreditos : MonoBehaviour
{
    public void AbrirCreditos()
    {
        SceneManager.LoadScene("Creditos");
    }

    public void VoltarAoMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void SairDoJogo()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}