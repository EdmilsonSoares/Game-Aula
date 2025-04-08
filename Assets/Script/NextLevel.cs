//script para mudar de cena quando jogador colidir com o final
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    [SerializeField] private string proximaCena;

    private void OnTriggerEnter2D(Collider2D colidiu)
    {
        if(colidiu.gameObject.CompareTag("Player")){
            ProximoNivel();
        }
    }

    public void ProximoNivel(){
        SceneManager.LoadScene(this.proximaCena);
    }
}
