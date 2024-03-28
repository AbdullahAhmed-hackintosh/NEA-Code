using FistOfTheFree.Guns.ImpactEffects;
using FistOfTheFree.Guns.Modifiers;
using FistOfTheFree.ImpactSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace FistOfTheFree.Guns.Demo.UI
//class for UI Document and defining all the different attachments
{
    [RequireComponent(typeof(UIDocument))]
    public class AttachmentDocumentController : MonoBehaviour
    {
        [field: SerializeField]
        public bool IsVisible { get; private set; }
        [SerializeField]
        private MonoBehaviour[] InputControllers;
        [SerializeField]
        private PlayerGunSelector GunSelector;

        [SerializeField]
        private ImpactType ExplosiveType;
        [SerializeField]
        private ImpactType FrostType;

        private AttachmentSlot HandleSlot;
        private AttachmentSlot BarrelSlot;
        private AttachmentSlot AmmoSlot;

        private readonly Dictionary<Slot, int> SelectedAttachments = new()
        {
            { Slot.Handle, 0 },
            { Slot.Barrel, 0 },
            { Slot.Ammo, 0 }
        };

        //Creating Attachments
        private static readonly Dictionary<Slot, List<Attachment>> Attachments = new()
        {
            { Slot.Handle, new List<Attachment>()
                {
                    new Attachment("Standard", "Standard issue", new(), 0, 0, true), // Different Types of Grips
                    new Attachment("Potato Grip", "Reduces the time it takes to reach maximum spread. Reduces vertical spread by 10%", new()
                    {
                        new Vector3Modifier(new Vector3(1, 0.9f, 1), "ShootConfig/Spread"),
                        new FloatModifier(0.9f, "ShootConfig/SpreadMultiplier"),
                        new FloatModifier(1.25f, "ShootConfig/MaxSpreadTime")
                    }, 0, 0),
                    new Attachment("Enhanced Grips", "Reduces Spread by 25%", new()
                    {
                        new FloatModifier(0.75f, "ShootConfig/SpreadMultiplier"),
                        new Vector3Modifier(new Vector3(0.75f, 0.75f, 0.75f), "ShootConfig/Spread")
                    }, 0, 0)
                } },
            { Slot.Barrel, new List<Attachment>() // Different types of Barrels
                {
                    new Attachment("Standard", "Standard issue", new(), 0, 0, true),
                    new Attachment("Long Barrel", "Reduces the time it takes to reach maximum spread.", new() { new FloatModifier(1.25f, "ShootConfig/MaxSpreadTime") }, 0, 0)
                } },
            { Slot.Ammo, new List<Attachment>() // Different Types of Ammo
                {
                    new Attachment("Standard", "Standard issue", new(), 0, 0, true),
                    new Attachment("Explosive Rounds", "Explodes on impact dealing damage to nearby enemies. Increases bullet weight significantly.", new()
                    {
                        new ImpactTypeModifier(null, ""),
                        new FloatModifier(2, "ShootConfig/BulletWeight"),
                        new ImpactEffectReplacementModifier(new ICollisionHandler[] {
                            new Explode(1.5f, new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0.5f) }), 10, 10)
                        })
                    }, 0, 0),
                    new Attachment("Frost Rounds", "Creates a small frost explosion on impact dealing light damage to nearby enemies and slowing them. Increases bullet weight significantly.", new()
                    {
                        new ImpactTypeModifier(null, ""),
                        new FloatModifier(2, "ShootConfig/BulletWeight"),
                        new ImpactEffectReplacementModifier(new ICollisionHandler[] {
                            new Frost(0.75f, new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0.5f) }), 7, 10, new AnimationCurve(new Keyframe[] { new Keyframe(0, 0.5f), new Keyframe(1.75f, 0.5f), new Keyframe(2, 1)}))
                        })
                    }, 0, 0)
                } }
        };

        //  User Interface Stuff
        private UIDocument Document; 
        private ScrollView SlotScrollView;
        private VisualElement BorderLeft;
        private ScrollView OptionScrollView;

        private void Awake()
        {
            Document = GetComponent<UIDocument>();
            IsVisible = false;
            Document.rootVisualElement.AddToClassList("hidden");

            // some hack to add in the serialized fields into the dictionary.
            // If you have serialized data (highly recommend) instead of static dictionary you shouldn't need to do this
            // video is already quite long so ... :) quick and dirty
            (Attachments[Slot.Ammo][1].Modifiers[0] as ImpactTypeModifier).Amount = ExplosiveType;
            (Attachments[Slot.Ammo][2].Modifiers[0] as ImpactTypeModifier).Amount = FrostType;
        }


        // ensures that the attachments the player chooses stay persistent
        private void Start()
        {
            SlotScrollView = Document.rootVisualElement.Q<ScrollView>("slot-scrollview");

            BarrelSlot = new("Barrel", "Standard issue", "standard");

            BarrelSlot.Button.RegisterCallback<ClickEvent>((_) => HandleSlotClick(Slot.Barrel, BarrelSlot));
            SlotScrollView.Add(BarrelSlot);

            HandleSlot = new("Handle", "Standard issue", "standard");
            HandleSlot.Button.RegisterCallback<ClickEvent>((_) => HandleSlotClick(Slot.Handle, HandleSlot));
            SlotScrollView.Add(HandleSlot);

            AmmoSlot = new("Ammo Type", "Standard issue", "standard");
            AmmoSlot.Button.RegisterCallback<ClickEvent>((_) => HandleSlotClick(Slot.Ammo, AmmoSlot));
            SlotScrollView.Add(AmmoSlot);

            Button closeButton = Document.rootVisualElement.Q<Button>();
            closeButton.RegisterCallback<ClickEvent>((_) => Hide());
        }

        // ESC key used to access the Attachment UI 
        private void Update()
        {
            if (Keyboard.current.escapeKey.wasReleasedThisFrame)
            {
                if (IsVisible)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
            }
        }

    // when player leaves the Attachment UI menu, this ensures that the cursor becomes locked to the middle of the screen again
        private void Hide()
        {
            Document.rootVisualElement.AddToClassList("hidden");
            Document.rootVisualElement.RemoveFromClassList("visible");
            IsVisible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            GunSelector.ApplyModifiers(CollectModifiers());
            ToggleInputHandlers();
        }

        private void ToggleInputHandlers()
        {
            foreach (MonoBehaviour behaviour in InputControllers)
            {
                behaviour.enabled = !IsVisible;
            }
        }

    // when player enters Attachment menu, cursor lock is set to none.
        private void Show()
        {
            Document.rootVisualElement.AddToClassList("visible");
            Document.rootVisualElement.RemoveFromClassList("hidden");
            IsVisible = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ToggleInputHandlers();
        }

        // Animations within the UI
        private IEnumerator BuildOptionsForSlot(Slot FocusSlot)
        {
            VisualElement optionContainer = Document.rootVisualElement.Q<VisualElement>("option-container");
            if (BorderLeft == null)
            {
                BorderLeft = Document.rootVisualElement.Q<VisualElement>("border-left");
            }
            optionContainer.AddToClassList("fade-in-right");
            optionContainer.RemoveFromClassList("fade-out-left");
            optionContainer.focusable = true;
            optionContainer.pickingMode = PickingMode.Position;

            VisualElement backButton = optionContainer.Q<VisualElement>("icon");
            if (OptionScrollView == null)
            {
                OptionScrollView = optionContainer.Q<ScrollView>();
            }
            else
            {
                OptionScrollView.Clear();
            }

            backButton.RegisterCallback<ClickEvent>((_) => HideOptionContainer(optionContainer));

            for (int i = 0; i < Attachments[FocusSlot].Count; i++)
            {
                int index = i;
                Attachment attachment = Attachments[FocusSlot][i];
                AttachmentSlot slot = new(attachment.Name, attachment.Description, attachment.Name.ToLower().Replace(" ", "-"));

                slot.RegisterCallback<ClickEvent>((_) => HandleChangeOption(slot, FocusSlot, index));

                if (Attachments[FocusSlot][i].IsSelected)
                {
                    slot.AddToClassList("selected");
                }

                VisualElement chevron = slot.Q<VisualElement>(null, "chevron-right"); // custom icons 
                chevron.RemoveFromClassList("chevron-right");
                chevron.AddToClassList("display-none");

                OptionScrollView.Add(slot);
            }

            int childIndex = 3;
            foreach (VisualElement child in OptionScrollView.Children())
            {
                child.AddToClassList("fade-out-up");
                child.AddToClassList($"delay-{childIndex * 50}ms");
                childIndex++;
            }

            yield return null;

            foreach (VisualElement child in OptionScrollView.Children())
            {
                child.AddToClassList("fade-in-down");
            }
            BorderLeft.AddToClassList("border-left-animate-height-100");
            BorderLeft.AddToClassList("delay-150ms");
        }

        private void HideOptionContainer(VisualElement OptionContainer)
        {
            OptionContainer.AddToClassList("fade-out-left");
            OptionContainer.RemoveFromClassList("fade-in-right");
            BorderLeft.RemoveFromClassList("border-left-animate-height-100");
            BorderLeft.RemoveFromClassList("delay-150ms");
            BorderLeft.AddToClassList("delay-100ms");
            OptionContainer.focusable = false;
            OptionContainer.pickingMode = PickingMode.Ignore;

            int index = 0;
            foreach (VisualElement child in OptionScrollView.Children())
            {
                int i = index;
                child.RemoveFromClassList("fade-in-down");
                child.RemoveFromClassList($"delay-{(i + 3) * 50}ms");
                child.AddToClassList($"delay-{(OptionScrollView.childCount - i) * 50}ms");
                index++;
            }

            ClearSlotSelectionClasses();
        }

        private void HandleSlotClick(Slot Type, AttachmentSlot Slot)
        {
            ClearSlotSelectionClasses();
            Slot.AddToClassList("selected");
            StartCoroutine(BuildOptionsForSlot(Type));
        }

        private void HandleChangeOption(AttachmentSlot Slot, Slot FocusSlot, int Index)
        {
            foreach (VisualElement child in OptionScrollView.Children())
            {
                child.RemoveFromClassList("selected");
            }
            Slot.AddToClassList("selected");

            // Deselect the previously selected one
            Attachments[FocusSlot][SelectedAttachments[FocusSlot]].IsSelected = false;
            // Select the new one
            SelectedAttachments[FocusSlot] = Index;
            Attachments[FocusSlot][Index].IsSelected = true;

            AttachmentSlot slot = null;
            switch(FocusSlot)
            {
                case Guns.Slot.Ammo:
                    slot = AmmoSlot;
                    break;
                case Guns.Slot.Barrel:
                    slot = BarrelSlot; 
                    break;
                case Guns.Slot.Handle:
                    slot = HandleSlot;
                    break;
            }

            slot.Icon.ClearClassList();
            slot.Setup(Slot.Name.text, Slot.Description.text, Slot.Name.text.ToLower().Replace(" ", "-"));

            //probably you would want to persist the selection at this time as well
        }

        private void ClearSlotSelectionClasses()
        {
            foreach (VisualElement element in SlotScrollView.Children())
            {
                element.RemoveFromClassList("selected");
            }
        }

        // collects all the modifications made and returns it
        private IModifier[] CollectModifiers()
        {
            List<IModifier> modifiersToApply = new();

            foreach (KeyValuePair<Slot, int> keyValuePair in SelectedAttachments)
            {
                modifiersToApply.AddRange(Attachments[keyValuePair.Key][keyValuePair.Value].Modifiers);
            }

            return modifiersToApply.ToArray();
        }
    }
}
