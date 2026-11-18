using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneCastelo : MonoBehaviour
{
    [Header("Configurações")]
    public string nomeDaCena = "CutsceneCastelo";

    private bool cutsceneIniciada = false;

    private void OnTriggerEnter2D(Collider2D outro)
    {
        if (cutsceneIniciada)
            return;

        if (outro.CompareTag("Player"))
        {
            if (ProgressoFase.Instance != null &&
                ProgressoFase.Instance.Completo)
            {
                cutsceneIniciada = true;
                SceneManager.LoadScene(nomeDaCena);
            }
        }
    }

}