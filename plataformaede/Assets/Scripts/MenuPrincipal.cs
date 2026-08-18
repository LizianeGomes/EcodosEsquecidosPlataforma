using UnityEngine;

public class MenuPrincipal : MonoBehaviour
{
    public FadeController fade;
    
    public void Jogar()
    {
        Debug.Log("BOTAO JOGAR FUNCIONOU!");
        fade.CarregarCena("SampleScene");
    }
}