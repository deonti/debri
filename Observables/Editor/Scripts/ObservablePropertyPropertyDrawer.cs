using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Debri.Observables.Editor
{
  [CustomPropertyDrawer(typeof(ObservableProperty<>))]
  public class ObservablePropertyPropertyDrawer : PropertyDrawer
  {
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
      var root = new VisualElement();
      root.Add(new PropertyField(property.FindPropertyRelative("_value"), property.displayName));
      return root;
    }
  }
}