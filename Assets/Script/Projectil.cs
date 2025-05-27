// Anexe esse script ao projectil
using UnityEngine;
using System.Collections; // Necessário para usar corrotinas (IEnumerator)

public class Projectil : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float timeToLive;
    [SerializeField] private ElementType projectilElement;
    private Rigidbody2D projectilRb;
    private Animator projectilAnimator;
    private int direcaoX;
    private bool hasHit = false; // Adicione uma flag para evitar múltiplas chamadas de hit

    void Awake()
    {
        projectilRb = GetComponent<Rigidbody2D>();
        projectilAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        Destroy(gameObject, timeToLive);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;

        hasHit = true;
        EnemySlime enemySlime = collision.gameObject.GetComponent<EnemySlime>();
        if (enemySlime != null)
        {
            enemySlime.TakeDamageByProjectil(damage, projectilElement); // Se encontrou um EnemySlime, chama o método TakeDamage() dele
            Debug.Log($"Projetil causou {damage} de dano ao {collision.gameObject.name}.");
        }
        else
        {
            // Se o projétil colidiu com algo que não é um EnemySlime, mas ainda assim é um impacto
            // Você pode adicionar outras verificações aqui para outros tipos de inimigos/destrutíveis
            // Ex: DestructibleObject destructible = collision.gameObject.GetComponent<DestructibleObject>();
            // if (destructible != null) { destructible.TakeDamage(damage); }
        }
        // Para o movimento do projétil e desativa sua física para colisões futuras
        if (projectilRb != null)
        {
            projectilRb.linearVelocity = Vector2.zero; // Zera a velocidade
            projectilRb.bodyType = RigidbodyType2D.Kinematic; // Impede que a física o afete
        }
        Collider2D col = GetComponent<Collider2D>(); // 2. Desativa o Collider para que ele não colida com mais nada após o primeiro impacto
        if (col != null)
        {
            col.enabled = false;
        }
        // 3. Inicia a animação de hit
        if (projectilAnimator != null)
        {
            projectilAnimator.SetTrigger("hit");
            // Inicia a corrotina para destruir o projétil após a animação
            StartCoroutine(DestroyAfterAnimation(projectilAnimator.GetCurrentAnimatorStateInfo(0).length));
        }
        else
        {
            Destroy(gameObject); // Se não tem Animator, destrói imediatamente para não ficar preso
        }
    }
    // Corrotina para esperar e destruir o objeto
    IEnumerator DestroyAfterAnimation(float delay)
    {

        if (delay < 0) delay = 0; // Garante que o delay não seja negativo, caso a animação tenha duração zero ou estranha
        yield return new WaitForSeconds(delay); // Espera pelo tempo especificado
        Destroy(gameObject); // Finalmente, destrói o GameObject do projétil
    }

    public void SetDirection(int dir)
    {
        direcaoX = dir;
        projectilRb.linearVelocity = new Vector2(direcaoX * speed, 0f);
    }

}
