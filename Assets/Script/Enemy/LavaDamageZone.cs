using UnityEngine;

public class LavaDamageZone : MonoBehaviour
{
    private DamageDealer damageDealer; // O DamageDealer já deve estar neste GameObject da Lava
    [SerializeField] private float continuousDamageTickInterval = 0.5f; // Intervalo de tempo entre cada tick de dano
    private float lastDamageTickTime; // Para controlar o tempo do dano contínuo
    [SerializeField] private LayerMask slimeLayers; // Camadas dos slimes (você pode selecionar "Slime_Pyro", "Slime_Hydro", etc. no Inspector)
    [SerializeField] private string ignoreDamageTagObject = "Slime_Pyro"; // Tag para o Slime de Fogo (configure no Inspector!)

    void Awake()
    {
        damageDealer = GetComponent<DamageDealer>();
        if (damageDealer == null)
        {
            Debug.LogError("LavaDamageZone precisa de um componente DamageDealer no mesmo GameObject!", this);
            enabled = false; // Desativa o script se não houver DamageDealer
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        GameObject collidedObject = collision.gameObject; // Pega o objeto que colidiu com a lava
        if (collidedObject == null) return; // Se não houver objeto, sai do método
        //Debug.Log($"{collision.gameObject.name}");
        // Verifica se o objeto colidido está em uma das camadas de slime
        if (((1 << collidedObject.layer) & slimeLayers) != 0) // Compara o Bitwise 
        {
            // Verifica se NÃO é o Slime de Fogo
            // Você pode usar Tag ou verificar o componente do Slime para o elemento
            if (collidedObject.CompareTag(ignoreDamageTagObject))
            {
                return; // É um Slime de Fogo, não sofre dano da lava.
            }
            // É um Slime (não de fogo), aplica dano
            if (Time.time >= lastDamageTickTime + continuousDamageTickInterval)
            {
                //EnemySlime enemySlime = collision.gameObject.GetComponent<EnemySlime>();
                EnemySlime slime = collidedObject.GetComponent<EnemySlime>(); // Pega o script do Slime
                if (slime != null)
                {
                    float damageToApply = damageDealer.GetDamageAmount();
                    slime.TakeDamageByLava(damageToApply);
                    Debug.Log($"Slime {collidedObject.name} recebeu {damageToApply} de dano da lava.");
                    lastDamageTickTime = Time.time;
                }
            }
        }
    }
}