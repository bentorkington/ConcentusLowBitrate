using Concentus.Structs;
using NAudio.Utils;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

int frameSize = 960;

var sg = new SignalGenerator(48000, 2)
{
    Type = SignalGeneratorType.Sin,
    Frequency = 440,
    Gain = Decibels.DecibelsToLinear(-18),  // -18dB is a fairly standard level for calibration tones that doesn't risk bursting eardrums
};
var sp = sg.ToWaveProvider16();

int buflen = frameSize 
    * 2 /* bytes per sample */ 
    * 2 /* channels */;

var sineBytes = new byte[buflen];
var converterBuffer = new WaveBuffer(sineBytes);
sp.Read(sineBytes, 0, buflen);

var encodedOpusBytes = new byte[20000];
var decodeDestination = new byte[5760 * 2];
var decodeBuffer = new WaveBuffer(decodeDestination);

var encoder = new OpusEncoder(48000, 2, Concentus.Enums.OpusApplication.OPUS_APPLICATION_AUDIO);
var decoder = new OpusDecoder(48000, 2);

encoder.Bitrate = 8000;

var bytesEncoded = encoder.Encode(converterBuffer.ShortBuffer, 0, frameSize, encodedOpusBytes, 0, encodedOpusBytes.Length);
Console.WriteLine($"Encoded 10ms to {bytesEncoded} bytes");
var decodedBytes = decoder.Decode(encodedOpusBytes, 0, bytesEncoded, decodeBuffer.ShortBuffer, 0, 5760);
Console.WriteLine($"Decoded 10ms to {decodedBytes} samples");
