// Anexe esse script a arma
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : MonoBehaviour
{
    // Evento que outros scripts podem assinar para saber qual projétil foi escolhido
    public static event Action<GameObject> OnProjectileChanged;
    [SerializeField] private List<GameObject> projectilList; // A lista de todos os prefabs de projéteis
    private GameObject chosenProjectil; // O prefab de projétil atualmente selecionado
    private int projectilIndex = 0; // Índice do projétil atualmente selecionado na lista
    [SerializeField] private Transform playerTransform;
    [Header("Configurações de Som")]
    //[SerializeField] private AudioClip sound;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        chosenProjectil = projectilList[0];
        OnProjectileChanged?.Invoke(chosenProjectil);
    }

    void Update()
    {
        Shooting();
        ProjectileSwitching();
    }

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


            //audioSource.clip = sound;
            audioSource.Play();
            // Toca o som na posição da arma (ou player, o que fizer mais sentido)
            //AudioSource.PlayClipAtPoint(switchProjectileSound, transform.position, switchSoundVolume);

            // Opcional: Você pode querer exibir uma UI ou um som para indicar a troca
            OnProjectileChanged?.Invoke(chosenProjectil);
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
