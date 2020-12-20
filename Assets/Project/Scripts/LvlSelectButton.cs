using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LvlSelectButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject LockedImage = null;
    [SerializeField] GameObject NumberText = null;

    public int Level { get; private set; }
    public bool Interactable { get; private set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Interactable)
            SystemController.instance.ChangeScene(Level);
    }

    public void SetUpLockedLvl(int Level)
    {
        SetLevel(Level);
        SetInteractable(false);
        NumberText.SetActive(false);
        GetComponent<Button>().interactable = false;
    }

    public void SetUpUnlockedLvl(int Level)
    {
        SetLevel(Level);
        SetInteractable(true);
        NumberText.GetComponent<Text>().text = "" + (Level+1);
        LockedImage.SetActive(false);
    }

    void SetLevel(int Level) => this.Level = Level;
    void SetInteractable(bool Interactable) => this.Interactable = Interactable;


}
