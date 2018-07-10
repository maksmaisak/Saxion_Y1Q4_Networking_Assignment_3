using UnityEngine;

public class ColorOverlay : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }
    
    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}