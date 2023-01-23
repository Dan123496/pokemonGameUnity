using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MenuControllor : MonoBehaviour
{
    [SerializeField] GameObject menu;
     
    List<TextMeshProUGUI> menuItems;
    [SerializeField] Color highlightColor = Color.red;
    [SerializeField] AudioClip error;
    [SerializeField] AudioClip curser;
    [SerializeField] AudioClip back;
    [SerializeField] AudioClip selected;
    [SerializeField] AudioSource audioSource;

    public event Action<int> onMenuSelected;
    public event Action onBack;

    int selectedItem = 0;
    private void Awake()
    {
        menuItems = menu.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        Debug.Log(menuItems.Count);
        
    }
    public void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
    public void OpenMenu()
    {
        if (menu.activeSelf == false)
        {
            menu.SetActive(true);
            UpdateItemSelection();
        } 
    }
    public void CloseMenu()
    {
       
            menu.SetActive(false);
    }
    public void HandleUpdate()
    {
        int prevSelection = selectedItem;
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            ++selectedItem;
            audioSource.PlayOneShot(curser);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            --selectedItem;
            audioSource.PlayOneShot(curser);
        }
        if (selectedItem < 0)
        {
            selectedItem = menuItems.Count - 1;
            audioSource.PlayOneShot(curser);
        }
        else if (selectedItem >= menuItems.Count)
        {
            selectedItem = 0;
            audioSource.PlayOneShot(curser);
        }
        
        if(selectedItem != prevSelection)
        {
            Debug.Log(selectedItem);
            UpdateItemSelection();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            audioSource.PlayOneShot(selected);
            onMenuSelected?.Invoke(selectedItem);
            CloseMenu();
        }
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.X))
        {
            audioSource.PlayOneShot(back);
            onBack?.Invoke();
            CloseMenu();
        }

    }
    void UpdateItemSelection()
    {
        for(int i = 0; i <= menuItems.Count -1; i++)
        {
            if(i == selectedItem)
            {
                menuItems[i].color = highlightColor;
                
            }
            else
            {
                menuItems[i].color = Color.black;
                
            }
        }
    }
}
