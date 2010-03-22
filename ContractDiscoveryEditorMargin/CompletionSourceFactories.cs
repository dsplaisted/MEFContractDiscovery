using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Operations;

namespace ContractDiscoveryEditorMargin
{
	//[Export(typeof(ICompletionSourceProvider))]
	//[Name("IntellisetsCompletionProvider")]
	//[ContentType("text")]
	//[Order(After = "default")]
	class CategoryCompletionSourceFactory : ICompletionSourceProvider
	{
		public ICompletionSource TryCreateCompletionSource(Microsoft.VisualStudio.Text.ITextBuffer textBuffer)
		{
			return new CategoryCompletionSource();
		}
	}

	[Export(typeof(ICompletionSourceProvider))]
	[Name("TestCompletionProvider")]
	[ContentType("code")]
	[Order(After = "default")]
	class TestCompletionSourceFactory : ICompletionSourceProvider
	{
		[Import]
		internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

		public ICompletionSource TryCreateCompletionSource(Microsoft.VisualStudio.Text.ITextBuffer textBuffer)
		{
			return new TestCompletionSource(this, textBuffer);
		}
	}

}
