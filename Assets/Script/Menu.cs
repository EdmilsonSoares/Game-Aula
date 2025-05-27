/* Script para tranasicionar entre as cenas pelo botão. No inspector do botão
 * na opção OnClick() adicione esse script e selecione a função CarregarCenas,
 * depois no campo de string coloque o nome da cena que deseja carregar */
using UnityEngine;

public class Menu : MonoBehaviour
{
    public void CarregarComCenaLoading(string cena)
    {
        GameGerenciador.Instance.CarregarComCenaLoading(cena); // Cena adicionada na opção OnClick() dos botões
    }

    public void Carregar(string cena)
    {
        GameGerenciador.Instance.Carregar(cena); // Cena adicionada na opção OnClick() dos botões 
    }

    public void Sair()
    {
        Application.Quit();
    }
}
