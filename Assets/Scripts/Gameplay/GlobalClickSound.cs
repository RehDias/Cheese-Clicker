using UnityEngine;
// É necessário importar a nova biblioteca de Input
using UnityEngine.InputSystem;

public class GlobalClickSound : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Verifica se o mouse existe e se o botão esquerdo foi pressionado nesta janela de frame
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.PlayOneShot(audioSource.clip);
            }
        }
    }
}
