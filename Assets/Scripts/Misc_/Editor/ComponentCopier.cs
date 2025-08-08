using UnityEngine;
using UnityEditor;

public class ComponentCopier : MonoBehaviour
{
    [MenuItem("Tools/Copy Components From Selected")]
    static void CopyComponents()
    {
        if (Selection.gameObjects.Length != 2)
        {
            Debug.LogError("Select exactly 2 GameObjects: source first, then target.");
            return;
        }

        GameObject source = Selection.gameObjects[0];
        GameObject target = Selection.gameObjects[1];

        foreach (Component comp in source.GetComponents<Component>())
        {
            if (comp is Transform) continue; // Skip Transform

            UnityEditorInternal.ComponentUtility.CopyComponent(comp);
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(target);
        }

        Debug.Log("Copied all components from " + source.name + " to " + target.name);
    }
}
