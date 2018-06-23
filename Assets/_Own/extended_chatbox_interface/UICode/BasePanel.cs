using UnityEngine;
using System.Collections;

public class BasePanel : MonoBehaviour {

    //private CanvasGroup _canvasGroup;

    private void Awake()
    {
        //_canvasGroup = GetComponent<CanvasGroup>();
    }

    public void DisableGUI()
    {
        gameObject.SetActive(false);

        //_canvasGroup.interactable = false;
    }

    public void EnableGUI()
    {
        gameObject.SetActive(true);
        //_canvasGroup.interactable = true;
    }

}
