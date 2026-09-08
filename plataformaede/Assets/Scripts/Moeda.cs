using UnityEngine;
 
 public class Moeda : MonoBehaviour
 {
     private void OnTriggerEnter2D(Collider2D other)
     {
         if (other.CompareTag("Player"))
         {
             Debug.Log("Moeda coletada!");
 
             Destroy(gameObject);
         }
     }
 }