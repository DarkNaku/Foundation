using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPopupHandler 
{
    string Name { get; }
    Canvas PopupCanvas { get; }
    bool Interactable { get; set; }
    bool IsInTransition { get; }
    bool IsShow { get; }

    void Initialize();
    IEnumerator Show();
    IEnumerator Hide();
    void OnEscape();
}
