using UnityEngine;

public class ColorOverlay : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(false);
    }
    
    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}