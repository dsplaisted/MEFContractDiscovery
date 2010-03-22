using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System.Diagnostics;

namespace ContractDiscoveryEditorMargin
{
	class TestCompletionCommandHandler : IOleCommandTarget
	{
		IOleCommandTarget m_nextCommandHandler;
		ITextView m_textView;
		ICompletionBroker m_broker;

		//ICompletionSession m_session;

		internal TestCompletionCommandHandler(IVsTextView textViewAdapter, ITextView textView, ICompletionBroker broker)
		{
			this.m_textView = textView;
			this.m_broker = broker;

			//add the command to the command chain
			textViewAdapter.AddCommandFilter(this, out m_nextCommandHandler);
		}

		public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
		{
			if (nCmdID == (uint)VSConstants.VSStd2KCmdID.RETURN
				|| nCmdID == (uint)VSConstants.VSStd2KCmdID.TAB)
			{
				IEnumerable<ICompletionSession> sessions = m_broker.GetSessions(m_textView);
				sessions = sessions.Where(s => !s.IsDismissed && s.SelectedCompletionSet.SelectionStatus.IsSelected);
				if (sessions.Count() > 1)
				{
					Trace.WriteLine("Multiple selected sessions.");
				}
				var session = sessions.FirstOrDefault();
				if (session != null)
				{
					if (session.SelectedCompletionSet.SelectionStatus.Completion.InsertionText == "Test Insertion")
					{
						session.Dismiss();
						

						m_textView.TextBuffer.Insert(m_textView.Caret.Position.BufferPosition.Position, "Overridden Text!");
						return VSConstants.S_OK;
					}
				}
			}

			int retVal = m_nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);


			return retVal;
		}

		public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
		{
			return m_nextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
		}

	}

	//[Export(typeof(IVsTextViewCreationListener))]
	//[Name("test completion controller")]
	//[ContentType("code")]
	//[TextViewRole(PredefinedTextViewRoles.Editable)]
	class TestCompletionHandlerProvider : IVsTextViewCreationListener
	{
		[Import]
		internal IVsEditorAdaptersFactoryService AdapterService = null;

		[Import]
		internal ICompletionBroker CompletionBroker { get; set; }

		

		public void VsTextViewCreated(IVsTextView textViewAdapter)
		{
			ITextView textView = AdapterService.GetWpfTextView(textViewAdapter);
			if (textView == null)
				return;

			Func<TestCompletionCommandHandler> createCommandHandler = delegate() { return new TestCompletionCommandHandler(textViewAdapter, textView, CompletionBroker); };
			textView.Properties.GetOrCreateSingletonProperty(createCommandHandler);
		}

		
	}

}
