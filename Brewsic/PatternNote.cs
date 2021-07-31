namespace Brewsic
{
	public struct PatternNote
	{
		public bool HasNote;
		public bool HasVolumeEffect;
		public byte Note;
		public byte Instrument;
		public byte Effect;
		public byte EffectParam;
		public byte VolumeEffect;

		public override int GetHashCode()
		{
			return (int)Note | (Instrument << 8) | (Effect << 16) | (EffectParam << 24) | (VolumeEffect << 32) | (HasNote ? (1 << 40) : 0) | (HasVolumeEffect ? (1 << 41) : 0);
		}
	}
}