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

namespace IngameScript
{
    partial class Program : MyGridProgram
    {

        //Adds an identification postfix to the block names in case you 
        //want to have multiple transmision scripts on the same grid
        public string postfix = "";

/// ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        IMyRadioAntenna Transmitter; //The antenna which will transmit
        IMyTextPanel messagePanel; //The LCD which has the message

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            Transmitter = GridTerminalSystem.GetBlockWithName("Message_Antenna") as IMyRadioAntenna;
            messagePanel = GridTerminalSystem.GetBlockWithName("Transmitter_Message"+postfix) as IMyTextPanel;
        }

        public void Save()
        {
            
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (Transmitter != null && messagePanel != null && Transmitter.IsWorking && messagePanel.IsWorking)
            {
                Echo("Setup ready");

                if (messagePanel.GetText().Length>=2)
                {
                    var pieces = messagePanel.GetText().Split(new[] { ':' }, 2);

                    string Channel = pieces[0]; //gets the channel from the text panel

                    IGC.SendBroadcastMessage(Channel, pieces[1], TransmissionDistance.TransmissionDistanceMax);
                    Echo(messagePanel.GetText());
                }
            }
            else
            {
                Transmitter = GridTerminalSystem.GetBlockWithName("Message_Antenna") as IMyRadioAntenna;
                messagePanel = GridTerminalSystem.GetBlockWithName("Transmitter_Message" + postfix) as IMyTextPanel;

                if (Transmitter == null)
                {
                    Echo("'Message_Antenna'" + " Antenna block not found");
                }

                if (messagePanel == null)
                {
                    Echo("'Transmitter_Message"+ postfix + "'" + " LCD panel not found");
                }
            }
            
        }

        //to here
    }
}
