using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PanelWrapper : MonoBehaviour
{
 
    [SerializeField] private InputField _input;

    [SerializeField] private Text _output;

    [SerializeField] private ScrollRect _scrollRect;

    private bool _focusedRequested = false;
    private bool _scrollRequested = false;

    public string GetInput()
    {
        return _input.text;
    }

    public void ClearInput()
    {
        _input.text = "";
        _focusedRequested = true;
    }

    public void RegisterInputHandler(UnityAction pInputHandler)
    {
        _input.onEndEdit.AddListener((value) => pInputHandler());
    }

    public void UnregisterInputHandlers()
    {
        _input.onEndEdit.RemoveAllListeners();
    }

    public void AddOutput(string pOutput)
    {
        _output.text += pOutput + "\n";
        _scrollRequested = true;
    }

    private void Update()
    {
        checkFocus();
    }

    private void checkFocus()
    {
        if (_focusedRequested)
        {
            _input.ActivateInputField();
            _input.Select();
            _focusedRequested = false;
        }

        if (_scrollRequested)
        {
            _scrollRect.verticalNormalizedPosition = 0;
            _scrollRequested = false;
        }
    }

}
