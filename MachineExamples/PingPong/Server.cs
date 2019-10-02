﻿// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------------------------------------------

using Microsoft.Coyote;
using Microsoft.Coyote.Machines;

namespace Coyote.Examples.PingPong
{
    /// <summary>
    /// A Coyote machine that models a simple server.
    ///
    /// It receives 'Ping' events from a client, and responds with a 'Pong' event.
    /// </summary>
    internal class Server : Machine
    {
        /// <summary>
        /// Event declaration of a 'Pong' event that does not contain any payload.
        /// </summary>
        internal class Pong : Event { }

        [Start]
        /// <summary>
        /// The 'OnEventDoAction' action declaration will execute (asynchrously)
        /// the 'SendPong' method, whenever a 'Ping' event is dequeued while the
        /// server machine is in the 'Active' state.
        /// </summary>
        [OnEventDoAction(typeof(Client.Ping), nameof(SendPong))]
        private class Active : MachineState { }

        private void SendPong()
        {
            // Receives a reference to a client machine (as a payload of
            // the 'Ping' event).
            var client = (this.ReceivedEvent as Client.Ping).Client;
            // Sends (asynchronously) a 'Pong' event to the client and make sure
            // it is handled at test time.
            this.Send(client, new Pong(), default, new SendOptions(mustHandle: true));
        }
    }
}
