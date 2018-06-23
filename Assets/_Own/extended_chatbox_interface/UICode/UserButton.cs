using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UserButton : Button {

    [SerializeField]
    private Text _label;

    public void SetName (string pName)
    {
        _label.text = pName;
    }

}
