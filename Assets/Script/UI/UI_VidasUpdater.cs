using UnityEngine;
using TMPro;

public class UI_VidasUpdater : MonoBehaviour
{
    private TMP_Text textVida;

    void Awake()
    {
        textVida = GetComponent<TMP_Text>();
        if (textVida == null)
        {
            Debug.LogError("VidasUIUpdater precisa de um componente TMP_Text no mesmo GameObject!", this);
            enabled = false; // Desativa o script se não houver TMP_Text
        }
    }

    void OnEnable() // Chamado quando o GameObject é ativado ou a cena é carregada
    {
        UpdateVidasUI();
    }

    public void UpdateVidasUI()
    {
        if (GameGerenciador.Instance != null )
        {
            textVida.text = GameGerenciador.Instance.currentPlayerLives.ToString();
        }
        else
        {
            Debug.LogWarning("GameGerenciador não encontrados para UI_VidasUpdater.", this);
        }
    }
}