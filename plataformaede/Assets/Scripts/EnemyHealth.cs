using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int vida = 3;

    public void ReceberDano(int dano)
    {
        vida -= dano;

        Debug.Log("Inimigo tomou dano!");

        if (vida <= 0)
        {
            Destroy(gameObject);
        }
    }
}