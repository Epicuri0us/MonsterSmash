using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour {

	public Pawn Pawn;

	private Text DamageText;
	private Text NameText;
	private CanvasRenderer canvasRenderer;

	// Use this for initialization
	void Start () {
		DamageText = transform.Find("Damage").GetComponent<Text>();
		NameText = transform.Find("Name").GetComponent<Text>();
		canvasRenderer = GetComponent<CanvasRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
		// TODO: Find cleaner way to hide panel
		if(Pawn == null) {
			gameObject.SetActive(false);
			return;
		}

		NameText.text = Pawn.Name;
		DamageText.text = Mathf.Ceil(Pawn.Damage * 100).ToString();
    }
}
