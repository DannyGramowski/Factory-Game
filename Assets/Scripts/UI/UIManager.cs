using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager> {

    [SerializeField] RectTransform[] selectItemUIs;
    [SerializeField] DisplayItemsUI itemDisplay;
    ISelectItem building;


    private void Awake() {
        gameObject.SetActive(false);
    }

    public void SetUI(ISelectItem selectItem) {
        building = selectItem;
        gameObject.SetActive(true);
        for(int i = 0; i < selectItemUIs.Length; i++) {
            if(selectItem.UINum() == i) {
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
