using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [System.Serializable]
    public class JUiText
    {
        public bool active = false;
        public string name = "Text";
        public string text = "Text";
        public Vector2 scale = Vector2.one;
        public Vector2 position = Vector2.zero;
        [Space]
        public Color color = Color.black;
        public float fontSize = 24f;
    }

    [System.Serializable]
    public class JUiButton
    {
        public bool active = false;
        public string name = "Button";

        public JUiText text;
        
        public UnityEvent onLeftPress;
        public UnityEvent onRightPress;
        public UnityEvent onMiddlePress;
    }

    public class JUiAction
    {
        public GameObject screen;
        public Vector2 scale = Vector2.one;
        public Vector2 position = Vector2.zero;
        public JUiButton button;
    }

    [System.Serializable]
    public class JUiGrid
    {
        public bool active = false;
        public Vector2 gridSize = Vector2.zero;
        public Vector2 gapSize = Vector2.zero;
    }

    [System.Serializable]
    public class JUiElement
    {
        public string name = "Element";
        public Sprite sprite;
        public Color color = Color.white;
        public Vector2 scale = Vector2.one;
        public Vector2 position = Vector2.zero;
        public bool fullScreen = false;
        [Header("Text")]
        public JUiText text;
        [Space]
        [Header("Grid")]
        public JUiGrid grid;
        [Space]
        [Header("Button")]
        public JUiButton button;
    }

    [System.Serializable]
    public class JUiScreen
    {
        public string name = "Screen";
        public bool isVisible = true;
        public List<JUiElement> elements;
    }

    [SerializeField] public List<JUiScreen> screens;
    public List<JUiAction> buttons = new List<JUiAction>();
    public JUiAction mostRecentAction;

    [Header("Debug")]
    [SerializeField] private Image defaultScreen;
    [SerializeField] private Image defaultElement;
    [SerializeField] private Image defaultButton;
    [SerializeField] private TextMeshProUGUI defaultText;
    [SerializeField] private Texture defaultTexture;

    private void Awake()
    {
        for (int i = 0; i < screens.Count; i++)
        {
            InstantiateScreen(screens[i]);
        }
    }

    private void InstantiateScreen(JUiScreen screen)
    {
        Image newScreen = Instantiate(defaultScreen);

        newScreen.name = screen.name;

        newScreen.transform.SetParent(transform);

        newScreen.rectTransform.anchoredPosition = Vector3.zero;
        newScreen.rectTransform.anchorMin = Vector2.zero;
        newScreen.rectTransform.anchorMax = Vector2.one;
        newScreen.rectTransform.localScale = Vector3.one;

        if (!screen.isVisible)
        {
            newScreen.gameObject.SetActive(false);
        }

        for (int i = 0; i < screen.elements.Count; i++)
        {
            InstantiateElement(screen.elements[i], newScreen);
        }
    }

    private void InstantiateElement(JUiElement element, Image newScreen)
    {
        Image elementInstance = Instantiate(defaultElement);

        elementInstance.name = element.name;

        elementInstance.transform.SetParent(newScreen.transform);

        elementInstance.sprite = element.sprite;
        elementInstance.color = element.color;

        if (element.fullScreen)
        {
            elementInstance.rectTransform.anchoredPosition = Vector3.zero;
            elementInstance.rectTransform.anchorMin = Vector2.zero;
            elementInstance.rectTransform.anchorMax = Vector2.one;
            elementInstance.rectTransform.localScale = Vector3.one;
        }
        else 
        {
            elementInstance.rectTransform.anchoredPosition = element.position;
            elementInstance.rectTransform.localScale = element.scale;
        }

        if (element.grid.active)
        {
            InstantiateGrid(elementInstance, element);
            elementInstance.enabled = false;
            elementInstance.rectTransform.localScale = Vector3.one;
        } 
        else
        {
            if (element.text.active)
            {
                InstantiateText(element.text, elementInstance);
            }

            if (element.button.active)
            {
                InstantiateButton(element, elementInstance);
            }
        }
    }

    private void InstantiateText(JUiText text, Image newElement)
    {
        TextMeshProUGUI textInstance = Instantiate(defaultText);

        textInstance.transform.SetParent(newElement.transform);

        textInstance.rectTransform.localScale = text.scale;
        textInstance.rectTransform.anchoredPosition = text.position;

        textInstance.text = text.text;
        textInstance.color = text.color;
        textInstance.fontSize = text.fontSize;
    }

    private void InstantiateGrid(Image instance, JUiElement element)
    {
        Vector2 elementAnchor = element.position;
        for(int i = 0; i < element.grid.gridSize.x; i++)
        {
            for(int j = 0; j < element.grid.gridSize.y; j++)
            {
                Vector2 centerOffset = element.grid.gridSize / 2.0f;
                centerOffset = new Vector2(MathF.Floor(centerOffset.x), MathF.Floor(centerOffset.y));
                element.position = new Vector2(i - centerOffset.x, -j - centerOffset.y) * element.grid.gapSize;

                if (element.button.active)
                {
                    buttons.Add(GenerateJUiAction(instance.transform.parent.gameObject, element.scale, elementAnchor + element.position, element.button));
                }

                InstantiateGridElement(element, instance);
            }
        }
    }

    private void InstantiateGridElement(JUiElement element, Image newScreen)
    {
        Image elementInstance = Instantiate(defaultElement);

        elementInstance.name = element.name;

        elementInstance.transform.SetParent(newScreen.transform);

        elementInstance.sprite = element.sprite;
        elementInstance.color = element.color;

        if (element.fullScreen)
        {
            elementInstance.rectTransform.anchoredPosition = Vector3.zero;
            elementInstance.rectTransform.anchorMin = Vector2.zero;
            elementInstance.rectTransform.anchorMax = Vector2.one;
            elementInstance.rectTransform.localScale = Vector3.one;
        }
        else
        {
            elementInstance.rectTransform.anchoredPosition = element.position;
            elementInstance.rectTransform.localScale = element.scale;
        }

        if (element.text.active)
        {
            InstantiateText(element.text, elementInstance);
        }

        if(element.button.active && element.button.text.active)
        {
            InstantiateText(element.button.text, elementInstance);
        }
    }
    
    private void InstantiateButton(JUiElement button, Image newElement)
    {
        Image buttonInstance = Instantiate(defaultButton);

        buttonInstance.name = button.button.name;

        buttonInstance.transform.SetParent(newElement.transform);

        buttonInstance.rectTransform.anchoredPosition = newElement.rectTransform.anchoredPosition;
        buttonInstance.rectTransform.localScale = Vector3.one;

        if (button.button.text.active)
        {
            InstantiateText(button.button.text, newElement);
        }

        buttons.Add(GenerateJUiAction(newElement.transform.parent.gameObject, button.scale, button.position, button.button));
    }

    public void ActivateUi(int ui, bool activeState, bool keepOtherScreens = false)
    {
        if (!keepOtherScreens)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        
        transform.GetChild(ui).gameObject.SetActive(activeState);
    }

    private JUiAction GenerateJUiAction(GameObject screen, Vector2 scale, Vector2 position, JUiButton button)
    {
        JUiAction jUiAction = new JUiAction();

        jUiAction.screen = screen;
        jUiAction.scale = scale;
        jUiAction.position = position;
        jUiAction.button = button;

        return jUiAction;
    }

    //Button logic

    public void LeftMousePress()
    {
        CheckButton(0);
    }

    public void RightMousePress()
    {
        CheckButton(1);
    }

    public void MiddleMousePress()
    {
        CheckButton(2);
    }

    private void CheckButton(int interaction)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            JUiAction currentButton = buttons[i];
            if (currentButton.screen.activeSelf)
            {
                Vector2 pos = currentButton.position;
                Vector2 scale = currentButton.scale * defaultButton.rectTransform.sizeDelta / 2.0f;
                Vector2 canvasSize = GetComponent<RectTransform>().sizeDelta;
                Vector2 mousePos = Input.mousePosition / new Vector2(Screen.width, Screen.height) * canvasSize - canvasSize / 2.0f;

                if (mousePos.x >= pos.x - scale.x && mousePos.x <= pos.x + scale.x && mousePos.y >= pos.y - scale.y && mousePos.y <= pos.y + scale.y)
                {
                    mostRecentAction = currentButton;
                    if (interaction == 0)
                    {
                        currentButton.button.onLeftPress.Invoke();
                    } 
                    else if (interaction == 1)
                    {
                        currentButton.button.onRightPress.Invoke();
                    }
                    else
                    {
                        currentButton.button.onMiddlePress.Invoke();
                    }
                    break;
                }
            }
        }
    }

    public void DefaultButtonAction()
    {
        Debug.Log("I have been touched");
    }

    public void TestUiA()
    {
        Debug.Log("I have been touched");
        ActivateUi(1, true);
    }
}