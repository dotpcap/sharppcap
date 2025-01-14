// Copyright 2011 Chris Morgan <chmorgan@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace SharpPcap
{
    /// <summary>
    /// Live device, capable of both capture and injection
    /// </summary>
    public interface ILiveDevice : ICaptureDevice , IInjectionDevice
    {

    }
}

