using FistOfTheFree.Guns.Modifiers;
using System.Collections.Generic;

namespace FistOfTheFree.Guns
{

    // Class representing Weapon attachments
    public class Attachment
    {
        public string Name { get; set; } // Attachment Name
        public string Description { get; set; } // Description
        public List<IModifier> Modifiers { get; set; }
        public bool IsSelected { get; set; } // whether attachment is selected.

        public Attachment() 
        {
            Modifiers = new();
        }

        public Attachment(
            string Name, 
            string Description, 
            List<IModifier> Modifiers, 
            int Cost, 
            int UnlockLevel, 
            bool IsSelected = false) : base()
        {
            this.Name = Name;
            this.Description = Description;
            this.Modifiers = Modifiers;
            this.IsSelected = IsSelected;
        }

        public void AddModifier(IModifier Modifier)
        {
            Modifiers.Add(Modifier); // Adds this to modifier
        }
    }
}
