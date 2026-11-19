
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("Vídeo")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Próxima cena")]
    [SerializeField] private string proximaCena = "SampleScene";

    [Header("Instrução para pular")]
    [SerializeField] private TMP_Text textoPular;

    [Header("Configuração do texto")]
    [SerializeField] private float atrasoParaAparecer = 2f;
    [SerializeField] private float duracaoTexto = 3f;

    private bool cutsceneFinalizada = false;

    void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
        {
            Debug.LogError("Video Player não encontrado!");
            return;
        }

        videoPlayer.isLooping = false;
        videoPlayer.playOnAwake = false;
        videoPlayer.loopPointReached += AoTerminarVideo;

        if (textoPular != null)
        {
            textoPular.gameObject.SetActive(false);
            StartCoroutine(ControlarTexto());
        }

        videoPlayer.Play();
    }

    void Update()
    {
        if (cutsceneFinalizada)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            FinalizarCutscene();
    }

    private IEnumerator ControlarTexto()
    {
        yield return new WaitForSecondsRealtime(atrasoParaAparecer);

        if (cutsceneFinalizada)
            yield break;

        textoPular.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(duracaoTexto);

        textoPular.gameObject.SetActive(false);
    }

    private void AoTerminarVideo(VideoPlayer vp)
    {
        FinalizarCutscene();
    }

    private void FinalizarCutscene()
    {
        if (cutsceneFinalizada)
            return;

        cutsceneFinalizada = true;
        SceneManager.LoadScene(proximaCena);
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= AoTerminarVideo;
    }
}
