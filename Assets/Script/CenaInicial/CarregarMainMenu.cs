// Script apenas para carregar a primeira cena do jogo (MainMenu) sem passar pela animação de Loading
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarregarMainMenu : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single); // Após o Manager ser inicializado, carrega a primeira cena do jogo
        Destroy(gameObject); // Desativar ou destruir este script após carregar a primeira cena
    }
}