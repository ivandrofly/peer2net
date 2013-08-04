﻿//
// - BandwidthController.cs
// 
// Author:
//     Lucas Ontivero <lucasontivero@gmail.com>
// 
// Copyright 2013 Lucas E. Ontivero
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//  http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

// <summary></summary>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using P2PNet.Utils;

namespace P2PNet.Progress
{
    public class BandwidthController
    {
        private readonly PidController _pidController;
        private int _targeSpeed;
        private int _accumulatedBytes;

        internal BandwidthController()
        {
            _pidController = new PidController();
            _targeSpeed = Int32.MaxValue;
            _accumulatedBytes = Int32.MaxValue;
        }

        public int TargetSpeed
        {
            get { return _targeSpeed; }
            set
            {
                Guard.IsBeetwen(value, 0, int.MaxValue, "value");
                _targeSpeed = value;
                _accumulatedBytes = _targeSpeed;
            }
        }

        internal bool CanTransmit(int bytesCount)
        {
            if (bytesCount > 0 && _accumulatedBytes < bytesCount) return false;

            _accumulatedBytes -= bytesCount;
            return true;
        }

        internal void Update(double measuredSpeed, TimeSpan deltaTime)
        {
            var seconds = deltaTime.TotalMilliseconds / 1000.0;
            var deltaSpeed = _targeSpeed - measuredSpeed;

            var correction = _pidController.Control(deltaSpeed, seconds);
            _accumulatedBytes += (int)(correction);
            _accumulatedBytes = _accumulatedBytes > 0 ? _accumulatedBytes : Int32.MaxValue;
        }
    }
}