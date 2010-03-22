using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;


namespace ContractDiscoveryEditorMargin
{
	class TestCompletion : Completion, ICustomCommit
	{
		ICompletionSession _session;

		public TestCompletion(string displayText, ICompletionSession session)
			:base(displayText)
		{
			_session = session;
		}

		public void Commit()
		{
			ITextSnapshot snapshot = _session.SelectedCompletionSet.ApplicableTo.TextBuffer.CurrentSnapshot;
			var edit =_session.TextView.TextBuffer.CreateEdit();
			edit.Delete(_session.SelectedCompletionSet.ApplicableTo.GetSpan(snapshot));
			edit.Insert(_session.SelectedCompletionSet.ApplicableTo.GetStartPoint(snapshot).Position, "Overridden edited text!");
			edit.Apply();
			//_session.TextView.TextBuffer.Insert(_session.TextView.Caret.Position.BufferPosition.Position, "Overridden Text!!!");
		}

	}
}
