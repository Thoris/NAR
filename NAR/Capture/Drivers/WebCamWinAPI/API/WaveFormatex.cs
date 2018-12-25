/******************************************************************************
* Copyright (c) 2012, TAP Consulting Group
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither TAP Consulting Group nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY TAP Consulting Group ''AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL TAP Consulting Group BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace NAR.Capture.Drivers.WebCamWinAPI.API
{

    /// <summary>
    /// structure defines the format of waveform-audio data
    /// </summary>
    public struct WaveFormatex
    {
        #region Properties
        /// <summary>
        /// Specifies the audio waveform type. A complete list of format tags can be found in the Mmreg.h header file. 
        /// See Audio Subtypes for more information on values to use for multichannel audio.
        /// </summary>
        public short wFormatTag;
        /// <summary>
        /// Specifies the number of channels in the audio stream
        /// </summary>
        public short nChannels;
        /// <summary>
        /// Specifies the sample rate in samples/second (Hz). Common values are 8,000; 11,025; 22,050; and 44,100
        /// </summary>
        public int nSamplesPerSec;
        /// <summary>
        /// Specifies the average data rate, in bytes per second. If wFormatTag is WAVE_FORMAT_PCM, nAvgBytesPerSec 
        /// must equal the product of nSamplesPerSec and nBlockAlign
        /// </summary>
        public int nAvgBytesPerSec;
        /// <summary>
        /// Specifies the block alignment, in bytes. The block alignment is the minimum atomic unit of data. This 
        /// value can be used for buffer alignment. If wFormatTag is WAVE_FORMAT_PCM, nBlockAlign must equal the 
        /// product of nChannels and wBitsPerSample divided by 8
        /// </summary>
        public short nBlockAlign;
        /// <summary>
        /// Specifies the number of bits per sample. Each channel is assumed to have the same sample resolution. 
        /// If wFormatTag is WAVE_FORMAT_PCM, then wBitsPerSample should be 8 or 16. For compressed audio, the value might be zero
        /// </summary>
        public short wBitsPerSample;
        /// <summary>
        /// Specifies the size of the extra information in the format header, in bytes, not including the size of the WAVEFORMATEX 
        /// structure. If there is no extra information, set the value to zero.  If wFormatTag is WAVE_FORMAT_PCM, this value is ignored
        /// </summary>
        public short cbSize;
        #endregion
    }
}
