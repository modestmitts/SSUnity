/************************************************************************************
                                        Panel Type
 This ensures that only the correct components are installed in the panel.
 This also determines what bulkhead assemblies (see below) will be connected to the panel.
 We will try to limit the panels to one type
 All parts can be placed in a Cargo Panel, but they will not be installed, or added to the ship functions, except
 Manufacturing Components
Cabin panels are connected to a Crew-Space 
Cargo panels are connected to an External-Bay or Cargo-Shifter.
CommSense panels are connected to an Antennae-Array.
Drone panels are connected to a Drone-Launcher
Engine panels are connected to a Thruster;
FuelComponent panels are connected to the Fuel-Line.
FuelTank panels are connected to Fuel-Intake.
Generator panels are connected to the Wiring-Harness.
Manipulator panels are connected to an external Machinery-Mount where tools may be mounted.
Projection panels are connected to an Emitter-Array
Weapon panels are connected to an external Hard-points where weapons maybe mounted
****************************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[System.Serializable]
public class PanelClass : MonoBehaviour 
{
    
}
