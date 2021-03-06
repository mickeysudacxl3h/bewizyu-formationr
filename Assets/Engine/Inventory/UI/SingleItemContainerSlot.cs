using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using SS3D.Engine.Examine;

namespace SS3D.Engine.Inventory.UI
{
    /// <summary>
    /// A ui element to modify a container that contains one item
    /// </summary>
    public class SingleItemContainerSlot : InventoryDisplayElement, IPointerClickHandler, ISlotProvider
    {
        public ItemDisplay ItemDisplay;
        private AttachedContainer container;

        public AttachedContainer Container
        {
            get => container;
            set => UpdateContainer(value);
        }

        public void Start()
        {
            Assert.IsNotNull(ItemDisplay);
            if (Container != null)
            {
                UpdateContainer(Container);
            }
        }
        
        public override void OnItemDrop(ItemDisplay display)
        {
            if (!container.Container.Empty)
            {
                return;
            }

            display.DropAccepted = true;
            ItemDisplay.Item = display.Item;
            Inventory.ClientTransferItem(ItemDisplay.Item, Vector2Int.zero, Container);
        }

        private void UpdateDisplay()
        {
            ItemDisplay.Item = container.Container.Items.FirstOrDefault();
        }

        private void UpdateContainer(AttachedContainer newContainer)
        {
            if (container == newContainer)
            {
                return;
            }
            
            if (container != null)
            {
                container.Container.ContentsChanged -= ContainerContentsChanged;
            }
            
            newContainer.Container.ContentsChanged += ContainerContentsChanged;
            container = newContainer;
        }

        private void ContainerContentsChanged(Container _, IEnumerable<Item> items, Container.ContainerChangeType changeType)
        {
            if (changeType != Engine.Inventory.Container.ContainerChangeType.Move)
            {
                UpdateDisplay();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Inventory.ClientInteractWithSingleSlot(container);

            // When receiving a click on one of the hands of the UI, change the current active hand with the one clicked.
            if (eventData.pointerPress.name == "HandRight(Clone)" || eventData.pointerPress.name == "HandLeft(Clone)")
            {
                Inventory.ActivateHand(container);
            }
        }
		
		public GameObject GetCurrentGameObjectInSlot()
		{
			if (ItemDisplay.Item == null)
			{
				return null;
			}
			else
			{
				return ItemDisplay.Item.gameObject;
			}
		}
		
    }
}