// Anexe a qualquer objeto que danifica o player
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f; // Dano que este objeto causa

    public float GetDamageAmount()
    {
        return damageAmount;
    }
}