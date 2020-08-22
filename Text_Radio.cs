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
using System.Security.Cryptography.X509Certificates;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {

        //set the name of the antenna block that'll be used
        public string Antenna_Name = "Message_Antenna";

        //set the name of the LCD that'll be used
        public string LCD_Name = "Message_LCD";

        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        IMyRadioAntenna Transmitter; //The antenna which will transmit
        IMyTextPanel messagePanel; //The LCD which has the message

        IMyBroadcastListener _broadcastListener;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            Transmitter = GridTerminalSystem.GetBlockWithName(Antenna_Name) as IMyRadioAntenna;
            messagePanel = GridTerminalSystem.GetBlockWithName(LCD_Name) as IMyTextPanel;
        }

        public void Save()
        {

        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (Transmitter != null && messagePanel != null && Transmitter.IsWorking && messagePanel.IsWorking)
            {
                Echo("Setup ready");

                //splits the text from the LCD panel into 3 pieces, the mode
                //(transmit or receive), channel name and the actual message
                var pieces = messagePanel.GetText().Split(new[] { ':' }, 3);

                if (pieces.Length == 3)
                {
                    string Channel = pieces[1]; //gets the channel from the text panel

                    if (pieces[0]=="tran")
                    {
                        IGC.SendBroadcastMessage(Channel, pieces[2], TransmissionDistance.TransmissionDistanceMax);
                        Echo(messagePanel.GetText());
                    }
                    else if(pieces[0]=="rec")
                    {
                        _broadcastListener = IGC.RegisterBroadcastListener(Channel);

                        string messages = "";

                        while (_broadcastListener.HasPendingMessage)
                        {
                            MyIGCMessage myIGCMessage = _broadcastListener.AcceptMessage();

                            if (myIGCMessage.Tag == Channel)
                            {
                                if (myIGCMessage.Data is string)
                                {
                                    messages += myIGCMessage.Data.ToString();
                                }
                            }
                        }

                        messagePanel.WriteText(pieces[0] + ":" + Channel + ":" + messages);

                        Echo(messagePanel.GetText());
                    }
                    else
                    {
                        
                        Echo("Specify a mode, either 'rec' to receive signals or 'tran' to transmit them");
                    }
                }
                else
                {
                    Echo("The text on LCD should look like this: tran/rec:channel_name:Message ");
                }
            }
            else
            {
                Transmitter = GridTerminalSystem.GetBlockWithName(Antenna_Name) as IMyRadioAntenna;
                messagePanel = GridTerminalSystem.GetBlockWithName(LCD_Name) as IMyTextPanel;

                if (Transmitter == null)
                {
                    Echo(Antenna_Name + " not found");
                }

                if (messagePanel == null)
                {
                    Echo(LCD_Name + " not found");
                }
            }

        }

        //to here
    }
}
