namespace Brewmaster.EditorWindows
{
	public interface IUndoable
	{
		void Undo();
		void Redo();
	}
}