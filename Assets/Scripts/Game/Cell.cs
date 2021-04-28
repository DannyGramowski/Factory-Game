using UnityEngine;

public class Cell : MonoBehaviour {
    public Building building;
    public Vector2Int pos;
    public Vector3 itemPos;
    [SerializeField] SpriteRenderer debugVisual;
    [SerializeField] TextMesh positionDisplay;

    [SerializeField] float scaleConst = 5f;
    public void Startup(Vector2Int pos, float cellSize) {
        transform.name = "cell " + Utils.Vector2IntToString(pos);
        this.pos = pos;
        debugVisual.transform.localScale = new Vector3(cellSize * scaleConst, cellSize * scaleConst, 1);
        positionDisplay.text = pos.ToString();
        itemPos = Utils.Vector3SetY(transform);

    }

    public void ShowDebug(bool visible) {
        // debugVisual.gameObject.SetActive(visible);
        positionDisplay.gameObject.SetActive(visible);
    }


}
