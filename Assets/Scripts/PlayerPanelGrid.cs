using UnityEngine;
using System.Collections.Generic;

public class PlayerPanelGrid : MonoBehaviour {

	[HideInInspector]
	public List<PlayerPanel> PlayerPanels = new List<PlayerPanel>();
	public Transform PlayerPanelPrefab;

	private RectTransform rect;

	// Use this for initialization
	void Start () {
		rect = GetComponent<RectTransform>();

		for(var i = 0; i < 4; i++) {
			var panel = ((Transform)Instantiate(PlayerPanelPrefab, new Vector3(i * rect.rect.width / 4, 0, 0), Quaternion.identity)).GetComponent<PlayerPanel>();
			panel.transform.SetParent(transform);
			PlayerPanels.Add(panel);
			panel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.rect.width / 4);// = new Rect(i * rect.rect.width / 4, 0, rect.rect.width / 4, 300);
        }
	}

	public PlayerPanel FillEmptyPanel(Pawn pawn) {
		foreach(var panel in PlayerPanels) {
			if(panel.Pawn == null) {
				panel.Pawn = pawn;
				panel.gameObject.SetActive(true);

				return panel;
			}
		}

		return null;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
