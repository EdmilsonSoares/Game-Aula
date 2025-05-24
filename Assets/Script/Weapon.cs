// Anexe esse script a arma
using System.Collections; // Não é estritamente necessário aqui, mas bom manter se usar corrotinas
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private List<GameObject> projectilList; // A lista de todos os prefabs de projéteis
    private GameObject chosenProjectil; // O prefab de projétil atualmente selecionado
    private int projectilIndex = 0; // Índice do projétil atualmente selecionado na lista
    [SerializeField] private Transform playerTransform;


    void Start()
    {
        chosenProjectil = projectilList[0];
    }

    void Update()
    {
        Shooting();
        ProjectileSwitching(); // Nova função para trocar de projétil
    }

    // NOVA LÓGICA: Trocar de projétil
    private void ProjectileSwitching()
    {
        if (Input.GetButtonDown("Fire2")) // Configurado para botão Q
        {
            if (projectilList == null || projectilList.Count == 0)
            {
                Debug.LogWarning("Não há projéteis para trocar na lista 'projectileTypes'.");
                return;
            }

            // Incrementa o índice
            projectilIndex++;

            // Se o índice exceder o número de projéteis, volta para o primeiro
            if (projectilIndex >= projectilList.Count)
            {
                projectilIndex = 0;
            }

            // Define o novo projétil atual
            chosenProjectil = projectilList[projectilIndex];
            Debug.Log($"Tipo de projétil alterado para: {chosenProjectil.name}");
            // Opcional: Você pode querer exibir uma UI ou um som para indicar a troca
        }
    }

    private void Shooting()
    {
        if (Input.GetButtonDown("Fire1"))// Configurado para botão K
        {
            if (chosenProjectil != null)
            {
                Fire(chosenProjectil); // Atira com o projétil atualmente selecionado
            }
            else
            {
                Debug.LogWarning("Nenhum projétil selecionado para atirar!");
            }
        }
    }

    private void Fire(GameObject projectile)
    {
        // Pega o sinal da escala X do player para determinar a direção
        int direcaoPlayer = (int)Mathf.Sign(playerTransform.localScale.x); // Retorna 1 ou -1
        Quaternion projectilRotation;
        // Se o sprite do seu projétil APONTA PARA A DIREITA por padrão:
        if (direcaoPlayer > 0)
        {
            projectilRotation = Quaternion.Euler(0, 0, 0); // Sem rotação, aponta para a direita
        }
        else // direcaoPlayer < 0 (para a esquerda)
        {
            projectilRotation = Quaternion.Euler(0, 180, 0); // Vira 180 graus no eixo Y, aponta para a esquerda
        }
        var newProjectil = Instantiate(projectile, transform.position, projectilRotation);
        newProjectil.GetComponent<Projectil>().SetDirection(direcaoPlayer);
    }
}
