using System;
using System.ComponentModel;
using System.Runtime.Remoting.Lifetime;
using System.Threading;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using SharpDX.XInput;

namespace IrishSpring
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool completed = true;

            // Initialize the physical controller
            var physicalController = new Controller(UserIndex.One);

            // If controller is not connected, exit the program
            if (!physicalController.IsConnected)
            {
                Console.WriteLine("Xbox controller is not connected.");
                Thread.Sleep(2000);
                return;
            }

            using (var vigemClient = new ViGEmClient())
            {
                var virtualController = vigemClient.CreateXbox360Controller();
                virtualController.Connect();

                // Run this loop while the controller is connected
                while (physicalController.IsConnected)
                {
                    // Get the current state of the physical controller
                    var state = physicalController.GetState();

                    // Map each button press/stick movement on the physical controller to the same input on the virtual controller
                    virtualController.SetButtonState(Xbox360Button.A, state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.A));
                    virtualController.SetButtonState(Xbox360Button.B, state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.B));
                    virtualController.SetButtonState(Xbox360Button.Y, state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Y));
                    virtualController.SetButtonState(Xbox360Button.Start, state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Start));
                    virtualController.SetButtonState(Xbox360Button.Back, state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Back));
                    virtualController.SetButtonState(Xbox360Button.LeftThumb, state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftThumb));
                    virtualController.SetButtonState(Xbox360Button.RightThumb, state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb));

                    // Set trigger values
                    virtualController.SetSliderValue(Xbox360Slider.LeftTrigger, state.Gamepad.LeftTrigger);
                    virtualController.SetSliderValue(Xbox360Slider.RightTrigger, state.Gamepad.RightTrigger);

                    // Set thumbstick values
                    virtualController.SetAxisValue(Xbox360Axis.LeftThumbX, state.Gamepad.LeftThumbX);
                    virtualController.SetAxisValue(Xbox360Axis.LeftThumbY, state.Gamepad.LeftThumbY);
                    virtualController.SetAxisValue(Xbox360Axis.RightThumbX, state.Gamepad.RightThumbX);
                    virtualController.SetAxisValue(Xbox360Axis.RightThumbY, state.Gamepad.RightThumbY);

                    // For the X button, translate a single press into a 2-second press
                    if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.X) && completed)
                    {
                        virtualController.SetButtonState(Xbox360Button.X, true);
                        Thread.Sleep(485); //LeBron
                        virtualController.SetButtonState(Xbox360Button.X, false);
                        Thread.Sleep(100);
                    }
                    else
                    {
                        virtualController.SetButtonState(Xbox360Button.X, false);
                    }

                    // Check if the left shoulder button is pressed and simulate X button being pressed for 5 seconds
                    if (state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder) && completed)
                    {
                        virtualController.SetButtonState(Xbox360Button.X, true);
                        Thread.Sleep(619);
                        virtualController.SetButtonState(Xbox360Button.X, false);
                        Thread.Sleep(100);
                    }
                    else
                    {
                        virtualController.SetButtonState(Xbox360Button.LeftShoulder, false);
                    }

                    virtualController.SetButtonState(Xbox360Button.RightShoulder, state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder));

                                     
                }

                virtualController.Disconnect();
            }
        }
    }
}