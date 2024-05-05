using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Redwyre.CustomToolbar.Editor.UIElements
{
    public class BindableImage : Image, IBindable, INotifyValueChanged<Object>
    {
        public new class UxmlFactory : UxmlFactory<BindableImage, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private UxmlStringAttributeDescription m_PropertyPath;

            public UxmlTraits()
            {
                m_PropertyPath = new UxmlStringAttributeDescription
                {
                    name = "binding-path"
                };
            }
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                string valueFromBag = m_PropertyPath.GetValueFromBag(bag, cc);
                if (ve is IBindable bindable)
                {
                    bindable.bindingPath = (string.IsNullOrEmpty(valueFromBag) ? string.Empty : valueFromBag);
                }
            }
        }

        public IBinding binding { get; set; }

        public string bindingPath { get; set; }

        public Object value { get => (Object)image ?? (Object)sprite ?? (Object)vectorImage; set => SetValueWithoutNotify(value); }

        public BindableImage()
        {
        }

        public void SetValueWithoutNotify(Object newValue)
        {
            switch (newValue)
            {
                case Texture t:
                    image = t;
                    break;
                case Sprite s:
                    sprite = s;
                    break;
                case VectorImage v:
                    vectorImage = v;
                    break;
                default:
                    image = null;
                    break;
            }
        }
    }
}