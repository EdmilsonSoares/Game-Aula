// Anexe esse script ao projectil
using Unity.VisualScripting;
using UnityEngine;
using System.Collections; // Necessário para usar corrotinas (IEnumerator)

public class Projectil : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float damage; // Ainda não estou usando
    [SerializeField] private float timeToLive;
    private Rigidbody2D projectilRb;
    private Animator projectilAnimator;
    private int direcaoX;
    private bool hasHit = false; // NOVO: Adicione uma flag para evitar múltiplas chamadas de hit

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

        hasHit = true; // Marca que a colisão aconteceu

        // 1. Para o movimento do projétil e desativa sua física para colisões futuras
        if (projectilRb != null)
        {
            projectilRb.linearVelocity = Vector2.zero; // Zera a velocidade
            projectilRb.bodyType = RigidbodyType2D.Kinematic; // Impede que a física o afete
        }

        // 2. Desativa o Collider para que ele não colida com mais nada após o primeiro impacto
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // 3. Inicia a animação de hit
        if (projectilAnimator != null)
        {
            projectilAnimator.SetTrigger("hit");

            // Inicia a corrotina para destruir o projétil após a animação
            // GetCurrentAnimatorStateInfo(0).length pega a duração da animação atual no Layer 0 do Animator.
            // Isso só funciona se a animação de hit for o estado *atual* no momento do trigger.
            // Para maior robustez, pode-se usar um tempo fixo, e.g., 0.3f.
            StartCoroutine(DestroyAfterAnimation(projectilAnimator.GetCurrentAnimatorStateInfo(0).length));
        }
        else
        {
            // Se não tem Animator, destrói imediatamente para não ficar preso
            Destroy(gameObject);
        }
    }

    // Corrotina para esperar e destruir o objeto
    IEnumerator DestroyAfterAnimation(float delay)
    {
        // Garante que o delay não seja negativo, caso a animação tenha duração zero ou estranha
        if (delay < 0) delay = 0;

        yield return new WaitForSeconds(delay); // Espera pelo tempo especificado

        // Finalmente, destrói o GameObject do projétil
        Destroy(gameObject);
    }

    public void SetDirection(int dir)
    {
        direcaoX = dir;
        projectilRb.linearVelocity = new Vector2(direcaoX * speed, 0f);
    }

}
