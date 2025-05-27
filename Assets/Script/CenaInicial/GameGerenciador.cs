// Singleton que persiste dessde a cena inicial, usado para gerenciar dados entre cenas
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameGerenciador : MonoBehaviour
{
    public static GameGerenciador Instance { get; private set; }

    public string NomeProxCena { get; set; }
    [Header("Configurações do Jogador")]
    [SerializeField] private int playerStartingLives = 3; // Vidas iniciais para um NOVO jogo
    public int currentPlayerLives = 3; // Vidas ATUAIS do jogador

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

        if (currentPlayerLives == 0)
        {
            currentPlayerLives = playerStartingLives;
        }
    }

    public void CarregarComCenaLoading(string sceneName)
    {
        NomeProxCena = sceneName;
        SceneManager.LoadScene("Loading");
    }

    public void Carregar(string sceneName)
    {
        NomeProxCena = sceneName;
        SceneManager.LoadScene(NomeProxCena);
    }

    public void PlayerLoseLife()
    {
        currentPlayerLives--;
        Debug.Log($"Vida perdida! Vidas restantes: {currentPlayerLives}");
        Invoke("Recarregar", 1f);
    }

    private void Recarregar()
    {
        if (currentPlayerLives > 0)
        {
            SceneManager.LoadScene("Gameplay");
        }
        else
        {
            Debug.Log("Game Over! Nenhuma vida restante.");
            SceneManager.LoadScene("Ending");
            ResetGameLives();
        }
    }
    // Método para resetar as vidas para um novo jogo (ex: após game over final ou iniciar um novo jogo do menu)
    public void ResetGameLives()
    {
        currentPlayerLives = playerStartingLives;
    }

}