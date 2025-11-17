using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonInstantSFX : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSFX;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (audioSource != null && clickSFX != null)
        {
            audioSource.PlayOneShot(clickSFX, volume);
        }
    }
}
