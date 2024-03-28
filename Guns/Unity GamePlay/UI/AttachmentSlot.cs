using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FistOfTheFree.Guns.Demo

{
    public class AttachmentSlot : VisualElement
    {
        public Label Name => this.Q<Label>("name"); // name of attachment
        public Label Description => this.Q<Label>("description"); // description of attachment
        public VisualElement Icon => this.Q<VisualElement>("icon"); // attachment icon
        public VisualElement Button => this.Q<VisualElement>(null, "chevron-right"); // custom icon

        private static VisualTreeAsset Tree;

        public new class UxmlFactory : UxmlFactory<AttachmentSlot> { }
        public AttachmentSlot()
        {
            GetTreeAsset();
        }


        // sets up how the UI will look
        public AttachmentSlot(string Name, string Description, string IconName)
        {
            GetTreeAsset();
            Setup(Name, Description, IconName);
        }

        
        private void GetTreeAsset()
        {
            if (Tree == null)
            {
                if (Application.isPlaying)
                {
                    Tree = Resources.Load<VisualTreeAsset>("UI/Templates/AttachmentSlot");
                }
                else
                {
                   Tree = Resources.Load<VisualTreeAsset>("Assets/Resources/UI/Templates/AttachmentSlot.uxml");
                }
            }

            if (Tree == null)
            {
                Debug.LogError("Invalid path specified to visual tree asset");
            }
            else
            {
                Tree.CloneTree(this);
            }

            this.AddToClassList("attachment-slot");
            this.AddToClassList("deselected");
        }

        // displays the name, description and icon of the attachment
        public void Setup(string Name, string Description, string IconName)
        {
            this.Name.text = Name;
            this.Description.text = Description;
            Icon.AddToClassList(IconName);
        }
    }
}
