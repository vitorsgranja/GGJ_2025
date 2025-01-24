using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace PanelController
{
    public class PanelController : MonoBehaviour
    {
        private int currentButton;
        private int buttonsLenght;

        public string menuTitle;
        public bool isMainMenu = true;
        public bool isChildPanel = false;


        public GameObject[] childPanels;

        private GameObject panelActive;
        private Button[] buttons;



        void Start()
        {
            loadPanelsAndButtons();
        }

        private void OnDisable()
        {
            GetComponent<PlayerInput>().enabled = false;
        }

        private void OnEnable()
        {
            loadPanelsAndButtons();
        }

        void Update()
        {
            if (buttons.Length > 0)
            {

                buttons[currentButton].Select();

                foreach (Button button in buttons)
                {
                    if (button == buttons[currentButton])
                    {
                        button.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                        button.GetComponentInChildren<TextMeshProUGUI>().alpha = 1f;

                    }
                    else
                    {
                        button.GetComponentInChildren<TextMeshProUGUI>().color = Color.grey;
                        button.GetComponentInChildren<TextMeshProUGUI>().alpha = 1f;
                    }

                    if (!button.IsInteractable())
                    {
                        button.GetComponentInChildren<TextMeshProUGUI>().color = Color.grey;
                        button.GetComponentInChildren<TextMeshProUGUI>().alpha = 0.3f;
                    }
                }

            }

        }


        private void loadPanelsAndButtons()
        {
            TextMeshProUGUI title = transform.Find("Title").GetComponent<TextMeshProUGUI>();
            title.text = menuTitle;

            buttons = transform.Find("Buttons").GetComponentsInChildren<Button>();

            if (buttons.Length > 0)
            {
                buttons[0].Select();
                buttonsLenght = buttons.Length - 1;
            }

            panelActive = null;

            if (childPanels.Length > 0)
            {
                foreach (GameObject p in childPanels)
                {
                    Debug.Log(p);
                    p.SetActive(false);
                }
            }

            GetComponent<PlayerInput>().enabled = true;
        }



        // Function to handle movement input
        public void OnUp(InputAction.CallbackContext context)
        {

            if (context.performed)
            {

                if (buttons.Length > 0)
                {

                    if (currentButton == 0)
                    {
                        currentButton = buttons.Length - 1;

                    }
                    else
                    {

                        currentButton--;

                        if (!buttons[currentButton].IsInteractable())
                        {
                            currentButton--;
                        }

                    }
                }

            }

        }

        public void OnDown(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (buttons.Length > 0)
                {
                    if (currentButton < (buttons.Length - 1))
                    {

                        currentButton++;

                        if (!buttons[currentButton].IsInteractable())
                        {
                            currentButton++;
                        }

                    }
                    else
                    {
                        currentButton = 0;
                    }

                }

            }

        }

        public void confirmButton(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (buttons.Length > 0)
                {
                    buttons[currentButton].onClick.Invoke();
                }
            }
        }

        public void backButton(InputAction.CallbackContext context)
        {
            gameObject.SetActive(false);
        }

        public void backButtonPanel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (isChildPanel)
                {
                    GetComponent<PlayerInput>().enabled = false;
                    transform.parent.GetComponent<PlayerInput>().enabled = true;
                    Debug.Log(gameObject);
                    gameObject.SetActive(false);
                }
                else
                {
                    if (!isMainMenu)
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
        }

        public void BackAction()
        {
            if (panelActive != null)
            {
                panelActive.SetActive(false);
                panelActive = null;
                GetComponent<PlayerInput>().enabled = true;
            }
            else if (isChildPanel)
            {
                GetComponent<PlayerInput>().enabled = false;
                transform.parent.GetComponent<PlayerInput>().enabled = true;
                gameObject.SetActive(false);
            }
            else if (!isMainMenu)
            {
                gameObject.SetActive(false);
            }
        }

        public void setPanelActive(int panel)
        {
            if (panelActive == null)
            {
                GetComponent<PlayerInput>().enabled = false;
                panelActive = childPanels[panel];
                panelActive.SetActive(true);
                panelActive.GetComponent<PlayerInput>().enabled = true;
            }
        }


    }

}

