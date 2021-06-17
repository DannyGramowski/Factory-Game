using UnityEngine;
using UnityEngine.UI;
using System;
using Factory.Core;
using Factory.Buildings;
using Factory.Saving;

namespace Factory.UI {
    public class UIManager : Singleton<UIManager> {

        [SerializeField] RectTransform[] selectItemUIs;
        [SerializeField] DisplayItemsUI itemDisplay;
        [SerializeField] Button save;
        [SerializeField] Button load;
        ISelectableBuilding building;

        private void Awake() {
         //   save.onClick.AddListener(delegate () { SaveObject(); });
           // load.onClick.AddListener(delegate () { SaveManager.LoadGame("test"); });

            gameObject.SetActive(false);
        }

      /*  private void SaveObject() {
            Debug.Log(InputManager.Instance.selection);
            if (InputManager.Instance.selection != null) {
                SaveManager.Save(InputManager.Instance.selection.gameObject);
                SaveManager.SaveGame("test");
            }
        }*/

        public void SetUI(ISelectableBuilding selectItem) {
            if (selectItem == null) return;
            print("set UI for " + (selectItem as Building).name);
            gameObject.SetActive(true);
            building = selectItem;
            for (int i = 0; i < selectItemUIs.Length; i++) {
                if (selectItem.UINum() == i) {
                    selectItemUIs[i].gameObject.SetActive(true);
                } else {
                    selectItemUIs[i].gameObject.SetActive(false);
                }
            }

            itemDisplay.SetDisplayType(selectItem.ProducableBuildingsType());
        }

        public void SetBuildingItem(Item item) {
            building.SetItem(item);
            gameObject.SetActive(false);
        }
    }
}
