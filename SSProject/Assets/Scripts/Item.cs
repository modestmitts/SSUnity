using UnityEngine;
using System.Collections;

[System.Serializable]
public class Item
{
	public string itemName; 
	public int itemID; //Unique Item ID
	public string itemDesc; // Description of item
	public Texture2D itemIcon;  // An icon representing the item
	public float itemPrice; //Price per standard unit
	public float itemMass; // Weight of item in kilograms (remember 1 Kilos = 2.2 Lb)
	public float itemVolume; //in cubic meters (1 cubic m = 35.3 cubic feet)
	public enum itemWear; // How worn out is it
	public ItemType itemType; 
	public ItemAttType itemAttType; // 
	public float itemAttA; //
	public float itemAttB; //
	public float itemAttC; //
	public float itemAttD;

	public enum itemWear
	{
		NA,
		New,
		Used,
		Worn,
		Brittle,
		Broken
	}


	public enum ItemType
	{
		ChooseSomething,
		EngineFuelProc,  	
		EnginePowerGen,		// A: Power			B: Effiency		C: WarpDis		D: Cooldown
		EngineVectControl,	// A: Reaction
		AtmoEngine,			// A: MaxFuel		B: CurFuel		C: Efficiency
		SensorSource,		// A: Range
		Communicator,		// A: Range			B: TranslatorBool
		ShipGear,			// A; Attribute
		MissileSys,			// A: Damage 		B: MaxLoad		C: CurrentLoad
		BeamSys,			// A: Damage		B: Cooldown		C: PowerReq
		Shield,   			// A: DmgReduc		B: PowerReq
		HandTool,			// A: SpeedMod
		HandWeapon,			// A: Damage		B: Cooldown
		QuestItem,			// A: Quest#
		Gear, 				// A: Attribute
		LootItem
	}

	public enum ItemAttType
	{
		//Default
		NA,

		//Engine System Types
		Engine_SuperBoost,
		Engine_Fusion,
		Engine_AntiMatter,
		Engine_Hyperspace,
		Engine_Hypercoil,
		Engine_Warp,

		// Senser Types
		Sensor_Cavitating,
		Sensor_Thermic,
		Sensor_Tracking,
		Sensor_CavTherm,
		Sensor_PinPoint, //Defense system sensor needed for point defence

		//Ship Gear Types
		Sgear_InertialDamper,
		Sgear_DockingComputer,
		Sgear_TractorBeam,

		//Missile Types
		Missile_Photon,
		Missile_Magnetic,
		Missile_HeatSeeking,
		Missile_Tunneling,
		Missile_Streak,
		Missile_Ballistic,

		//Beam Weapon Types
		Beam_Laser, 
		Beam_Maser,
		Beam_Radiation,
		Beam_Particle,
		Beam_Plasma,
		Beam_Resonating,

		//Shield
		Shield_EnergyWave,
		Shield_ElectroMagnetic,
		Shield_PointDefence,

		//Tool
		Tool_Screwdriver,
		Tool_Wrench,
		Tool_Saw,
		Tool_Torch,
		Tool_Meter,
		Tool_Hammer,
		Tool_Crowbar,
		Tool_Drill,
		Tool_TweakingTool,

		//Handweapons
		//Personal Gear
		Wearable_Shirt,
		Wearable_Pants,
		Wearable_Hat,
		Wearable_JetPack,
		Wearable_ToolBelt,
		Wearable_SpaceSuit,
		Wearable_PersonalShield,
		Wearable_AirTank
	}

	public Item(string name, int id, string desc, int cost, float weight, float vol, ItemType type, ItemAttType atype, float a, float b, float c, float d)
	{
		itemName = name;
		itemID = id;
		itemDesc = desc;
		itemIcon = Resources.Load<Texture2D> ("Item Icons/" + name); // Need to be found in same folder call Item Icons
		itemPrice = cost;
		itemMass = weight;
		itemVolume = vol;
		itemType = type;
		itemAttType = atype;
		itemAttA = a;
		itemAttB = b;
		itemAttC = c;
		itemAttD = d;
	}

	public Item()
	{

	}

}