// EnumOrder.cs
// Credit to chai on the Unity Forums

using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field)]
public class EnumOrder : PropertyAttribute {

	public new readonly int[] order;

	public EnumOrder (string _orderStr) {
		order = StringToInts(_orderStr);
	}

	public EnumOrder (int[] _order) {
		order = _order;
	}

	int[] StringToInts (string str) {
		string[] stringArray = str.Split(',');
		int[] intArray = new int[stringArray.Length];
		for (int i=0; i<stringArray.Length; i++)
			intArray[i] = int.Parse (stringArray[i]);

		return (intArray);
	}

}