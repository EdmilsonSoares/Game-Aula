using UnityEngine;
using UnityEngine.UI; // Importar para Image
using System.Collections.Generic; // Para a lista de Sprites

public class UI_ProjectileIconUpdater : MonoBehaviour
{
    [System.Serializable] // Torna a classe visível no Inspector
    public class ProjectileSpriteMapping
    {
        public GameObject projectilePrefab; // O prefab do projétil (para comparação)
        public Sprite uiSprite;             // A Sprite correspondente para a UI
    }

    [SerializeField] private Image projectileIconImage; // A referência à sua imagem na UI
    [SerializeField] private List<ProjectileSpriteMapping> mappings; // A lista de mapeamentos prefab -> sprite

    void Awake()
    {
        if (projectileIconImage == null)
        {
            projectileIconImage = GetComponent<Image>(); // Tenta pegar se estiver no mesmo GO
            if (projectileIconImage == null)
            {
                Debug.LogError("UI_ProjectileIconUpdater precisa de uma referência à Image UI ou estar no mesmo GameObject!", this);
                enabled = false;
            }
        }
    }

    void OnEnable()
    {
        // Assina o evento da arma quando o objeto é ativado
        Weapon.OnProjectileChanged += UpdateProjectileIcon;
        // Chama uma vez para garantir que o ícone inicial esteja correto (se a arma já estiver ativa)
        // Isso é importante caso a UI apareça depois da arma já ter inicializado
    }

    void OnDisable()
    {
        // Desassina o evento quando o objeto é desativado ou destruído
        Weapon.OnProjectileChanged -= UpdateProjectileIcon;
    }

    private void UpdateProjectileIcon(GameObject currentProjectilePrefab)
    {
        if (projectileIconImage == null)
        {
            Debug.LogWarning("Projectile Icon Image não está atribuída no Inspector de UI_ProjectileIconUpdater.", this);
            return;
        }

        foreach (var mapping in mappings)
        {
            // Compara o prefab recebido com os prefabs mapeados
            if (mapping.projectilePrefab == currentProjectilePrefab)
            {
                projectileIconImage.sprite = mapping.uiSprite;
                Debug.Log($"Ícone de projétil atualizado para: {currentProjectilePrefab.name}");
                return; // Encontrou o mapeamento, pode sair
            }
        }

        Debug.LogWarning($"Nenhuma sprite de UI encontrada para o projétil: {currentProjectilePrefab.name}", this);
        projectileIconImage.sprite = null; // Limpa a imagem se não encontrar um mapeamento
    }
}