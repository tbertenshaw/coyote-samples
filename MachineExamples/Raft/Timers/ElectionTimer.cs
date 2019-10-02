﻿// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------------------------------------------

using System;
using Microsoft.Coyote;
using Microsoft.Coyote.Machines;

namespace Coyote.Examples.Raft
{
    internal class ElectionTimer : Machine
    {
        internal class ConfigureEvent : Event
        {
            public MachineId Target;

            public ConfigureEvent(MachineId id)
                : base()
            {
                this.Target = id;
            }
        }

        internal class StartTimerEvent : Event { }

        internal class CancelTimerEvent : Event { }

        internal class TimeoutEvent : Event { }

        private class TickEvent : Event { }

        private MachineId Target;

        [Start]
        [OnEventDoAction(typeof(ConfigureEvent), nameof(Configure))]
        [OnEventGotoState(typeof(StartTimerEvent), typeof(Active))]
        private class Init : MachineState { }

        private void Configure()
        {
            this.Target = (this.ReceivedEvent as ConfigureEvent).Target;
            // this.Raise(new StartTimerEvent());
        }

        [OnEntry(nameof(ActiveOnEntry))]
        [OnEventDoAction(typeof(TickEvent), nameof(Tick))]
        [OnEventGotoState(typeof(CancelTimerEvent), typeof(Inactive))]
        [IgnoreEvents(typeof(StartTimerEvent))]
        private class Active : MachineState { }

        private void ActiveOnEntry()
        {
            this.Send(this.Id, new TickEvent());
        }

        private void Tick()
        {
            if (this.Random())
            {
                this.Logger.WriteLine("\n [ElectionTimer] " + this.Target + " | timed out\n");
                this.Send(this.Target, new TimeoutEvent());
            }

            // this.Send(this.Id, new TickEvent());
            this.Raise(new CancelTimerEvent());
        }

        [OnEventGotoState(typeof(StartTimerEvent), typeof(Active))]
        [IgnoreEvents(typeof(CancelTimerEvent), typeof(TickEvent))]
        private class Inactive : MachineState { }
    }
}
