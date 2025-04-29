/* Script para tranasicionar entre as cenas pelo botão. No inspector do botão
 * na opção OnClick() adicione esse script e selecione a função CarregarCenas,
 * depois no campo de string coloque o nome da cena que deseja carregar */
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void CarregarCenas(string cena){
        //SceneManager.LoadScene(cena, LoadSceneMode.Single);
        CenaLoadGerenciador.Instance.Carregar(cena);
    }

    public void Sair(){
        Application.Quit();
    }
}
