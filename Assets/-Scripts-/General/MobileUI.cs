using UnityEngine;

public class MobileUI : MonoBehaviour
{
    [Header("Testing")]
    [SerializeField] private bool forceShowInEditor = true;

    private void Awake()
    {
#if UNITY_EDITOR
        if (forceShowInEditor)
        {
            gameObject.SetActive(true);
            return;
        }
#endif

        // Application.isMobilePlatform returns true for iOS, Android, and WebGL on mobile devices
        if (Application.isMobilePlatform)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
