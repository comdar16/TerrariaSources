using System.IO;
using Microsoft.Xna.Framework.Audio;
using NVorbis;

namespace Terraria.Audio
{
	public class OGGAudioTrack : ASoundEffectBasedAudioTrack
	{
		private VorbisReader _vorbisReader;

		private int _loopStart;

		private int _loopEnd;

		public OGGAudioTrack(Stream streamToRead)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			_vorbisReader = new VorbisReader(streamToRead, true);
			FindLoops();
			CreateSoundEffect(_vorbisReader.SampleRate, (AudioChannels)_vorbisReader.Channels);
		}

		protected override void ReadAheadPutAChunkIntoTheBuffer()
		{
			PrepareBufferToSubmit();
			_soundEffectInstance.SubmitBuffer(_bufferToSubmit);
		}

		private void PrepareBufferToSubmit()
		{
			byte[] bufferToSubmit = this._bufferToSubmit;
			float[] temporaryBuffer = this._temporaryBuffer;
			VorbisReader vorbisReader = this._vorbisReader;
			int num = vorbisReader.ReadSamples(temporaryBuffer, 0, temporaryBuffer.Length);
			bool flag = this._loopEnd > 0 && vorbisReader.DecodedPosition >= (long)this._loopEnd;
			bool flag2 = num < temporaryBuffer.Length;
			if (flag || flag2)
			{
				vorbisReader.SamplePosition = (long)this._loopStart;
				vorbisReader.ReadSamples(temporaryBuffer, num, temporaryBuffer.Length - num);
			}
			OGGAudioTrack.ApplyTemporaryBufferTo(temporaryBuffer, bufferToSubmit);
		}

		private static void ApplyTemporaryBufferTo(float[] temporaryBuffer, byte[] samplesBuffer)
		{
			for (int i = 0; i < temporaryBuffer.Length; i++)
			{
				short num = (short)(temporaryBuffer[i] * 32767f);
				samplesBuffer[i * 2] = (byte)num;
				samplesBuffer[i * 2 + 1] = (byte)(num >> 8);
			}
		}

		public override void Reuse()
		{
			_vorbisReader.SeekTo(0L, SeekOrigin.Begin);
		}

		private void FindLoops()
		{
			string[] comments = _vorbisReader.Comments;
			foreach (string vorbisComment in comments)
			{
				TryGettingVariable(vorbisComment, "LOOPSTART", ref _loopStart);
				TryGettingVariable(vorbisComment, "LOOPEND", ref _loopEnd);
			}
		}

		private void TryGettingVariable(string vorbisComment, string variableWeLookFor, ref int variableValueHolder)
		{
			if (vorbisComment.StartsWith(variableWeLookFor) && int.TryParse(vorbisComment, out var result))
			{
				variableValueHolder = result;
			}
		}

		public override void Dispose()
		{
			_soundEffectInstance.Dispose();
			_vorbisReader.Dispose();
		}
	}
}
