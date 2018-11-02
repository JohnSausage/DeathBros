using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class ButtonSetInput : MonoBehaviour
{
    [SerializeField]
    protected string inputName;

    [Space]

    [SerializeField]
    protected TextMeshProUGUI currentInputText;

    [SerializeField]
    protected TextMeshProUGUI setInputText;

    protected Button button;
    protected string currentButton;

    protected void Start()
    {
        setInputText.text = "Set " + inputName;

        button = GetComponent<Button>();
        button.onClick.AddListener(ClickButton);

        UpdateButtonName();

        InputManager.InputSet += RefreshButton;
    }

    protected void UpdateButtonName()
    {
        if (InputManager.Instance != null)
        {
            currentButton = InputManager.Instance.GetInput(inputName).GetButtonName();
            currentInputText.text = currentButton;
        }
    }

    protected void ClickButton()
    {
        InputManager.Instance.SetInput(inputName);
    }

    protected void RefreshButton(DualInput input)
    {
        if (input == InputManager.Instance.GetInput(inputName))
        {
            UpdateButtonName();
        }
    }
}
