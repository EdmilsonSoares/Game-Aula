//Script para tranasicionar entre as cenas pelo botão
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void CarregarCenas(string cena){
        SceneManager.LoadScene(cena, LoadSceneMode.Single);
    }

    public void Sair(){
        Application.Quit();
    }
}
