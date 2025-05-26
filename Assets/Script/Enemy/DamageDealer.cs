// Anexe a qualquer objeto que danifica o player
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f; // Dano que este objeto causa por padrão

    public float GetDamageAmount()
    {
        return damageAmount;
    }
}