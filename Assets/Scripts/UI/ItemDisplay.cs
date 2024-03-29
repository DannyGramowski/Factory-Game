using UnityEngine;
using UnityEngine.UI;
using Factory.Core;

namespace Factory.UI {
    public class ItemDisplay : MonoBehaviour {
        [SerializeField] Image image;
        [SerializeField] Text text;
        [SerializeField] Button button;
        Item item;

        public void SetItemDisplay(Item item) {
            this.item = item;
            image.sprite = item.sprite;
            text.text = item.itemName;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate { OnClick(); });
        }

        public void SetActive(bool visible) {
            gameObject.SetActive(visible);
        }

        public void OnClick() {
            UIManager.Instance.SetBuildingItem(item);
        }

    }
}
