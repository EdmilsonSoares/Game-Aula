using UnityEngine;

public class SomGerenciador : MonoBehaviour
{
    public static SomGerenciador Instance { get; private set; }
    [Header("Configurações de Som")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] trilhas;
    [SerializeField] private AudioClip[] audios;
    /*private float userMusicVolume = 1;
    private float userSoundsVolume = 1f;
    private float pitch;*/

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TocarTrilha(string nomeCena, float volume)
    {
        switch (nomeCena)
        {
            case "Gameplay":
                audioSource.clip = trilhas[0];
                TocarAudio(0, 0.5f);
                break;
            default:
                audioSource.Stop(); // Se não achar umna música pausa o componente AudioSource
                break;
        }

        if (audioSource != null)
        {
            audioSource.volume = volume;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void TocarAudio(int index, float volume)
    {
        if (audioSource == null) return;
        audioSource.PlayOneShot(audios[index], volume);
    }
}
