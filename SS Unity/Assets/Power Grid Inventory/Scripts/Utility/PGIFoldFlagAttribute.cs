/**********************************************
* Power Grid Inventory
* Copyright 2015 James Clark
**********************************************/
using UnityEngine;
using System.Collections;
using System;

namespace PowerGridInventory.Utility
{
	/// <summary>
	/// Used to mark a boolean field so that the PGI custom
	/// editors know where to store the PGI 'Events' foldout state.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class PGIFoldFlagAttribute : System.Attribute
	{
		
	}
}
