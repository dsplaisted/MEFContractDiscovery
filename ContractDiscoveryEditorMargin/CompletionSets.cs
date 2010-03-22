using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace ContractDiscoveryEditorMargin
{
	internal class IntellisetsCompletionSet : CompletionSet
	{
		internal IntellisetsCompletionSet(String displayName, ITrackingSpan applicableTo,
											IEnumerable<Completion> completions,
											IEnumerable<Completion> completionBuilders)
			: base(displayName, displayName,
				applicableTo, completions, completionBuilders) { }

		public override void SelectBestMatch()
		{
			ITextSnapshot snapshot = ApplicableTo.TextBuffer.CurrentSnapshot;
			string text = ApplicableTo.GetText(snapshot);

			if (!String.IsNullOrEmpty(text))
			{
				var completion = Completions.
				FirstOrDefault(c => !String.IsNullOrEmpty(c.DisplayText) &&
				c.DisplayText.IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1);

				if (completion != null)
					SelectionStatus = new CompletionSelectionStatus(completion, true, false);
			}
		}
	}
}
