using UnityEngine;

public class MenuPrincipal : MonoBehaviour
{
    public FadeController fade;

    public void Jogar()
    {
        fade.CarregarCena("SampleScene");
    }
}