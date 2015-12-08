/*******************************************************************************
					StructureClass
	This class stores the structure information of a ship. Like Fuselage, Nose, Pylon, Cockpit etc. 
	It also contains the list of what Bulkhead assemblies are present, and if there are Tool or Weapon Mounts
	
	Note that Tool mounts need to be on a structure that also has Cargo.
********************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Structure : MonoBehaviour
{
    /*
    public string Model; //Contains the Structure model name eg: Park-Otoole-Delta-V;
    public StructType StructureType; // What kind of structure is it (Wing, Cockpit etc)
    public int TotalArmor; // How much damage it can take before it is destroyed
    public float DamageMin; // Structure damage that gives 5% chance to take internal damage 
    public float DamageMed; // Structure damage that gives 25% chance to take internal damage 
    public float DamageMax; // Structure damage that gives 66% chance to take internal damage 
    public List <PanelClass> Panels; // The list of Panels associated with this Model
    int CurrentArmor; //
    float CurrentChance; //

    enum StructType { Nose, Pylon, Cockpit, Wing, Strut, LandingGear, Hull, Fuselage, Dome, EngineHousing, Tank, Cargo, Mount, HardPoint };

    public Structure()
    {
    }

    public Structure(string mod, StructType sType, int Arm, float Dmin, float Dmed, float Dmax, List<PanelClass> PC)
    {
        Model = mod;
        StructureType = sType;
        TotalArmor = Arm;
        DamageMin = Dmin;
        DamageMed = Dmed;
        DamageMax = Dmax;
        Panels = PC;
        CurrentArmor = TotalArmor;
        CurrentChance = TotalArmor * DamageMin;             
    }

    void TakeDamage(int damage)
    {
        float percentchance = 0.5f;
        CurrentArmor -= damage;
        if (CurrentArmor <= 0)
        {
            CurrentArmor = 0;
            DestroyStructure();

        }
        else if (CurrentArmor < CurrentChance)
        {
            if (CurrentChance < (CurrentArmor * DamageMed))
            {
                CurrentChance = CurrentArmor * DamageMed;
                if (CurrentChance < (CurrentArmor * DamageMax)) CurrentChance = CurrentArmor * DamageMax; 
            }

            ComponentDamage(percentchance);
        }
    }

    void DestroyStructure()
    { 
        // Do Something
    }

    void ComponentDamage(float chance)
    { 
        //Determine if component damage occured
        //If so, Assemble a list of the available components going through the Panel list
        // Choose one of them, and damage it (Whatever that means)
    }
    */
}
