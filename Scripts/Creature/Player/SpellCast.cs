﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpellCast : MonoBehaviour {

	public SpellComboManager ComboManager;
	public Spell spell;
	public Vector2 combo = new Vector2(0, 1);
	public float retryTime;
	public int comboPlace;
	public Player player;
	public Text SpellText;
	private float minButtonCheckTime;

	void Start() {
		if (!ComboManager) {
			ComboManager = GetComponent<SpellComboManager>();
		}
		ComboManager.Spells.Add (this);
		if (!player)
			player = transform.root.GetComponent<Player> ();
	}

	void Update () {
		if (Time.time >= spell.NextSpamResetTime) {
			spell.SpamMana = 0;
			spell.NextSpamResetTime = Time.time + spell.SpamResetTime;
		}
	}

	public IEnumerator Cast () {
		if (player.mana.MP >= spell.ManaConsumed + spell.SpamMana) {
			comboPlace = 3;
			SpellText.text += " [Spell Cast! -" + (spell.ManaConsumed + spell.SpamMana) + "MP]";
			if (spell is Spell_Multihit) {
				Spell_Multihit S = spell as Spell_Multihit;
				S.StartCoroutine (S.CastSpell (Player.Punch.heldItem as Item_Weapon));
			} else if (spell is Spell_Charge) {
				Spell_Charge S = spell as Spell_Charge;
				S.StartCoroutine (S.CastSpell (Player.Punch.heldItem as Item_Weapon));
			} else if (spell is Spell_Shockwave) {
				Spell_Shockwave S = spell as Spell_Shockwave;
				S.StartCoroutine (S.CastSpell (Player.Punch.heldItem as Item_Weapon));
			}
			player.mana.MP -= spell.ManaConsumed + spell.SpamMana;
			spell.NextSpamResetTime = Time.time + spell.SpamResetTime;
			spell.SpamMana ++;
			ComboManager.retryTime = Time.time + 0.25f;
			yield return new WaitForSeconds (1);
			if (SpellText.text.Contains ("Spell Cast"))
				SpellText.text = "";
		} else {
			SpellText.text += " [Not Enough Mana!]";
			ComboManager.retryTime = Time.time + 0.25f;
			yield return new WaitForSeconds(1);
			if (SpellText.text.Contains ("Enough"))
				SpellText.text = "";
		}
	}
}