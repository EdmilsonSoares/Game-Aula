// Singleton para receber o nome da próxima cena a ser carregada e carregar a cena de loading antes
using UnityEngine;
using UnityEngine.SceneManagement;

public class CenaLoadGerenciador : MonoBehaviour
{
    public static CenaLoadGerenciador Instance { get; private set; }
    public string NomeProxCena { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Carregar(string sceneName)
    {
        NomeProxCena = sceneName;
        SceneManager.LoadScene("Loading");
    }
}