using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;

namespace ContractDiscoveryEditorMargin
{
	class CategoryCompletionSource : ICompletionSource
	{

		public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
		{
			if (completionSets.Count == 0) return;

			var set = completionSets.Where(c => c.DisplayName == "All").SingleOrDefault();

			var categories = set.Completions.Select(s =>
					new Completion(s.DisplayText, s.InsertionText, s.Description,
					s.IconSource, s.IconAutomationText)).
				GroupBy(s => SetHelper.GetNameFromIconAutomationText(s.IconAutomationText));

			foreach (var set1 in categories.Select(catList =>
				new IntellisetsCompletionSet(catList.Key, set.ApplicableTo, catList, null)).
				ToList<CompletionSet>().AsReadOnly())
			{
				completionSets.Add(set1);
			}
		}

		
		public void Dispose()
		{
			
		}

		
	}

	class TestCompletionSource : ICompletionSource
	{
		TestCompletionSourceFactory _factory;
		ITextBuffer _textBuffer;
		//List<Completion> _compList;

		public TestCompletionSource(TestCompletionSourceFactory factory, ITextBuffer textBuffer)
		{
			_factory = factory;
			_textBuffer = textBuffer;

		
		}

		public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
		{
			var compList = new List<Completion>();
			//var completion = new Completion("Test", "Test Insertion", "Test Description", null, null);
			var completion = new TestCompletion("Test", session);

			compList.Add(completion);

			completionSets.Add(new CompletionSet(
				"Test",    //the non-localized title of the tab
				"Test",    //the displaytitle of the tab
				FindTokenSpanAtPosition(session.GetTriggerPoint(_textBuffer), session),
				compList,
				null));


		}

		


		private ITrackingSpan FindTokenSpanAtPosition(ITrackingPoint point, ICompletionSession session)
		{
			SnapshotPoint currentPoint = (session.TextView.Caret.Position.BufferPosition) - 1;
			ITextStructureNavigator navigator = _factory.NavigatorService.GetTextStructureNavigator(_textBuffer);
			TextExtent extent = navigator.GetExtentOfWord(currentPoint);
			return currentPoint.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
		}

		public void Dispose()
		{

		}
	}

}
