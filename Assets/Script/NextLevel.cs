//script para mudar de cena quando jogador colidir com o final (gate)
using UnityEngine;

public class NextLevel : MonoBehaviour
{
    [SerializeField] private string proximaCena;

    private void OnTriggerEnter2D(Collider2D colidiu)
    {
        if (colidiu.gameObject.CompareTag("Player"))
        {
            ProximoNivel();
        }
    }

    public void ProximoNivel()
    {
        GameGerenciador.Instance.Carregar(proximaCena);
    }
}
