using Music.ViewModels;

namespace SheetLearner.Music.ViewModels
{
	public class Stem
	{
		public enum Horizontal
		{
			Left,
			Right,
			Mid
		}

		public enum Direction
		{
			Up,
			Down
		}

		public int PosX()
		{
			if (HorizontalOrientaion == Horizontal.Mid)
				return 14;
			else if (HorizontalOrientaion == Horizontal.Left)
				return 3;

			return 14;
		}

		public int Start()
		{
			var offsets = StemDirection == Direction.Up ?
				new { x = 3, y = 3, noteIndexCorrection = 0 } :
				new { x = 14, y = ClefViewModel.NoteHeight, noteIndexCorrection = -2 };

			return offsets.y;
		}

		public Horizontal HorizontalOrientaion { get; set; }
		public Direction StemDirection { get; set; }
	}
}
