using UnityEngine;
using UnityEngine.SceneManagement;

public class CenaInicial : MonoBehaviour
{
    void Start()
    {
        // Após o Manager ser inicializado, carrega a primeira cena do jogo
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        // Opcional: Desativar ou destruir este script após carregar a primeira cena
        Destroy(gameObject);
    }
}