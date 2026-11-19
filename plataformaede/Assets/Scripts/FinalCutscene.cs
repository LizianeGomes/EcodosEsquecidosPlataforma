using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class FinalCutscene : MonoBehaviour
{
    [Header("Referências")]
    public VideoPlayer videoPlayer;

    [Header("Cena do Menu")]
    public string nomeCenaMenu = "Menu";

    [Header("Instrução para pular")]
    public TMP_Text textoPular;

    [Header("Configuração do texto")]
    public float atrasoParaAparecer = 2f;
    public float duracaoTexto = 3f;

    private bool videoFinalizado = false;

    private void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
        {
            Debug.LogError("Video Player não encontrado!");
            return;
        }

        videoPlayer.loopPointReached += AoTerminarVideo;

        if (textoPular != null)
        {
            textoPular.gameObject.SetActive(false);
            StartCoroutine(ControlarTexto());
        }

        videoPlayer.Play();
    }

    private void Update()
    {
        if (videoFinalizado)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FinalizarCutscene();
        }
    }

    private IEnumerator ControlarTexto()
    {
        yield return new WaitForSecondsRealtime(atrasoParaAparecer);

        if (videoFinalizado)
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
        if (videoFinalizado)
            return;

        videoFinalizado = true;

        SceneManager.LoadScene(nomeCenaMenu);
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= AoTerminarVideo;
    }
}