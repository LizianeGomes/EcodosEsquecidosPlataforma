using UnityEngine;

public class MouseParticles : MonoBehaviour
{
    public ParticleSystem particulas;

    private RectTransform rectTransform;
    private Canvas canvas;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out Vector2 posicaoMouse
        );

        rectTransform.localPosition = posicaoMouse;
    }
}