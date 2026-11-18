using UnityEngine;
using UnityEngine.InputSystem;

// keeps track of what you're holding. lantern goes in the left hand, axe or log in the right
// Q drops the right hand first, then the left
// goes on the player
public class PlayerHands : MonoBehaviour
{
    public Camera playerCamera;
    public HoldableItem rightItem;
    public HoldableItem leftItem;

    // called by an item when you pick it up. if that hand's full, drop the old thing first
    public void Take(HoldableItem item)
    {
        if (item.leftHand)
        {
            if (leftItem != null) leftItem.Drop();
            leftItem = item;
        }
        else
        {
            if (rightItem != null) rightItem.Drop();
            rightItem = item;
        }
    }

    // called by an item when it leaves your hand
    public void LetGo(HoldableItem item)
    {
        if (rightItem == item) rightItem = null;
        if (leftItem == item) leftItem = null;
    }

    void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.qKey.wasPressedThisFrame) return;

        if (rightItem != null) rightItem.Drop();
        else if (leftItem != null) leftItem.Drop();
    }

    // small reminder in the corner while you're holding something
    void OnGUI()
    {
        if (rightItem == null && leftItem == null) return;
        var style = new GUIStyle(GUI.skin.label) { fontSize = 18 };
        GUI.Label(new Rect(20f, Screen.height - 40f, 300f, 30f), "Q to drop", style);
    }
}
