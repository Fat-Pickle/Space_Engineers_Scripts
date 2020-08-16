using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;
using Sandbox.Game.Gui;
using System.Threading;
using System.Reflection;
using VRageRender;
using System.Security.Cryptography.X509Certificates;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.

        
       
        // Change this value to however much credits you want to be entered
        // Do keep in mind how many credits small cargo containers can store on your world
        public int creditAmount = 40000;

        // If you have multiple gambling machines connected to the same grid then you should 
        // add some sort of additional charcaters to the name of the blocks in the control terminal 
        // AND set the additional charaters that you want to add below
        public string IdentificationPrefix = "";

        // If you've changed the Identification prefix while other gambling machines were present on
        // the same grid, then you'll have to go into the programmable block, go to "edit" and hit "ok" again (You might have to do it twice)

        // Enables an option that gaurantees that the user will not loose once, after designated amount of losses
        public bool gWin = true;

        // designate the amount of time a user can loose when you set gWin == true
        public int alLosses = 4;

        // Set the highest number you want to be generated 
        // The code counts from 0, so just add 1 to whatever the highest value is you want
        // Setting a higher value will decrease the chance of the user winning without the gWin option
        // Setting a higher value will also return a higher reward, so be careful or the user may not get his whole reward
        // The default value is the highest value for 1x inventories, for this particular machine
        public int mNum = 4;

/// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //Item type for space credits
        MyItemType creditType = MyItemType.Parse("MyObjectBuilder_PhysicalObject/SpaceCredit");

        IMyTextSurfaceProvider Primarydisplay;
        IMyCargoContainer CInput; //credits insertion container
        IMyCargoContainer CStorage; //Credit storage container
        IMyCargoContainer COutput; //credits output container

        public Program()
        {
            Storage = "";

            //assigns blocks to the IMy"thingies" so that they can be used on any grid with the right blocks
            Primarydisplay = GridTerminalSystem.GetBlockWithName("Primarydisplay" + IdentificationPrefix) as IMyTextSurfaceProvider;
            CInput = GridTerminalSystem.GetBlockWithName("CInput" + IdentificationPrefix) as IMyCargoContainer;
            COutput = GridTerminalSystem.GetBlockWithName("COutput" + IdentificationPrefix) as IMyCargoContainer;
            CStorage = GridTerminalSystem.GetBlockWithName("CStorage" + IdentificationPrefix) as IMyCargoContainer;
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.

        }

        public void Main(string argument, UpdateType updateSource)
        {
            Random rng = new Random();

            Echo(Storage);

            //various checks to see if the blocks exist
            if (CInput == null)
                Echo("No input container");

            if (COutput == null)
                Echo("No output container");

            if (CStorage == null)
                Echo("No storage container");

            if (Primarydisplay == null)
            {
                //The text surface is on the programmable block itself so this should never happen
                Echo("Display surface not detected on the programmable block");
            }

            //declaring CItem so that i can get an item in MyInventoryItem type
            MyInventoryItem CItem;

            //first message
            Primarydisplay.GetSurface(0).WriteText("Insert 40k\ncredits");

            if (CInput.GetInventory(0).GetItemAmount(creditType) >= creditAmount && !CInput.GetInventory(0).IsItemAt(1))
            {
                Echo("Credits entered");

                CItem = CInput.GetInventory(0).GetItemAt(0).Value;

                CInput.GetInventory(0).TransferItemTo(CStorage.GetInventory(0), CItem, creditAmount);

                MyInventoryItem CItem2 = CStorage.GetInventory(0).GetItemAt(0).Value;

                int n1 = rng.Next(1, mNum);
                int n2 = rng.Next(1, mNum);
                int n3 = rng.Next(1, mNum);

                if (  gWin == true && Storage.Length < alLosses )
                {
                    Primarydisplay.GetSurface(0).WriteText(n1.ToString() + " : " + n2.ToString() + " : " + n3.ToString());

                    if (n1 == n2 && n2 == n3)
                    {
                        Primarydisplay.GetSurface(0).WriteText(n1.ToString() + " : " + n2.ToString() + " : " + n3.ToString() + "\nMoney returned");
                        CStorage.GetInventory(0).TransferItemTo(COutput.GetInventory(0), CItem2, creditAmount*n1);
                        Storage = "";
                    }
                    else
                    {
                        Primarydisplay.GetSurface(0).WriteText(n1.ToString() + " : " + n2.ToString() + " : " + n3.ToString() + "\nNo prize");
                        Storage += "a";
                    }
                }
                else if(gWin == true && Storage.Length >= alLosses )
                {
                    int n4 = rng.Next(1, mNum);

                    Primarydisplay.GetSurface(0).WriteText(n4.ToString() + " : " + n4.ToString() + " : " + n4.ToString() + "\nYou won!");
                    CStorage.GetInventory(0).TransferItemTo(COutput.GetInventory(0), CItem2, creditAmount*n4);
                    Storage = "";
                }
                else
                {
                    Primarydisplay.GetSurface(0).WriteText(n1.ToString() + " : " + n2.ToString() + " : " + n3.ToString());

                    if ( n1 == n2 && n2==n3 )
                    {
                        Primarydisplay.GetSurface(0).WriteText(n1.ToString() + " : " + n2.ToString() + " : " + n3.ToString() + "\nMoney returned");
                        CStorage.GetInventory(0).TransferItemTo(COutput.GetInventory(0), CItem2, creditAmount*n1);
                    }
                    else
                    {
                        Primarydisplay.GetSurface(0).WriteText(n1.ToString() + " : " + n2.ToString() + " : " + n3.ToString() + "\nNo prize");
                    }
                }
            }
            else
            {
                Primarydisplay.GetSurface(0).WriteText("Insufficient\nfunds");
                Echo("Not enough credits or there are\nsome other items in the cargo container");
            }
        }

        //To here
    }
}
