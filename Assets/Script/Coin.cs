using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Animator coinAnimator;
    
    void Start()
    {
        coinAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D colidiu)
    {
        if(colidiu.gameObject.CompareTag("Player")){
            // Adiciona a moeda ao jogador (pode ser um sistema de pontuação ou inventário) 
            Player player = colidiu.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.coins += 1;
            }
            coinAnimator.SetTrigger("Collected");
            GetComponent<Collider2D>().enabled = false; // Desativa o collider para evitar múltiplas coletas
            Destroy(gameObject, 0.25f); // Destrói o objeto após 0.25 segundo (tempo para a animação tocar)
        }
    }
}
