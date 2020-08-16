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

namespace IngameScript
{
    partial class Program : MyGridProgram
    {


        //Adds an identification postfix to the block names that the script uses (except the antenna) 
        //in case you want to have multiple receiver scripts on the same grid
        public string postfix = "";

/// ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        IMyRadioAntenna Receiver;
        IMyTextPanel messagePanel;

        IMyBroadcastListener _broadcastListener;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            Receiver = GridTerminalSystem.GetBlockWithName("Message_Antenna") as IMyRadioAntenna;
            messagePanel = GridTerminalSystem.GetBlockWithName("Receiver_Message" + postfix) as IMyTextPanel;
        }

        public void Save()
        {

        }

        public string data;

        public void Main(string argument, UpdateType updateSource)
        {
            if (Receiver != null && messagePanel != null && Receiver.IsWorking && messagePanel.IsWorking)
            {
                Echo("Setup ready");

                var pieces = messagePanel.GetText().Split(new[] { ':' }, 2);

                string Channel = pieces[0]; //gets the channel from the text panel

                _broadcastListener = IGC.RegisterBroadcastListener(Channel);

                while (_broadcastListener.HasPendingMessage)
                {
                    MyIGCMessage myIGCMessage = _broadcastListener.AcceptMessage();

                    if (myIGCMessage.Tag == Channel)
                    {
                        if (myIGCMessage.Data is string)
                        {
                            messagePanel.WriteText(Channel+":"+myIGCMessage.Data.ToString());
                        }
                    }
                }
            }
            else
            {
                Receiver = GridTerminalSystem.GetBlockWithName("Message_Antenna") as IMyRadioAntenna;
                messagePanel = GridTerminalSystem.GetBlockWithName("Receiver_Message" + postfix) as IMyTextPanel;

                if (Receiver==null)
                {
                    Echo("'Message_Antenna'" + " Antenna block not found");
                }

                if (messagePanel == null)
                {
                    Echo("'Receiver_Message" + postfix + "'" + " LCD panel not found");
                }
            }
        }

        //to here
    }
}
