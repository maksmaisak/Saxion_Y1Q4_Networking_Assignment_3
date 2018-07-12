using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableListItemView : MonoBehaviour
{
    [SerializeField] Text uiText;
    [SerializeField] Button button;

    public event Action<TableListItemView> OnClick;

    void Start()
    {
        button.onClick.AddListener(() => OnClick?.Invoke(this));
    }

    public void SetText(string text)
    {
        uiText.text = text;
    }
}
