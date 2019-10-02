﻿// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------------------------------------------

using System;
using Microsoft.Coyote;
using Microsoft.Coyote.Machines;

namespace Coyote.Examples.CacheCoherence
{
    internal class CPU : Machine
    {
        internal class Config : Event
        {
            public Tuple<MachineId, MachineId, MachineId> Clients;

            public Config(Tuple<MachineId, MachineId, MachineId> clients)
                : base()
            {
                this.Clients = clients;
            }
        }

        internal class AskShare : Event { }

        internal class AskExcl : Event { }

        private class Unit : Event { }

        private Tuple<MachineId, MachineId, MachineId> Cache;

        [Start]
        [OnEventDoAction(typeof(Config), nameof(Configure))]
        [OnEventGotoState(typeof(Unit), typeof(MakingReq))]
        internal class Init : MachineState { }

        internal void Configure()
        {
            this.Cache = (this.ReceivedEvent as Config).Clients;
            this.Raise(new Unit());
        }

        [OnEntry(nameof(MakingReqOnEntry))]
        [OnEventGotoState(typeof(Unit), typeof(MakingReq))]
        internal class MakingReq : MachineState { }

        internal void MakingReqOnEntry()
        {
            if (this.Random())
            {
                if (this.Random())
                {
                    this.Send(this.Cache.Item1, new AskShare());
                }
                else
                {
                    this.Send(this.Cache.Item1, new AskExcl());
                }
            }
            else if (this.Random())
            {
                if (this.Random())
                {
                    this.Send(this.Cache.Item2, new AskShare());
                }
                else
                {
                    this.Send(this.Cache.Item2, new AskExcl());
                }
            }
            else
            {
                if (this.Random())
                {
                    this.Send(this.Cache.Item3, new AskShare());
                }
                else
                {
                    this.Send(this.Cache.Item3, new AskExcl());
                }
            }

            this.Raise(new Unit());
        }
    }
}
